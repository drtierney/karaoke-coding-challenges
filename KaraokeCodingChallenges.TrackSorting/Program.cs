using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.TrackSorting
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

            PrintTracks("Tracks Unsorted:", sampleTracks);

            IEnumerable<MediaTrackModel> sortedTracks = MediaLibrarySortService.SortByArtistThenTitle(sampleTracks);
            PrintTracks("Tracks Sorted by Artist Then Title:", sortedTracks);

            IEnumerable<MediaTrackModel> sortedByTitleTracks = MediaLibrarySortService.SortByTitle(sampleTracks);
            PrintTracks("Tracks Sorted By Title:", sortedByTitleTracks);

            IEnumerable<MediaTrackModel> sortedByDurationAscTracks = MediaLibrarySortService.SortByDurationAscending(sampleTracks);
            PrintTracks("Tracks Sorted By Shortest Duration:", sortedByDurationAscTracks);

            IEnumerable<MediaTrackModel> sortedByDurationDescTracks = MediaLibrarySortService.SortByDurationDescending(sampleTracks);
            PrintTracks("Tracks Sorted By Longest Duration:", sortedByDurationDescTracks);

        }
        static void PrintTracks(string heading, IEnumerable<MediaTrackModel> tracks)
        {
            Console.WriteLine(heading);

            foreach (MediaTrackModel track in tracks)
            {
                Console.WriteLine(track.DisplayName);
            }

            Console.WriteLine();
        }
    }
}
