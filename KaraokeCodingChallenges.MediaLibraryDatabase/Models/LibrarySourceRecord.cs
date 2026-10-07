using KaraokeCodingChallenges.Configuration;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Models;

public sealed record LibrarySourceRecord
{
    public long Id { get; init; }

    public required string Path { get; init; }

    public LibrarySourceType Type { get; init; }

    public bool Enabled { get; init; }
}
