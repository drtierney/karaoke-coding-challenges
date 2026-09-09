namespace KaraokeCodingChallenges.MetadataReader
{
    public interface IMetadataReader
    {
        EmbeddedMetadata Read(string filePath);
    }
}
