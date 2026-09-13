using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.DuplicateDetection;

internal class Program
{
    static void Main(string[] args)
    {
        List<MediaTrackModel> sampleTracks =
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
                "BOHEMIAN RHAPSODY",
                "QUEEN",
                @"D:\Music\TRACK1.MP3",
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
                @"D:\Music\ABBA - Dancing Queen.mp3",
                231,
                false),

            new MediaTrackModel(
                "dancing queen",
                "abba",
                @"E:\Backup\ABBA - Dancing Queen.mp3",
                231,
                false),

            new MediaTrackModel(
                "Wonderwall",
                "Oasis",
                @"D:\Music\Oasis - Wonderwall.mp3",
                258,
                false),

            new MediaTrackModel(
                "Africa",
                "Toto",
                @"D:\Music\Toto - Africa.mp3",
                295,
                false)
        ];

        IEnumerable<MediaTrackModel> distinctTracks = DuplicateDetectionService.FindDistinctTracks(sampleTracks);

        IEnumerable<MediaTrackModel> duplicateTracks = DuplicateDetectionService.FindDuplicateTracks(sampleTracks);

        PrintTracks("Sample Tracks:", sampleTracks);
        PrintTracks("Distinct Tracks:", distinctTracks);
        PrintTracks("Duplicate Tracks:", duplicateTracks);
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
}