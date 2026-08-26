using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.TrackSorting
{
    public static class MediaLibrarySortService
    {
        public static IEnumerable<MediaTrackModel> SortByArtistThenTitle(IEnumerable<MediaTrackModel> tracks)
        {
            //return tracks.OrderBy(track => track.Artist).ThenBy(track => track.Title);
            return tracks.OrderBy(track => track, new ArtistTitleComparer());
        }

        public static IEnumerable<MediaTrackModel> SortByTitle(IEnumerable<MediaTrackModel> tracks)
        {
            return from track in tracks
                   orderby track.Title
                   select track;
        }

        public static IEnumerable<MediaTrackModel> SortByDurationAscending(IEnumerable<MediaTrackModel> tracks)
        {
            return tracks.OrderBy(track => track.DurationSeconds);
        }

        public static IEnumerable<MediaTrackModel> SortByDurationDescending(IEnumerable<MediaTrackModel> tracks)
        {
            return tracks.OrderByDescending(track => track.DurationSeconds);
        }
    }
}
