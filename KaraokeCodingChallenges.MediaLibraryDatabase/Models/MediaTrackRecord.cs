
namespace KaraokeCodingChallenges.MediaLibraryDatabase.Models
{
    public sealed record MediaTrackRecord
    {
        public long Id { get; init; }

        public long SourceId { get; init; }

        public required string Title { get; init; }

        public required string Artist { get; init; }

        public required string FilePath { get; init; }

        public int DurationSeconds { get; init; }

        public bool IsKaraoke { get; init; }

        public bool IsAvailable { get; set; } = true;

        public long? FileSize { get; set; }

        public DateTime? LastModifiedUtc { get; set; }
    }
}
