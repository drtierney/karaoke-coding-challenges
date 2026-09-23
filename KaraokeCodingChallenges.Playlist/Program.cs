using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.Playlist
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaTrackModel track1 = new(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                354);

            MediaTrackModel track2 = new(
                "The Only Exception",
                "Paramore",
                @"D:\Music\Paramore - The Only Exception.mp3",
                267);

            MediaTrackModel track3 = new(
                "Sign of the Times",
                "Harry Styles",
                @"D:\Music\Harry Styles - Sign of the Times.mp3",
                340);

            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            playlist.AddTrack(track1);
            playlist.AddTrack(track2);
            playlist.AddTrack(track3);

            
            DisplayPlaylistInfo(playlist);
            DisplayPlaylistTrackList(playlist);

            bool removed = playlist.RemoveTrack(track2);

            Console.WriteLine($"Track removed: {removed}");
            Console.WriteLine();

            DisplayPlaylistInfo(playlist);
            DisplayPlaylistTrackList(playlist);

            bool removedAgain = playlist.RemoveTrack(track2);

            Console.WriteLine($"Track removed again: {removedAgain}");
            Console.WriteLine();

            playlist.Clear();

            DisplayPlaylistInfo(playlist);
            DisplayPlaylistTrackList(playlist);
        }

        private static void DisplayPlaylistInfo(MediaPlaylist playlist)
        {
            Console.WriteLine($"Playlist: {playlist.Name}");
            Console.WriteLine($"Id: {playlist.Id}");
            Console.WriteLine($"Track count: {playlist.Count}");
            Console.WriteLine();
        }

        private static void DisplayPlaylistTrackList(MediaPlaylist playlist)
        {
            foreach (var track in playlist.Tracks)
            {
                Console.WriteLine(track.DisplayName);
            }
            Console.WriteLine();
        }

    }

}