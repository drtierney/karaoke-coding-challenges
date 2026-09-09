namespace KaraokeCodingChallenges.MetadataReader
{
    public class TagLibMetadataReader : IMetadataReader
    {
        public EmbeddedMetadata Read(string filePath)
        {
            using TagLib.File file = TagLib.File.Create(filePath);

            return new EmbeddedMetadata
            {
                Title = file.Tag.Title,
                Artist = file.Tag.FirstPerformer,
                Album = file.Tag.Album,
                Genre = file.Tag.FirstGenre,
                Year = file.Tag.Year > 0 ? (int)file.Tag.Year : null,
                TrackNumber = file.Tag.Track > 0 ? (int)file.Tag.Track : null,           
                DurationSeconds = (int)file.Properties.Duration.TotalSeconds,
                Bitrate = file.Properties.AudioBitrate > 0 ? file.Properties.AudioBitrate : null,
                SampleRate = file.Properties.AudioSampleRate > 0 ? file.Properties.AudioSampleRate : null,
                Channels = file.Properties.AudioChannels > 0 ? file.Properties.AudioChannels : null,
            };
        }
    }
}
