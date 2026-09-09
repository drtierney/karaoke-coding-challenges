using KaraokeCodingChallenges.MetadataReader;
using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;

namespace KaraokeCodingChallenges.MediaMetadataScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] filePaths =
            [
                @"D:\KaraokeTest\1975 - The Sound.mp3",
                @"D:\KaraokeTest\01 - UB40 - Can't Help Falling In Love.mp3",
                @"D:\KaraokeTest\Foxes Body Talk.mp3",
                @"D:\KaraokeTest\Logic ft Alessia Cara & Khalid - 1-800-273-8255.mp3",
                @"D:\KaraokeTest\MissingFile.mp3"
            ];

            IMetadataReader metadataReader = new TagLibMetadataReader();

            foreach (string filePath in filePaths)
            {
                try
                {
                    Console.WriteLine(filePath);

                    MediaMetadataRecord metadata = MediaMetadataScanner.ReadMetadata(filePath, metadataReader);

                    DisplayMetadata(metadata);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine();
            }
        }

        private static void DisplayMetadata(MediaMetadataRecord metadata)
        {
            string fileSize = metadata.FileSizeBytes.HasValue
                ? $"{metadata.FileSizeBytes.Value / (1024.0 * 1024.0):F2} MB"
                : "N/A";

            Console.WriteLine($"File Path:        {metadata.FilePath}");
            Console.WriteLine($"File Name:        {metadata.FileName ?? "N/A"}");
            Console.WriteLine($"Extension:        {metadata.Extension ?? "N/A"}");
            Console.WriteLine($"File Size:        {fileSize}");
            Console.WriteLine($"Artist:           {metadata.Artist ?? "N/A"}");
            Console.WriteLine($"Title:            {metadata.Title ?? "N/A"}");
            Console.WriteLine($"Album:            {metadata.Album ?? "N/A"}");
            Console.WriteLine($"Genre:            {metadata.Genre ?? "N/A"}");
            Console.WriteLine($"Year:             {metadata.Year?.ToString() ?? "N/A"}");
            Console.WriteLine($"Track Number:     {metadata.TrackNumber?.ToString() ?? "N/A"}");
            Console.WriteLine($"Duration Seconds: {metadata.DurationSeconds?.ToString() ?? "N/A"}");
            Console.WriteLine($"Bitrate:          {metadata.Bitrate?.ToString() ?? "N/A"}");
            Console.WriteLine($"Sample Rate:      {metadata.SampleRate?.ToString() ?? "N/A"}");
            Console.WriteLine($"Channels:         {metadata.Channels?.ToString() ?? "N/A"}");
        }
    }
}