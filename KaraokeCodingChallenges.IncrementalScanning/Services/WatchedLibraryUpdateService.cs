using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MediaScanService;
using KaraokeCodingChallenges.MetadataReader;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public class WatchedLibraryUpdateService
{
    private readonly IMediaTrackRepository _trackRepository;
    private readonly ScannedTrackPersistenceService _persistenceService;
    private readonly IMetadataReader _metadataReader;
    private readonly FileChangeDetector _changeDetector;
    private readonly FileFingerprintProvider _fingerprintProvider;

    public WatchedLibraryUpdateService(
        IMediaTrackRepository trackRepository,
        ScannedTrackPersistenceService persistenceService,
        IMetadataReader metadataReader,
        FileFingerprintProvider fingerprintProvider)
    {
        ArgumentNullException.ThrowIfNull(trackRepository);
        ArgumentNullException.ThrowIfNull(persistenceService);
        ArgumentNullException.ThrowIfNull(metadataReader);
        ArgumentNullException.ThrowIfNull(fingerprintProvider);

        _trackRepository = trackRepository;
        _persistenceService = persistenceService;
        _metadataReader = metadataReader;
        _fingerprintProvider = fingerprintProvider;

        _changeDetector = new FileChangeDetector();
    }

    public bool HandleDeleted(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        MediaTrackRecord? existingTrack = _trackRepository.GetByFilePath(filePath);

        if (existingTrack is null)
        {
            return false;
        }

        MediaTrackRecord unavailableTrack = existingTrack with
        {
            IsAvailable = false
        };

        return _trackRepository.Update(unavailableTrack);
    }

    public bool HandleRenamed(string oldFilePath, string newFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(oldFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newFilePath);

        MediaTrackRecord? existingTrack = _trackRepository.GetByFilePath(oldFilePath);
        MediaTrackRecord? newTrack = _trackRepository.GetByFilePath(newFilePath);

        if (existingTrack is null)
        {
            return false;
        }

        if (newTrack is not null && existingTrack.Id != newTrack.Id)
        {
            return false;
        }

        MediaTrackRecord renamedTrack = existingTrack with
        {
            FilePath = newFilePath
        };

        return _trackRepository.Update(renamedTrack);
    }

    public long? HandleCreated(string filePath, long sourceId, bool isKaraoke = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        MediaScanResult scanResult = MediaScannerService.ScanFile(filePath, _metadataReader);

        if (!scanResult.IsSuccess)
        {
            return null;
        }

        FileFingerprint fingerprint = _fingerprintProvider.GetFingerprint(filePath);

        return _persistenceService.Persist(
            scanResult,
            sourceId,
            isKaraoke,
            fingerprint.FileSize,
            fingerprint.LastModifiedUtc);
    }

    public long? HandleChanged(string filePath, long sourceId, bool isKaraoke = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        MediaTrackRecord? existingTrack = _trackRepository.GetByFilePath(filePath);

        if (existingTrack is null)
        {
            return HandleCreated(filePath, sourceId, isKaraoke);
        }

        if (existingTrack.FileSize is null || existingTrack.LastModifiedUtc is null)
        {
            return HandleCreated(filePath, sourceId, isKaraoke);
        }

        FileFingerprint currentFingerprint = _fingerprintProvider.GetFingerprint(filePath);

        FileFingerprint previousFingerprint = new()
        {
            FilePath = existingTrack.FilePath,
            FileSize = existingTrack.FileSize.Value,
            LastModifiedUtc = existingTrack.LastModifiedUtc.Value
        };

        FileChangeStatus status = _changeDetector.DetectChange(currentFingerprint, previousFingerprint);

        if (status == FileChangeStatus.Unchanged && existingTrack.IsAvailable)
        {
            return existingTrack.Id;
        }

        return HandleCreated(filePath, sourceId, isKaraoke);
    }
}
