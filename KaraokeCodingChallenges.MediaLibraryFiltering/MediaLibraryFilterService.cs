using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibraryFiltering
{
    public static class MediaLibraryFilterService
    {
        public static IEnumerable<MediaTrackModel> FilterKaraokeTracks(IEnumerable<MediaTrackModel> tracks)
        {
            return from track in tracks
                   where track.IsKaraoke
                   select track;
        }

        public static IEnumerable<MediaTrackModel> FilterMusicTracks(IEnumerable<MediaTrackModel> tracks)
        {
            return from track in tracks
                   where !track.IsKaraoke
                   select track;
        }

        public static IEnumerable<MediaTrackModel> FilterByMinimumDuration(IEnumerable<MediaTrackModel> tracks, int minimumDurationSeconds)
        {
            return from track in tracks
                   where track.DurationSeconds >= minimumDurationSeconds
                   select track;
        }

        public static IEnumerable<MediaTrackModel> FilterByMaximumDuration(IEnumerable<MediaTrackModel> tracks, int maximumDurationSeconds)
        {
            return from track in tracks
                   where track.DurationSeconds <= maximumDurationSeconds
                   select track;
        }
    }
}
