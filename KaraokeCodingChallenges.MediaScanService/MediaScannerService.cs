using KaraokeCodingChallenges.MediaMetadata;
using KaraokeCodingChallenges.MediaMetadataScanner;
using KaraokeCodingChallenges.MetadataReader;
using KaraokeCodingChallenges.ScanResult;


namespace KaraokeCodingChallenges.MediaScanService
{
    public static class MediaScannerService
    {
        public static IEnumerable<MediaScanResult> ScanFiles(IEnumerable<string> filePaths, IMetadataReader metadataReader)
        {
            foreach (string filePath in filePaths)
            {
                yield return ScanFile(filePath, metadataReader);
            }
        }

        public static MediaScanResult ScanFile(string filePath, IMetadataReader metadataReader)
        {
            try
            {
                MediaMetadataRecord metadata = MediaMetadataScannerService.ReadMetadata(filePath, metadataReader);

                List<string> warnings = [];

                if (string.IsNullOrWhiteSpace(metadata.Artist))
                {
                    warnings.Add("Artist metadata is missing.");
                }

                if (string.IsNullOrWhiteSpace(metadata.Title))
                {
                    warnings.Add("Title metadata is missing.");
                }

                return new MediaScanResult
                {
                    FilePath = filePath,
                    Metadata = metadata,
                    IsSuccess = true,
                    Warnings = warnings
                };
            }
            catch (FileNotFoundException ex)
            {
                return CreateFailedResult(filePath, ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return CreateFailedResult(filePath, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return CreateFailedResult(filePath, ex.Message);
            }
            catch (Exception ex)
            {
                return CreateFailedResult(filePath, $"Unexpected error: {ex.Message}");
            }
        }

        private static MediaScanResult CreateFailedResult(string filePath, string errorMessage)
        {
            return new MediaScanResult
            {
                FilePath = filePath,
                Metadata = null,
                IsSuccess = false,
                Errors = [errorMessage]
            };
        }
    }
}