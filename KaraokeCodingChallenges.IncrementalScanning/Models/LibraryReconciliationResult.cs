using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

public class LibraryReconciliationResult
{
    public long SourceId { get; init; }

    public int StoredTrackCount { get; init; }

    public int ScannedTrackCount { get; init; }

    public IReadOnlyList<MediaTrackRecord> MissingTracks { get; init; } = [];

    public IReadOnlyList<MediaTrackRecord> PresentTracks { get; init; } = [];

    public int MissingTrackCount => MissingTracks.Count;

    public bool ScanCompletedSuccessfully { get; init; }
}
