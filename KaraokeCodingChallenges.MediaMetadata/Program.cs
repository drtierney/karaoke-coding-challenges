namespace KaraokeCodingChallenges.MediaMetadata
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaMetadata queenTrack = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                Album = "A Night at the Opera",
                Genre = "Rock",
                Year = 1975,
                TrackNumber = 11,
                DurationSeconds = 354,
                Bitrate = 320,
                SampleRate = 44100,
                Channels = 2
            };

            MediaMetadata queenTrackCopy = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                Album = "A Night at the Opera",
                Genre = "Rock",
                Year = 1975,
                TrackNumber = 11,
                DurationSeconds = 354,
                Bitrate = 320,
                SampleRate = 44100,
                Channels = 2
            };

            MediaMetadata incompleteTrack = new()
            {
                FilePath = @"D:\Music\untagged.opus",
                DurationSeconds = null,
                Bitrate = null,
                SampleRate = 48000,
                Channels = 2
            };

            Console.WriteLine("Record equality:");
            Console.WriteLine(queenTrack == queenTrackCopy);

            MediaMetadata correctedTrack = queenTrack with
            {
                Genre = "Classic Rock"
            };

            Console.WriteLine();
            Console.WriteLine("Changed copy:");
            Console.WriteLine($"Original genre: {queenTrack.Genre}");
            Console.WriteLine($"Corrected genre: {correctedTrack.Genre}");
            Console.WriteLine($"Records equal after change: {queenTrack == correctedTrack}");
            Console.WriteLine();
            Console.WriteLine("Nullable metadata:");
            Console.WriteLine($"Title: {incompleteTrack.Title ?? "Unknown"}");
            Console.WriteLine($"Duration: {incompleteTrack.DurationSeconds?.ToString() ?? "Unknown"}");
            Console.WriteLine($"Bitrate: {incompleteTrack.Bitrate?.ToString() ?? "Unknown"}");
            Console.WriteLine();
            Console.WriteLine("Record output:");
            Console.WriteLine(queenTrack);
        }
    }
}