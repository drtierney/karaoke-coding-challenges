using KaraokeCodingChallenges.MediaTypeDetection;

namespace KaraokeCodingChallenges.FileValidation
{
    public class MediaFileValidator
    {
        public static FileValidationStatus Validate(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return FileValidationStatus.InvalidPath;
            }

            MediaFileType fileType = MediaTypeDetector.GetMediaType(filePath);

            if (fileType == MediaFileType.Unsupported)
            {
                return FileValidationStatus.UnsupportedFileType;
            }

            if (!File.Exists(filePath))
            {
                return FileValidationStatus.FileNotFound;
            }

            return FileValidationStatus.Valid;
        }
    }
}
