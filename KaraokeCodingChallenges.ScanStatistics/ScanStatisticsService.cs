using ScanResultRecord = KaraokeCodingChallenges.ScanResult.ScanResult;
using KaraokeFilePairingService = KaraokeCodingChallenges.KaraokeFilePairing.KaraokeFilePairing;

namespace KaraokeCodingChallenges.ScanStatistics
{
    public static class ScanStatisticsService
    {
        public static ScanStatistics GenerateStatistics(IEnumerable<ScanResultRecord> scanResults, KaraokeFilePairingService karaokeFilePairing)
        {
            var results = scanResults.ToList();

            int totalScans = results.Count;
            int successfulScans = results.Count(r => r.IsSuccess);
            int failedScans = results.Count(r => !r.IsSuccess);
            int totalWarnings = results.Sum(r => r.Warnings.Count);

            var filesByExtension = results
                .GroupBy(r => Path.GetExtension(r.FilePath).ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.Count());

            var successfulMetadata = results
                .Where(r => r.IsSuccess && r.Metadata is not null)
                .Select(r => r.Metadata!)
                .ToList();

            int missingArtistCount = successfulMetadata.Count(m => string.IsNullOrWhiteSpace(m.Artist));

            int missingTitleCount = successfulMetadata.Count(m => string.IsNullOrWhiteSpace(m.Title));

            int missingAlbumCount = successfulMetadata.Count(m => string.IsNullOrWhiteSpace(m.Album));

            int missingGenreCount = successfulMetadata.Count(m => string.IsNullOrWhiteSpace(m.Genre));

            int missingTrackNumberCount = successfulMetadata.Count(m => !m.TrackNumber.HasValue);

            int completeMetadataCount = successfulMetadata.Count(m =>
                !string.IsNullOrWhiteSpace(m.Artist) &&
                !string.IsNullOrWhiteSpace(m.Title) &&
                !string.IsNullOrWhiteSpace(m.Album) &&
                !string.IsNullOrWhiteSpace(m.Genre) &&
                m.TrackNumber.HasValue);

            double completeMetadataPercentage = successfulMetadata.Count > 0 ? (double)completeMetadataCount / successfulMetadata.Count * 100 : 0;

            return new ScanStatistics
            {
                TotalScans = totalScans,
                SuccessfulScans = successfulScans,
                FailedScans = failedScans,
                TotalWarnings = totalWarnings,
                SuccessPercentage = totalScans > 0 ? (double)successfulScans / totalScans * 100 : 0,
                FailurePercentage = totalScans > 0 ? (double)failedScans / totalScans * 100 : 0,
                FilesByExtension = filesByExtension,
                MissingArtistCount = missingArtistCount,
                MissingTitleCount = missingTitleCount,
                MissingAlbumCount = missingAlbumCount,
                MissingGenreCount = missingGenreCount,
                MissingTrackNumberCount = missingTrackNumberCount,
                CompleteMetadataCount = completeMetadataCount,
                CompleteMetadataPercentage = completeMetadataPercentage,
                MatchedKaraokePairs = karaokeFilePairing.MatchedPairs.Count,
                UnmatchedMp3Files = karaokeFilePairing.UnmatchedMp3Files.Count,
                UnmatchedCdgFiles = karaokeFilePairing.UnmatchedCdgFiles.Count
            };
        }
    }
}