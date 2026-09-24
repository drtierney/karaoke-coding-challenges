namespace KaraokeCodingChallenges.PlaybackHistory;

public record PlaybackHistoryEntry
{
    public string TrackPath { get; init; } = string.Empty;
    public DateTimeOffset PlayedAt { get; init; }
}