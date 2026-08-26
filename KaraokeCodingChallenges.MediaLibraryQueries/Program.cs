using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibraryQueries
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<MediaTrackModel> sampleTracks =
            [
                new MediaTrackModel(
                    "Bohemian Rhapsody",
                    "Queen",
                    @"D:\Music\Queen - Bohemian Rhapsody.MP3",
                    354,
                    false),

                new MediaTrackModel(
                    "Don't Stop Me Now",
                    "Queen",
                    @"D:\Karaoke\Queen - Don't Stop Me Now.mp3",
                    209,
                    true),

                new MediaTrackModel(
                    "Dancing Queen",
                    "ABBA",
                    @"D:\Karaoke\ABBA - Dancing Queen.Mp3",
                    231,
                    true),

                new MediaTrackModel(
                    "Wonderwall",
                    "Oasis",
                    @"D:\Music\Oasis - Wonderwall.Flac",
                    259,
                    false),

                new MediaTrackModel(
                    "Don't Stop Believin'",
                    "Journey",
                    @"D:\Karaoke\Journey - Don't Stop Believin.mp3",
                    251,
                    true),

                new MediaTrackModel(
                    "Africa",
                    "Toto",
                    @"D:\Music\Toto - Africa.opus",
                    295,
                    false),

                new MediaTrackModel(
                    "Mamma Mia",
                    "ABBA",
                    @"D:\Music\ABBA - Mamma Mia.flac",
                    213,
                    false),

                new MediaTrackModel(
                    "Take On Me",
                    "a-ha",
                    @"D:\Music\a-ha - Take On Me.mp3",
                    225,
                    false)
            ];

            Console.WriteLine("All Tracks:");
            PrintDisplayNames(sampleTracks);

            var abbaTracks =
                MediaLibraryQueries.FindTracksByArtist(sampleTracks, "Abba");

            Console.WriteLine("ABBA Tracks:");
            PrintAllDetails(abbaTracks);

            List<string> artistSearches =
            [
                "Queen",
                "queen",
                "QUEEN",
                "Oasis",
                "Unknown Artist"
            ];

            foreach (string artist in artistSearches)
            {
                Console.WriteLine($"Search results for: {artist}");

                var tracks =
                    MediaLibraryQueries.FindTracksByArtist(
                        sampleTracks,
                        artist);

                PrintDisplayNames(tracks);
            }

            var titleContainingStop =
                MediaLibraryQueries.FindTracksByTitle(
                    sampleTracks,
                    "stop");

            Console.WriteLine("Tracks containing 'stop' in the title:");
            PrintDisplayNames(titleContainingStop);

            var missingTitle =
                MediaLibraryQueries.FindTracksByTitle(
                    sampleTracks,
                    "Yesterday");

            Console.WriteLine("Tracks containing 'Yesterday' in the title:");
            PrintDisplayNames(missingTitle);

            var filePathContainingTake =
                MediaLibraryQueries.FindTracksByFilePath(
                    sampleTracks,
                    "take");

            Console.WriteLine("Tracks containing 'take' in the file path:");
            PrintFilePaths(filePathContainingTake);

            string filePathSearch = @"D:\Music\a";

            var filePathMatches =
                MediaLibraryQueries.FindTracksByFilePath(
                    sampleTracks,
                    filePathSearch);

            Console.WriteLine(
                $"Tracks containing '{filePathSearch}' in the file path:");

            PrintFilePaths(filePathMatches);
        }

        public static bool HasTracksToPrint(
            IEnumerable<MediaTrackModel> tracks)
        {
            if (!tracks.Any())
            {
                Console.WriteLine("No tracks found.");
                Console.WriteLine();
                return false;
            }

            return true;
        }

        public static void PrintAllDetails(
            IEnumerable<MediaTrackModel> tracks)
        {
            if (!HasTracksToPrint(tracks))
            {
                return;
            }

            foreach (var track in tracks)
            {
                Console.WriteLine(track);
                Console.WriteLine();
            }
        }

        public static void PrintDisplayNames(
            IEnumerable<MediaTrackModel> tracks)
        {
            if (!HasTracksToPrint(tracks))
            {
                return;
            }

            foreach (var track in tracks)
            {
                Console.WriteLine(track.DisplayName);
            }

            Console.WriteLine();
        }

        public static void PrintFilePaths(
            IEnumerable<MediaTrackModel> tracks)
        {
            if (!HasTracksToPrint(tracks))
            {
                return;
            }

            foreach (var track in tracks)
            {
                Console.WriteLine(track.FilePath);
            }

            Console.WriteLine();
        }
    }
}