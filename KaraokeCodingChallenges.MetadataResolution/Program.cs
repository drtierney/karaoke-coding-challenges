using KaraokeCodingChallenges.MediaMetadata;

namespace KaraokeCodingChallenges.MetadataResolution
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaMetadataRecord embeddedMetadata = new()
            {
                FilePath = @"D:\Music\01 - Queen - Bohemian Rhapsody.mp3",
                Title = " ",
                Artist = "Queen",
                Album = "A Night at the Opera",
                Genre = "Rock",
                Year = 1975,
                TrackNumber = null,
                DurationSeconds = 354,
                Bitrate = 320,
                SampleRate = 44100,
                Channels = 2
            };

            MediaMetadataRecord filenameMetadata = new()
            {
                FilePath = @"D:\Music\01 - Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                TrackNumber = 1
            };

            Console.WriteLine("Embedded Metadata");
            Console.WriteLine("-----------------");
            DisplayMetadata(embeddedMetadata);
            Console.WriteLine();

            Console.WriteLine("Filename Metadata");
            Console.WriteLine("-----------------");
            DisplayMetadata(filenameMetadata);
            Console.WriteLine();

            MediaMetadataRecord resolvedMetadata = MetadataResolutionService.ResolveMetadata(embeddedMetadata, filenameMetadata);

            Console.WriteLine("Resolved Metadata");
            Console.WriteLine("-----------------");
            DisplayMetadata(resolvedMetadata);
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