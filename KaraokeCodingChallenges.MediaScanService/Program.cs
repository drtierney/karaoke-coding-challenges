using KaraokeCodingChallenges.MetadataReader;
using MediaScanServiceClass = KaraokeCodingChallenges.MediaScanService.MediaScanService;
using ScanResultRecord = KaraokeCodingChallenges.ScanResult.ScanResult;

namespace KaraokeCodingChallenges.MediaScanService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IMetadataReader metadataReader = new TagLibMetadataReader();

            string[] filePaths =
            [
            @"D:\KaraokeTest\01 - UB40 - Can't Help Falling In Love.mp3",
            @"D:\KaraokeTest\1975 - The Sound.mp3",
            @"D:\KaraokeTest\MissingFile.mp3",
            @"D:\KaraokeTest\Foxes Body Talk.mp3",
            @"D:\KaraokeTest\Logic ft Alessia Cara & Khalid - 1-800-273-8255.mp3"
            ];

            IEnumerable<ScanResultRecord> results =
                MediaScanServiceClass.ScanFiles(filePaths, metadataReader);

            foreach (ScanResultRecord scanResult in results)
            {
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
    }
}