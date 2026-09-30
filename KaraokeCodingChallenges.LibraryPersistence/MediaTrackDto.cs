namespace KaraokeCodingChallenges.LibraryPersistence;

public record MediaTrackDto
{
    public required string Title { get; init; }

    public required string Artist { get; init; }

    public required string FilePath { get; init; }

    public int DurationSeconds { get; init; }

    public bool IsKaraoke { get; init; }
}
