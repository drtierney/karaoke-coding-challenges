using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibrary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaLibrary library = new();

            MediaTrackModel testTrack = new(
                "title",
                "artist",
                @"D:\Music\test.mp3",
                123);

            List<MediaTrackModel> sampleTracks = new()
            {
                testTrack,

                new(
                    "Bohemian Rhapsody",
                    "Queen",
                    @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                    354),

                new(
                    "Wonderwall",
                    "Oasis",
                    @"D:\Music\Oasis - Wonderwall.flac",
                    258),

                new(
                    "Don't Stop Believin'",
                    "Journey",
                    @"D:\Karaoke\Journey - Don't Stop Believin.mp3",
                    251,
                    true),

                new(
                    "Dancing Queen",
                    "ABBA",
                    @"D:\Karaoke\ABBA - Dancing Queen.mp3",
                    231,
                    true)
            };

            library.AddTracks(sampleTracks);

            Console.WriteLine($"Tracks in library: {library.Count}");

            bool removed = library.RemoveTrack(testTrack);
            Console.WriteLine($"Removed test track: {removed}");

            bool removedMissingTrack = library.RemoveTrack(@"D:\Music\Missing Track.mp3");

            Console.WriteLine($"Removed missing track: {removedMissingTrack}");
            Console.WriteLine($"Tracks in library: {library.Count}");

            MediaTrackModel? foundTrack =
                library.FindTrackByFilePath(
                    @"D:\Music\Oasis - Wonderwall.flac");

            Console.WriteLine(
                foundTrack != null
                    ? $"\nFound: {foundTrack.DisplayName}"
                    : "\nTrack not found");

            bool removedByPath =
                library.RemoveTrack(
                    @"D:\Music\Oasis - Wonderwall.flac");

            Console.WriteLine(
                $"Removed 'Oasis - Wonderwall' by file path: {removedByPath}");

            Console.WriteLine(
                $"Tracks in library: {library.Count}");

            Console.WriteLine("\nMedia Library Contents:");

            foreach (MediaTrackModel track in library.Tracks)
            {
                Console.WriteLine(track);
                Console.WriteLine();
            }

            library.Clear();

            Console.WriteLine($"Tracks after clearing library: {library.Count}");
        }
    }
}