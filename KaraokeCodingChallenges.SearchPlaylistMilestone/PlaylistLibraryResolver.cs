using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playlist;

namespace KaraokeCodingChallenges.SearchPlaylistMilestone;

public class PlaylistLibraryResolver
{
    public MediaPlaylist Resolve(
        MediaPlaylist playlist,
        MediaLibraryCollection library)
    {
        MediaPlaylist resolvedPlaylist = new()
        {
            Name = playlist.Name
        };

        foreach (MediaTrackModel track in playlist.Tracks)
        {
            MediaTrackModel? libraryTrack = library.FindTrackByFilePath(track.FilePath);

            resolvedPlaylist.AddTrack(libraryTrack ?? track);
        }

        return resolvedPlaylist;
    }
}
