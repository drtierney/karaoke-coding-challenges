using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;


namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public class LibraryReconciliationService
{
    public LibraryReconciliationResult Reconcile(
        int sourceId,
        IReadOnlyList<MediaTrackRecord> storedTracks,
        IReadOnlyCollection<string> scannedPaths,
        bool scanCompletedSuccessfully)
    {
        HashSet<string> scannedPathSet = new(scannedPaths, StringComparer.OrdinalIgnoreCase);

        List<MediaTrackRecord> missingTracks = scanCompletedSuccessfully
            ? storedTracks.Where(track => !scannedPathSet.Contains(track.FilePath)).ToList()
            : [];

        return new LibraryReconciliationResult
        {
            SourceId = sourceId,
            StoredTrackCount = storedTracks.Count,
            ScannedTrackCount = scannedPaths.Count,
            MissingTracks = missingTracks,
            ScanCompletedSuccessfully = scanCompletedSuccessfully
        };
    }
}
