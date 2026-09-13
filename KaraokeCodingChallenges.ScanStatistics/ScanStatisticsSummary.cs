namespace KaraokeCodingChallenges.ScanStatistics
{
    public record ScanStatisticsSummary
    {
        public int TotalScans { get; init; }
        public int SuccessfulScans { get; init; }
        public int FailedScans { get; init; }

        public double SuccessPercentage { get; init; }
        public double FailurePercentage { get; init; }
        public int TotalWarnings { get; init; }

        public IReadOnlyDictionary<string, int> FilesByExtension { get; init; } = new Dictionary<string, int>();

        public int MissingArtistCount { get; init; }
        public int MissingTitleCount { get; init; }
        public int MissingAlbumCount { get; init; }
        public int MissingGenreCount { get; init; }
        public int MissingTrackNumberCount { get; init; }
        public int CompleteMetadataCount { get; init; }
        public double CompleteMetadataPercentage { get; init; }

        public int MatchedKaraokePairs { get; init; }
        public int UnmatchedMp3Files { get; init; }
        public int UnmatchedCdgFiles { get; init; }
    }
}
