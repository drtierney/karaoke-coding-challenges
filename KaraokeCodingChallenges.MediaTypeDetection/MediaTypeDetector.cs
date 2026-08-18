namespace KaraokeCodingChallenges.MediaTypeDetection
{
    public class MediaTypeDetector
    {
        public static MediaFileType GetMediaType(string filePath)
        {

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            // Modern switch expression - replaces switch/case style
            return extension switch
            {
                ".mp3" or ".flac" or ".opus" => MediaFileType.Audio,
                ".cdg" => MediaFileType.KaraokeGraphics,
                _ => MediaFileType.Unsupported
            };
        }
    }
}
