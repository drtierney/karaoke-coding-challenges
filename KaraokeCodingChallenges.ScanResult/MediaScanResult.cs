using KaraokeCodingChallenges.MediaMetadata;

namespace KaraokeCodingChallenges.ScanResult
{
    public record MediaScanResult
    {
        public required string FilePath { get; init; }

        public MediaMetadataRecord? Metadata { get; init; }

        public bool IsSuccess { get; init; }

        public IReadOnlyCollection<string> Warnings { get; init; } = [];

        public IReadOnlyCollection<string> Errors { get; init; } = [];
    }
}
