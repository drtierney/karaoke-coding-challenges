using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.DuplicateDetection;

internal class ArtistTitleEqualityComparer : IEqualityComparer<MediaTrackModel>
{
    public bool Equals(MediaTrackModel? x, MediaTrackModel? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return string.Equals(x.Artist, y.Artist, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(MediaTrackModel track)
    {
        HashCode hash = new();

        hash.Add(track.Artist, StringComparer.OrdinalIgnoreCase);
        hash.Add(track.Title, StringComparer.OrdinalIgnoreCase);

        return hash.ToHashCode();
    }
}