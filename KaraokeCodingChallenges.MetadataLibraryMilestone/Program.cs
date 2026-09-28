using KaraokeCodingChallenges.LibraryScanner;
using KaraokeCodingChallenges.MediaScanService;
using KaraokeCodingChallenges.MetadataReader;
using KaraokeCodingChallenges.ScanResult;
using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.ScanStatistics;
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.LibrarySourceRules;

namespace KaraokeCodingChallenges.MetadataLibraryMilestone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Metadata Library Milestone");

            AppConfiguration config;

            try
            {
                config = ConfigurationLoader.Load("appsettings.json");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Configuration file not found.");
                return;
            }
            catch (System.Text.Json.JsonException)
            {
                Console.WriteLine("Configuration file contains invalid JSON.");
                return;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            IReadOnlyCollection<string> errors = ConfigurationValidator.Validate(config);

            if (errors.Count > 0)
            {
                Console.WriteLine("Configuration errors:");

                foreach (string error in errors)
                {
                    Console.WriteLine($"- {error}");
                }

                return;
            }

            Console.WriteLine("Configuration loaded.");
            Console.WriteLine();

            LibraryScannerService scanner = new();

            List<string> files = [];
            List<string> karaokeFiles = [];

            foreach (LibrarySource source in config.LibrarySources)
            {
                if (!source.Enabled)
                {
                    Console.WriteLine($"Skipping disabled {source.Type} source: {source.Path}");
                    continue;
                }

                if (!Directory.Exists(source.Path))
                {
                    Console.WriteLine($"Library source unavailable: {source.Type} - {source.Path}");
                    continue;
                }

                Console.WriteLine($"Scanning {source.Type} source: {source.Path}");

                List<string> sourceFiles = scanner.Scan(source.Path);

                //foreach (string file in sourceFiles)
                //{
                //    Console.WriteLine($"Found file: {file}");
                //}

                karaokeFiles.AddRange(LibrarySourceRulesService.GetKaraokeFiles(source.Type, sourceFiles));

                files.AddRange(sourceFiles);
                Console.WriteLine($"Total files found in {source.Type} source: {sourceFiles.Count}");
                Console.WriteLine();
            }

            if (files.Count == 0)
            {
                Console.WriteLine("No supported media files found.");
                return;
            }

            HashSet<string> metadataExtensions = new(StringComparer.OrdinalIgnoreCase)
            {
                ".mp3",
                ".flac",
                ".opus"
            };

            List<string> metadataFiles = files
                .Where(file => metadataExtensions.Contains(Path.GetExtension(file)))
                .ToList();

            Console.WriteLine($"Files found: {files.Count}");
            Console.WriteLine($"Metadata files: {metadataFiles.Count}");

            TagLibMetadataReader metadataReader = new();

            List<MediaScanResult> results = MediaScannerService.ScanFiles(metadataFiles, metadataReader).ToList();

            DisplayScanResults(results);

            KaraokeFilePairingService karaokeFilePairing = new();
            karaokeFilePairing.PairFiles(karaokeFiles);

            ScanStatisticsSummary statistics = ScanStatisticsService.GenerateStatistics(results, karaokeFilePairing);

            DisplayStatistics(statistics);
        }

        private static void DisplayScanResults(IEnumerable<MediaScanResult> results)
        {
            foreach (MediaScanResult scanResult in results)
            {
                if (scanResult.Warnings.Count == 0 && scanResult.Errors.Count == 0)
                {
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine($"FilePath: {scanResult.FilePath}");
                Console.WriteLine($"Success: {scanResult.IsSuccess}");

                foreach (string warning in scanResult.Warnings)
                {
                    Console.WriteLine($"Warning: {warning}");
                }

                foreach (string error in scanResult.Errors)
                {
                    Console.WriteLine($"Error: {error}");
                }
            }
        }

        private static void DisplayStatistics(ScanStatisticsSummary statistics)
        {
            Console.WriteLine();
            Console.WriteLine("Scan Statistics");
            Console.WriteLine($"Total scans: {statistics.TotalScans}");
            Console.WriteLine($"Successful scans: {statistics.SuccessfulScans}");
            Console.WriteLine($"Failed scans: {statistics.FailedScans}");
            Console.WriteLine($"Total warnings: {statistics.TotalWarnings}");
            Console.WriteLine($"Success percentage: {statistics.SuccessPercentage:F1}%");
            Console.WriteLine($"Failure percentage: {statistics.FailurePercentage:F1}%");
            Console.WriteLine($"Files with complete metadata: {statistics.CompleteMetadataCount}");
            Console.WriteLine($"Complete metadata percentage: {statistics.CompleteMetadataPercentage:F1}%");
            Console.WriteLine();
            Console.WriteLine("Metadata Files by Extension");
            foreach (KeyValuePair<string, int> extension in statistics.FilesByExtension)
            {
                Console.WriteLine($"{extension.Key}: {extension.Value}");
            }
            Console.WriteLine();
            Console.WriteLine("Missing Metadata");
            Console.WriteLine($"Missing artist: {statistics.MissingArtistCount}");
            Console.WriteLine($"Missing title: {statistics.MissingTitleCount}");
            Console.WriteLine($"Missing album: {statistics.MissingAlbumCount}");
            Console.WriteLine($"Missing genre: {statistics.MissingGenreCount}");
            Console.WriteLine($"Missing track number: {statistics.MissingTrackNumberCount}");
            Console.WriteLine();
            Console.WriteLine("Karaoke Pairing");
            Console.WriteLine($"Matched karaoke pairs: {statistics.MatchedKaraokePairs}");
            Console.WriteLine($"Unmatched MP3 files: {statistics.UnmatchedMp3Files}");
            Console.WriteLine($"Unmatched CDG files: {statistics.UnmatchedCdgFiles}");
        }
    }
}