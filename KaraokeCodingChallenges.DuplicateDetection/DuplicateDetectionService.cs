using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.DuplicateDetection;

public static class DuplicateDetectionService
{
    public static IEnumerable<MediaTrackModel> FindDuplicateTracks(IEnumerable<MediaTrackModel> tracks)
    {
        HashSet<MediaTrackModel> uniqueTracks = new(new ArtistTitleEqualityComparer());

        List<MediaTrackModel> duplicates = [];

        foreach (MediaTrackModel track in tracks)
        {
            if (!uniqueTracks.Add(track))
            {
                duplicates.Add(track);
            }
        }

        return duplicates;
    }

    public static IEnumerable<MediaTrackModel> FindDistinctTracks(IEnumerable<MediaTrackModel> tracks)
    {
        return new HashSet<MediaTrackModel>(
            tracks,
            new ArtistTitleEqualityComparer());
    }
}