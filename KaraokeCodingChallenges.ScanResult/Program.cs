using KaraokeCodingChallenges.MediaMetadata;

namespace KaraokeCodingChallenges.ScanResult
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaScanResult successfulScan = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Metadata = new MediaMetadataRecord
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
                },
                IsSuccess = true
            };

            MediaScanResult warningScan = new()
            {
                FilePath = @"D:\Music\Unknown Artist - Mystery Song.mp3",
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Unknown Artist - Mystery Song.mp3",
                    Title = "Mystery Song",
                    Artist = "Unknown Artist"
                },
                IsSuccess = true,
                Warnings =
                [
                    "Album metadata was not available.",
                    "Track number could not be determined."
                ]
            };

            MediaScanResult failedScan = new()
            {
                FilePath = @"D:\Music\BrokenFile.mp3",
                Metadata = null,
                IsSuccess = false,
                Errors =
                [
                    "The file could not be read."
                ]
            };

            PrintScanResult(successfulScan);
            PrintScanResult(warningScan);
            PrintScanResult(failedScan);
        }
        static void PrintScanResult(MediaScanResult result)
        {
            Console.WriteLine($"File: {result.FilePath}");
            Console.WriteLine($"Success: {result.IsSuccess}");

            if (result.Metadata is not null)
            {
                Console.WriteLine($"Title: {result.Metadata.Title}");
                Console.WriteLine($"Artist: {result.Metadata.Artist}");
                Console.WriteLine($"Album: {result.Metadata.Album}");
            }

            if (result.Warnings.Count > 0)
            {
                Console.WriteLine("Warnings:");

                foreach (string warning in result.Warnings)
                {
                    Console.WriteLine($"- {warning}");
                }
            }

            if (result.Errors.Count > 0)
            {
                Console.WriteLine("Errors:");

                foreach (string error in result.Errors)
                {
                    Console.WriteLine($"- {error}");
                }
            }

            Console.WriteLine();
        }
    }
}