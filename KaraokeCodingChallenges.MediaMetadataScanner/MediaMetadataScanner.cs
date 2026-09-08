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

            var (fileArtist, fileTitle, fileTrackNumber) = ParseFileName(fileInfo.Name);

            return new MediaMetadataRecord
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                Extension = fileInfo.Extension,
                FileSizeBytes = fileInfo.Length,

                Artist = string.IsNullOrWhiteSpace(tagFile.Tag.FirstPerformer) ? fileArtist : tagFile.Tag.FirstPerformer,
                Title = string.IsNullOrWhiteSpace(tagFile.Tag.Title) ? fileTitle : tagFile.Tag.Title,
                TrackNumber = tagFile.Tag.Track > 0 ? (int)tagFile.Tag.Track : fileTrackNumber,
                Album = tagFile.Tag.Album,
                Genre = tagFile.Tag.FirstGenre,
                Year = tagFile.Tag.Year > 0 ? (int)tagFile.Tag.Year : null,
                DurationSeconds = (int)tagFile.Properties.Duration.TotalSeconds,
                Bitrate = tagFile.Properties.AudioBitrate,
                SampleRate = tagFile.Properties.AudioSampleRate,
                Channels = tagFile.Properties.AudioChannels
            };
        }

        private static (string? Artist, string? Title, int? TrackNumber) ParseFileName(string fileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string[] parts = fileNameWithoutExtension.Split(" - ");

            string? artist = null;
            string? title = fileNameWithoutExtension;
            int? trackNumber = null;

            if (parts.Length == 3 && int.TryParse(parts[0], out int parsedTrackNumber))
            {
                trackNumber = parsedTrackNumber;
                artist = parts[1];
                title = parts[2];
            }
            else if (parts.Length == 2)
            {
                artist = parts[0];
                title = parts[1];
            }

            return (artist, title, trackNumber);
        }
    }
}