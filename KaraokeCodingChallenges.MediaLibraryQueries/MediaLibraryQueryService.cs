using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibraryQueries;

public static class MediaLibraryQueryService
{
    public static IEnumerable<MediaTrackModel> FindTracksByArtist(
        IEnumerable<MediaTrackModel> tracks,
        string artist)
    {
        return tracks.Where(track =>
            track.Artist.Equals(
                artist,
                StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<MediaTrackModel> FindTracksByTitle(
        IEnumerable<MediaTrackModel> tracks,
        string title)
    {
        return tracks.Where(track => 
            track.Title.Contains(
                title, 
                StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<MediaTrackModel> FindTracksByFilePath(
        IEnumerable<MediaTrackModel> tracks,
        string searchString)
    {
        return tracks.Where(track => 
            track.FilePath.Contains(
                searchString,
                StringComparison.OrdinalIgnoreCase));
    }
}