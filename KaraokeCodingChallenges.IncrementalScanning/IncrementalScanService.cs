using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.ScanResult;
using KaraokeCodingChallenges.MediaScanService;
using KaraokeCodingChallenges.MetadataReader;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;

namespace KaraokeCodingChallenges.IncrementalScanning;

public class IncrementalScanService
{
    private readonly MediaTrackRepository repository;
    private readonly FileFingerprintProvider fingerprintProvider;
    private readonly FileChangeDetector changeDetector;
    private readonly ScannedTrackPersistenceService persistenceService;
    private readonly KaraokeLibraryPersistenceService? karaokePersistenceService;

    // Original constructor - maintains compatibility with existing tests
    public IncrementalScanService(
        MediaTrackRepository repository,
        FileFingerprintProvider fingerprintProvider,
        FileChangeDetector changeDetector,
        ScannedTrackPersistenceService persistenceService)
        : this(
            repository,
            fingerprintProvider,
            changeDetector,
            persistenceService,
            null)
    {
    }

    // New constructor - supports transactional MP3+CDG persistence
    public IncrementalScanService(
        MediaTrackRepository repository,
        FileFingerprintProvider fingerprintProvider,
        FileChangeDetector changeDetector,
        ScannedTrackPersistenceService persistenceService,
        KaraokeLibraryPersistenceService? karaokePersistenceService)
    {
        this.repository = repository;
        this.fingerprintProvider = fingerprintProvider;
        this.changeDetector = changeDetector;
        this.persistenceService = persistenceService;
        this.karaokePersistenceService = karaokePersistenceService;
    }

    private FileChangeStatus CheckFingerprint(FileFingerprint fingerprint)
    {
        MediaTrackRecord? existingTrack = repository.GetByFilePath(fingerprint.FilePath);

        if (existingTrack == null)
        {
            return FileChangeStatus.New;
        }

        if (existingTrack.FileSize == null ||
            existingTrack.LastModifiedUtc == null)
        {
            return FileChangeStatus.Modified;
        }

        FileFingerprint previousFingerprint = new FileFingerprint
        {
            FilePath = existingTrack.FilePath,
            FileSize = existingTrack.FileSize.Value,
            LastModifiedUtc = existingTrack.LastModifiedUtc.Value
        };

        return changeDetector.DetectChange(
            fingerprint,
            previousFingerprint);
    }

    public FileChangeStatus CheckFile(string filePath)
    {
        FileFingerprint fingerprint =
            fingerprintProvider.GetFingerprint(filePath);

        return CheckFingerprint(fingerprint);
    }

    private MediaScanResult ProcessChangedFile(
    FileFingerprint fingerprint,
    long sourceId,
    IMetadataReader metadataReader,
    bool isKaraoke,
    string? cdgFilePath)
    {
        MediaScanResult scanResult = MediaScannerService.ScanFile(fingerprint.FilePath, metadataReader);

        if (!scanResult.IsSuccess || scanResult.Metadata is null)
        {
            return scanResult;
        }

        FileFingerprint latestFingerprint = fingerprintProvider.GetFingerprint(fingerprint.FilePath);

        if (changeDetector.DetectChange(latestFingerprint, fingerprint)
            != FileChangeStatus.Unchanged)
        {
            return scanResult with
            {
                IsSuccess = false,
                Metadata = null,
                Errors = ["File changed during metadata extraction."]
            };
        }

        if (isKaraoke && cdgFilePath is not null)
        {
            if (karaokePersistenceService is null)
            {
                throw new InvalidOperationException("Karaoke persistence service is required for MP3+CDG pairs.");
            }

            karaokePersistenceService.PersistPair(
                scanResult,
                cdgFilePath,
                sourceId,
                fileSize: fingerprint.FileSize,
                lastModifiedUtc: fingerprint.LastModifiedUtc);
        }
        else
        {
            persistenceService.Persist(
                scanResult,
                sourceId,
                isKaraoke,
                fingerprint.FileSize,
                fingerprint.LastModifiedUtc);
        }

        return scanResult;
    }

    public MediaScanResult? ProcessFile(
        string filePath,
        long sourceId,
        IMetadataReader metadataReader,
        bool isKaraoke = false,
        string? cdgFilePath = null)
    {
        FileFingerprint fingerprint = fingerprintProvider.GetFingerprint(filePath);

        FileChangeStatus status = CheckFingerprint(fingerprint);

        if (status == FileChangeStatus.Unchanged)
        {
            return null;
        }

        return ProcessChangedFile(
            fingerprint,
            sourceId,
            metadataReader,
            isKaraoke,
            cdgFilePath);
    }

    public IncrementalScanSummary ProcessFiles(
        IEnumerable<string> filePaths,
        long sourceId,
        IMetadataReader metadataReader,
        bool isKaraoke = false,
        IReadOnlyDictionary<string, string>? cdgPathsByMp3 = null)
    {
        IncrementalScanSummary summary = new IncrementalScanSummary();

        foreach (string filePath in filePaths)
        {
            summary.Total++;

            try
            {
                string? cdgFilePath = null;

                if (isKaraoke && cdgPathsByMp3 is not null)
                {
                    cdgPathsByMp3.TryGetValue(filePath, out cdgFilePath);
                }

                FileFingerprint fingerprint = fingerprintProvider.GetFingerprint(filePath);

                FileChangeStatus status = CheckFingerprint(fingerprint);

                if (status == FileChangeStatus.Unchanged)
                {
                    summary.Skipped++;
                    continue;
                }

                MediaScanResult result = ProcessChangedFile(
                    fingerprint,
                    sourceId,
                    metadataReader,
                    isKaraoke,
                    cdgFilePath);

                if (!result.IsSuccess || result.Metadata is null)
                {
                    summary.Failed++;
                    continue;
                }

                if (status == FileChangeStatus.New)
                {
                    summary.Added++;
                }
                else if (status == FileChangeStatus.Modified)
                {
                    summary.Updated++;
                }
            }
            catch (FileNotFoundException)
            {
                summary.Failed++;
            }
            catch (DirectoryNotFoundException)
            {
                summary.Failed++;
            }
            catch (UnauthorizedAccessException)
            {
                summary.Failed++;
            }
            catch (IOException)
            {
                summary.Failed++;
            }
        }

        return summary;
    }


}
