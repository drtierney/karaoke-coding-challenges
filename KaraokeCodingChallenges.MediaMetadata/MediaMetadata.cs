namespace KaraokeCodingChallenges.MediaMetadata
{
    public record MediaMetadata
    {
        public required string FilePath { get; init; }
        public string? Title { get; init; }
        public string? Artist { get; init; }
        public string? Album { get; init; }
        public string? Genre { get; init; }
        public int? Year { get; init; }
        public int? TrackNumber { get; init; }
        public int? DurationSeconds { get; init; }
        public int? Bitrate { get; init; }
        public int? SampleRate { get; init; }
        public int? Channels { get; init; }
    }
}