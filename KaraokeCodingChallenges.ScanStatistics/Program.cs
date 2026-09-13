using ScanResultRecord = KaraokeCodingChallenges.ScanResult.ScanResult;
using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;
using KaraokeFilePairingService = KaraokeCodingChallenges.KaraokeFilePairing.KaraokeFilePairing;

namespace KaraokeCodingChallenges.ScanStatistics
{
    internal class Program
    {
        static void Main()
        {
            List<ScanResultRecord> scanResults =
            [
                new ScanResultRecord
                {
                    FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                    IsSuccess = true,
                    Metadata = new MediaMetadataRecord
                    {
                        FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                        Artist = "Queen",
                        Title = "Bohemian Rhapsody",
                        Album = "A Night at the Opera",
                        Genre = "Rock",
                        TrackNumber = 11
                    }
                },

                new ScanResultRecord
                {
                    FilePath = @"D:\Music\Unknown Artist - Song.opus",
                    IsSuccess = true,
                    Metadata = new MediaMetadataRecord
                    {
                        FilePath = @"D:\Music\Unknown Artist - Song.opus",
                        Artist = null,
                        Title = "Song",
                        Album = "Unknown Album",
                        Genre = "Pop",
                        TrackNumber = 2
                    },
                    Warnings =
                    [
                        "Artist metadata was missing."
                    ]
                },

                new ScanResultRecord
                {
                    FilePath = @"D:\Music\ABBA - Dancing Queen.flac",
                    IsSuccess = true,
                    Metadata = new MediaMetadataRecord
                    {
                        FilePath = @"D:\Music\ABBA - Dancing Queen.flac",
                        Artist = "ABBA",
                        Title = "Dancing Queen",
                        Album = null,
                        Genre = null,
                        TrackNumber = null
                    },
                    Warnings =
                    [
                        "Album metadata was missing.",
                        "Genre metadata was missing.",
                        "Track number metadata was missing."
                    ]
                },

                new ScanResultRecord
                {
                    FilePath = @"D:\Music\Missing.mp3",
                    IsSuccess = false,
                    Metadata = null,
                    Errors =
                    [
                        "File was not found."
                    ]
                }
            ];

            List<string> karaokeFiles =
            [
                @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
                @"D:\Karaoke\Queen - Bohemian Rhapsody.cdg",

                @"D:\Karaoke\ABBA - Dancing Queen.mp3",
                @"D:\Karaoke\ABBA - Dancing Queen.cdg",

                @"D:\Karaoke\Missing Graphics.mp3",

                @"D:\Karaoke\Missing Audio.cdg"
            ];

            KaraokeFilePairingService karaokeFilePairing = new();
            karaokeFilePairing.PairFiles(karaokeFiles);

            ScanStatistics statistics = ScanStatisticsService.GenerateStatistics(scanResults, karaokeFilePairing);
            DisplayStatistics(statistics);
        }

        private static void DisplayStatistics(ScanStatistics statistics)
        {
            Console.WriteLine("Scan Summary");
            Console.WriteLine($"Total Scans: {statistics.TotalScans}");
            Console.WriteLine($"Successful Scans: {statistics.SuccessfulScans}");
            Console.WriteLine($"Failed Scans: {statistics.FailedScans}");
            Console.WriteLine($"Success Percentage: {statistics.SuccessPercentage:F2}%");
            Console.WriteLine($"Failure Percentage: {statistics.FailurePercentage:F2}%");
            Console.WriteLine($"Total Warnings: {statistics.TotalWarnings}");

            Console.WriteLine();
            Console.WriteLine("Files By Extension");

            foreach (var kvp in statistics.FilesByExtension.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            Console.WriteLine();
            Console.WriteLine("Metadata Quality");
            Console.WriteLine($"Missing Artist: {statistics.MissingArtistCount}");
            Console.WriteLine($"Missing Title: {statistics.MissingTitleCount}");
            Console.WriteLine($"Missing Album: {statistics.MissingAlbumCount}");
            Console.WriteLine($"Missing Genre: {statistics.MissingGenreCount}");
            Console.WriteLine($"Missing Track Number: {statistics.MissingTrackNumberCount}");
            Console.WriteLine($"Complete Metadata Percentage: {statistics.CompleteMetadataPercentage:F2}%");

            Console.WriteLine();
            Console.WriteLine("Karaoke Pairing");
            Console.WriteLine($"Matched Pairs: {statistics.MatchedKaraokePairs}");
            Console.WriteLine($"Unmatched MP3 Files: {statistics.UnmatchedMp3Files}");
            Console.WriteLine($"Unmatched CDG Files: {statistics.UnmatchedCdgFiles}");
        }
    }
}