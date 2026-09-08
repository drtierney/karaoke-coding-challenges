using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;

namespace KaraokeCodingChallenges.MediaMetadataScanner
{
    public static class MediaMetadataScanner
    {
        public static MediaMetadataRecord ReadMetadata(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            FileInfo fileInfo = new(filePath);

            using TagLib.File tagFile = TagLib.File.Create(filePath);

            return new MediaMetadataRecord
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                Extension = fileInfo.Extension,
                FileSizeBytes = fileInfo.Length,

                Title = tagFile.Tag.Title,
                Artist = tagFile.Tag.FirstPerformer,
                Album = tagFile.Tag.Album,
                Genre = tagFile.Tag.FirstGenre,
                Year = tagFile.Tag.Year > 0 ? (int)tagFile.Tag.Year : null,
                TrackNumber = tagFile.Tag.Track > 0 ? (int)tagFile.Tag.Track : null,
                DurationSeconds = (int)tagFile.Properties.Duration.TotalSeconds,
                Bitrate = tagFile.Properties.AudioBitrate,
                SampleRate = tagFile.Properties.AudioSampleRate,
                Channels = tagFile.Properties.AudioChannels
            };
        }
    }
}