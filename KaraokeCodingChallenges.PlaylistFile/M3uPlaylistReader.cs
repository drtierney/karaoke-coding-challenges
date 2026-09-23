using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playlist;

namespace KaraokeCodingChallenges.PlaylistFile
{
    public class M3uPlaylistReader
    {
        public MediaPlaylist Read(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            MediaPlaylist playlist = new()
            {
                Name = Path.GetFileNameWithoutExtension(filePath)
            };

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string trackPath = line;

                if (!Path.IsPathRooted(trackPath))
                {
                    string playlistDirectory = Path.GetDirectoryName(filePath)!;

                    trackPath = Path.GetFullPath(Path.Combine(playlistDirectory, trackPath));
                }

                string title = Path.GetFileNameWithoutExtension(trackPath);

                MediaTrackModel track = new(
                    title,
                    string.Empty,
                    trackPath
                );

                playlist.AddTrack(track);
            }

            return playlist;
        }
    }
}