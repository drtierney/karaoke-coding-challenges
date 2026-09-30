using KaraokeCodingChallenges.LibraryPersistence;

public record MediaLibraryDto
{
    public int Version { get; init; }

    public List<MediaTrackDto> Tracks { get; init; } = [];
}
