using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.TrackSorting
{
    public sealed class ArtistTitleComparer : IComparer<MediaTrackModel>
    {
        public int Compare(MediaTrackModel? x, MediaTrackModel? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            int artistComparison = string.Compare(
                x.Artist,
                y.Artist,
                StringComparison.OrdinalIgnoreCase);

            if (artistComparison != 0)
            {
                return artistComparison;
            }

            return string.Compare(
                x.Title,
                y.Title,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}