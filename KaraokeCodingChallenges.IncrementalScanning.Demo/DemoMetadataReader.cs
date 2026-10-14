using KaraokeCodingChallenges.MetadataReader;

namespace KaraokeCodingChallenges.IncrementalScanning.Demo;

public class DemoMetadataReader : IMetadataReader
{
    public int ReadCount { get; private set; }

    public EmbeddedMetadata Read(string filePath)
    {
        ReadCount++;

        return new EmbeddedMetadata
        {
            Title = Path.GetFileNameWithoutExtension(filePath),
            Artist = "Demo Artist"
        };
    }
}
