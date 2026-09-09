using KaraokeCodingChallenges.MetadataReader;
using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;

namespace KaraokeCodingChallenges.MediaMetadataScanner
{
    public static class MediaMetadataScanner
    {
        public static MediaMetadataRecord ReadMetadata(string filePath, IMetadataReader metadataReader)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            FileInfo fileInfo = new(filePath);

            EmbeddedMetadata metadata = metadataReader.Read(filePath);

            var (fileArtist, fileTitle, fileTrackNumber) = ParseFileName(fileInfo.Name);

            return new MediaMetadataRecord
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                Extension = fileInfo.Extension,
                FileSizeBytes = fileInfo.Length,

                Artist = string.IsNullOrWhiteSpace(metadata.Artist) ? fileArtist : metadata.Artist,
                Title = string.IsNullOrWhiteSpace(metadata.Title) ? fileTitle : metadata.Title,
                Album = metadata.Album,
                Genre = metadata.Genre,
                Year = metadata.Year,
                TrackNumber = metadata.TrackNumber ?? fileTrackNumber,
                DurationSeconds = metadata.DurationSeconds,
                Bitrate = metadata.Bitrate,
                SampleRate = metadata.SampleRate,
                Channels = metadata.Channels
            };
        }

        private static (string? Artist, string? Title, int? TrackNumber)
            ParseFileName(string fileName)
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