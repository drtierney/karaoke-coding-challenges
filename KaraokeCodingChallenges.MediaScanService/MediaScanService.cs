using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;
using MediaMetadataScannerService = KaraokeCodingChallenges.MediaMetadataScanner.MediaMetadataScanner;
using KaraokeCodingChallenges.MetadataReader;
using ScanResultRecord = KaraokeCodingChallenges.ScanResult.ScanResult;


namespace KaraokeCodingChallenges.MediaScanService
{
    public static class MediaScanService
    {
        public static IEnumerable<ScanResultRecord> ScanFiles(IEnumerable<string> filePaths, IMetadataReader metadataReader)
        {
            foreach (string filePath in filePaths)
            {
                yield return ScanFile(filePath, metadataReader);
            }
        }

        public static ScanResultRecord ScanFile(string filePath, IMetadataReader metadataReader)
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

                return new ScanResultRecord
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

        private static ScanResultRecord CreateFailedResult(string filePath, string errorMessage)
        {
            return new ScanResultRecord
            {
                FilePath = filePath,
                Metadata = null,
                IsSuccess = false,
                Errors = [errorMessage]
            };
        }
    }
}