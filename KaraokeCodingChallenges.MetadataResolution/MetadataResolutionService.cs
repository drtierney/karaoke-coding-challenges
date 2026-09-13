using KaraokeCodingChallenges.MediaMetadata;

namespace KaraokeCodingChallenges.MetadataResolution
{
    public static class MetadataResolutionService
    {
        public static MediaMetadataRecord ResolveMetadata(
            MediaMetadataRecord embeddedMetadata,
            MediaMetadataRecord filenameMetadata)
        {
            return new MediaMetadataRecord
            {
                FilePath = embeddedMetadata.FilePath,
                Title = ResolveTitle(embeddedMetadata, filenameMetadata),
                Artist = ResolveArtist(embeddedMetadata, filenameMetadata),
                Album = ResolveAlbum(embeddedMetadata, filenameMetadata),
                Genre = ResolveGenre(embeddedMetadata, filenameMetadata),
                Year = ResolveYear(embeddedMetadata, filenameMetadata),
                TrackNumber = ResolveTrackNumber(embeddedMetadata, filenameMetadata),
                DurationSeconds = ResolveDurationSeconds(embeddedMetadata, filenameMetadata),
                Bitrate = ResolveBitrate(embeddedMetadata, filenameMetadata),
                SampleRate = ResolveSampleRate(embeddedMetadata, filenameMetadata),
                Channels = ResolveChannels(embeddedMetadata, filenameMetadata)
            };
        }

        public static string? ResolveTitle(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveString(embeddedMetadata.Title, filenameMetadata.Title);
        }

        public static string? ResolveArtist(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveString(embeddedMetadata.Artist, filenameMetadata.Artist);
        }

        public static int? ResolveTrackNumber(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveInt(embeddedMetadata.TrackNumber, filenameMetadata.TrackNumber);
        }

        public static string? ResolveAlbum(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveString(embeddedMetadata.Album, filenameMetadata.Album);
        }

        public static string? ResolveGenre(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveString(embeddedMetadata.Genre, filenameMetadata.Genre);
        }

        public static int? ResolveYear(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveInt(embeddedMetadata.Year, filenameMetadata.Year);
        }

        public static int? ResolveDurationSeconds(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveInt(embeddedMetadata.DurationSeconds, filenameMetadata.DurationSeconds);
        }

        public static int? ResolveBitrate(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveInt(embeddedMetadata.Bitrate, filenameMetadata.Bitrate);
        }

        public static int? ResolveSampleRate(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveInt(embeddedMetadata.SampleRate, filenameMetadata.SampleRate);
        }

        public static int? ResolveChannels(MediaMetadataRecord embeddedMetadata, MediaMetadataRecord filenameMetadata)
        {
            return ResolveInt(embeddedMetadata.Channels, filenameMetadata.Channels);
        }

        private static string? ResolveString(string? embeddedValue, string? filenameValue)
        {
            if (!string.IsNullOrWhiteSpace(embeddedValue))
            {
                return embeddedValue; 
            }

            if (!string.IsNullOrWhiteSpace(filenameValue))
            {
                return filenameValue;
            }

            return null;
        }


        private static int? ResolveInt(int? embeddedValue, int? filenameValue)
        {
            return embeddedValue ?? filenameValue;
        }
    }
}