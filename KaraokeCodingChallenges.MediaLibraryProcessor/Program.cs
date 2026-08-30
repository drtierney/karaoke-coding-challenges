using KaraokeCodingChallenges.DuplicateDetection;
using KaraokeCodingChallenges.MediaLibraryFiltering;
using KaraokeCodingChallenges.TrackSorting;
using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibraryProcessor;

internal class Program
{
    static void Main(string[] args)
    {
        List<MediaTrackModel> originalTracks =
        [
            new MediaTrackModel(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                354,
                false),

            new MediaTrackModel(
                "Bohemian Rhapsody",
                "Queen",
                @"E:\Backup\Queen - Bohemian Rhapsody.mp3",
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
                @"D:\Karaoke\ABBA - Dancing Queen.mp3",
                231,
                true),

            new MediaTrackModel(
                "Mamma Mia",
                "ABBA",
                @"D:\Music\ABBA - Mamma Mia.flac",
                213,
                false),

            new MediaTrackModel(
                "Take On Me",
                "a-ha",
                @"D:\Karaoke\a-ha - Take On Me.mp3",
                225,
                true),

            new MediaTrackModel(
                "Africa",
                "Toto",
                @"D:\Music\Toto - Africa.mp3",
                295,
                false),

            new MediaTrackModel(
                "DANCING QUEEN",
                "ABBA",
                @"E:\Backup\ABBA - Dancing Queen.mp3",
                231,
                true)
        ];

        PrintTracks("Original Tracks:", originalTracks);

        // Step 1: Find duplicate tracks.
        IEnumerable<MediaTrackModel> duplicateTracks = DuplicateDetectionService.FindDuplicateTracks(originalTracks);
        PrintTracks("Duplicate Tracks:", duplicateTracks);

        // Step 2: Create a collection containing only distinct tracks.
        IEnumerable<MediaTrackModel> distinctTracks = DuplicateDetectionService.FindDistinctTracks(originalTracks);
        PrintTracks("Distinct Tracks:", distinctTracks);

        // Step 3: Filter the distinct collection to karaoke tracks.
        IEnumerable<MediaTrackModel> karaokeTracks = MediaLibraryFilterService.FilterKaraokeTracks(distinctTracks);
        PrintTracks("Karaoke Tracks:", karaokeTracks);

        // Step 4: Sort the karaoke tracks by artist, then title.
        IEnumerable<MediaTrackModel> sortedKaraokeTracks = MediaLibrarySortService.SortByArtistThenTitle(karaokeTracks);

        // Step 5: Display the final tracks.
        PrintTracks("Sorted Karaoke Tracks:", sortedKaraokeTracks);

        // Step 6: Display the media library summary.
        PrintMediaLibrarySummary(originalTracks, duplicateTracks, distinctTracks, karaokeTracks);
    }
    
    static void PrintTracks(string heading, IEnumerable<MediaTrackModel> tracks)
    {
        Console.WriteLine(heading);

        foreach (MediaTrackModel track in tracks)
        {
            Console.WriteLine($"{track.DisplayName} --> {track.FilePath}");
        }

        Console.WriteLine();
    }

    static void PrintMediaLibrarySummary(
        IEnumerable<MediaTrackModel> originalTracks,
        IEnumerable<MediaTrackModel> duplicateTracks,
        IEnumerable<MediaTrackModel> distinctTracks,
        IEnumerable<MediaTrackModel> karaokeTracks)
    {
        Console.WriteLine("Media Library Summary");
        Console.WriteLine("---------------------");
        Console.WriteLine($"Original tracks: {originalTracks.Count()}");
        Console.WriteLine($"Duplicate tracks: {duplicateTracks.Count()}");
        Console.WriteLine($"Distinct tracks: {distinctTracks.Count()}");
        Console.WriteLine($"Karaoke tracks: {karaokeTracks.Count()}");
    }
}