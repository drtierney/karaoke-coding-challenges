using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;


namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public class LibraryReconciliationService
{
    public LibraryReconciliationResult Reconcile(
        long sourceId,
        IReadOnlyList<MediaTrackRecord> storedTracks,
        IReadOnlyCollection<string> scannedPaths,
        bool scanCompletedSuccessfully,
        IReadOnlyList<FileFingerprint>? newFiles = null)
    {
        HashSet<string> scannedPathSet = new(scannedPaths, StringComparer.OrdinalIgnoreCase);

        List<MediaTrackRecord> missingTracks = scanCompletedSuccessfully
            ? storedTracks.Where(track => !scannedPathSet.Contains(track.FilePath)).ToList()
            : [];

        List<MediaTrackRecord> presentTracks = scanCompletedSuccessfully
            ? storedTracks.Where(track => scannedPathSet.Contains(track.FilePath)).ToList()
            : [];


        List<FileMoveMatch> movedTracks = [];

        if (scanCompletedSuccessfully && newFiles is not null)
        {
            FileMoveDetector detector = new();

            movedTracks = detector.DetectMoves(missingTracks, newFiles).ToList();

            HashSet<long> movedTrackIds = movedTracks
                .Select(move => move.TrackId)
                .ToHashSet();

            missingTracks.RemoveAll(track => movedTrackIds.Contains(track.Id));
        }


        return new LibraryReconciliationResult
        {
            SourceId = sourceId,
            StoredTrackCount = storedTracks.Count,
            ScannedTrackCount = scannedPaths.Count,
            MissingTracks = missingTracks,
            PresentTracks = presentTracks,
            MovedTracks = movedTracks,
            ScanCompletedSuccessfully = scanCompletedSuccessfully
        };
    }
}
