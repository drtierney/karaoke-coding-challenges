using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibraryFiltering
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


            PrintTracks("All Tracks:", sampleTracks);
            PrintTracks("Karaoke Tracks:", MediaLibraryFilterService.FilterKaraokeTracks(sampleTracks));
            PrintTracks("Music Tracks:", MediaLibraryFilterService.FilterMusicTracks(sampleTracks));
            PrintTracks("Tracks with Minimum Duration of 240 seconds:", MediaLibraryFilterService.FilterByMinimumDuration(sampleTracks, 240));
            PrintTracks("Tracks with Maximum Duration of 240 seconds:", MediaLibraryFilterService.FilterByMaximumDuration(sampleTracks, 240));

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
