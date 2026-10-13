using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

namespace KaraokeCodingChallenges.IncrementalScanning;

public class IncrementalScanService
{
    private readonly MediaTrackRepository repository;
    private readonly FileFingerprintProvider fingerprintProvider;
    private readonly FileChangeDetector changeDetector;

    public IncrementalScanService(MediaTrackRepository repository, FileFingerprintProvider fingerprintProvider, FileChangeDetector changeDetector)
    {
        this.repository = repository;
        this.fingerprintProvider = fingerprintProvider;
        this.changeDetector = changeDetector;
    }

    public FileChangeStatus CheckFile(string filePath)
    {
        FileFingerprint fingerprint = fingerprintProvider.GetFingerprint(filePath);

        MediaTrackRecord? existingTrack = repository.GetByFilePath(fingerprint.FilePath);

        if (existingTrack == null)
        {
            return FileChangeStatus.New;
        }

        if (existingTrack.FileSize == null || existingTrack.LastModifiedUtc == null)
        {
            return FileChangeStatus.Modified;
        }

        FileFingerprint previousFingerprint = new FileFingerprint
        {
            FilePath = existingTrack.FilePath,
            FileSize = existingTrack.FileSize.Value,
            LastModifiedUtc = existingTrack.LastModifiedUtc.Value
        };

        return changeDetector.DetectChange(fingerprint, previousFingerprint);
    }
}
