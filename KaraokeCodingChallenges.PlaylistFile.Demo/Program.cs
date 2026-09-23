using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playlist;
using KaraokeCodingChallenges.PlaylistFile;

namespace KaraokeCodingChallenges.PlaylistFile.Demo
{
    internal class Program
    {
        static void Main()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Manual Test"
            };

            playlist.AddTrack(
                new MediaTrackModel(
                    "She's Kinda Hot",
                    "5 Seconds Of Summer",
                    @"D:\Music\5 Seconds Of Summer - She's Kinda Hot.mp3"
                )
            );

            playlist.AddTrack(
                new MediaTrackModel(
                    "Hello",
                    "Adele",
                    @"D:\Music\Adele - Hello.mp3"
                )
            );

            M3uPlaylistWriter writer = new();

            string playlistPath = @"D:\Playlists\Demo Playlist.m3u";

            writer.Write(playlistPath, playlist, useRelativePaths: false);

            M3uPlaylistReader reader = new();

            MediaPlaylist importedPlaylist = reader.Read(playlistPath);

            Console.WriteLine(importedPlaylist.Name);

            foreach (MediaTrackModel track in importedPlaylist.Tracks)
            {
                Console.WriteLine(track.FilePath);
            }
        }
    }
}
