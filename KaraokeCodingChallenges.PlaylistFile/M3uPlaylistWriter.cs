using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playlist;

namespace KaraokeCodingChallenges.PlaylistFile
{
    public class M3uPlaylistWriter
    {
        public void Write(string filePath, MediaPlaylist playlist, bool useRelativePaths = false)
        {
            string playlistDirectory = Path.GetDirectoryName(filePath)!;

            List<string> lines = [];
            foreach (MediaTrackModel track in playlist.Tracks)
            {
                if (useRelativePaths)
                {
                    lines.Add(Path.GetRelativePath(playlistDirectory, track.FilePath));
                }
                else
                {
                    lines.Add(track.FilePath);
                }
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}
