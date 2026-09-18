namespace KaraokeCodingChallenges.Configuration
{
    public class LibrarySource
    {
        public required string Path { get; init; }

        public LibrarySourceType Type { get; init; }

        public bool Enabled { get; init; } = true;
    }
}