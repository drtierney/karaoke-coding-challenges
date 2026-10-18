
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

namespace KaraokeCodingChallenges.IncrementalScanning.Models;

public sealed record FileMoveMatch
{
    public long TrackId { get; init; }

    public required string OldFilePath { get; init; }

    public required string NewFilePath { get; init; }
}
