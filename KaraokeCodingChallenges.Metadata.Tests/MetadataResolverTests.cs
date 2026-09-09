using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;
using KaraokeCodingChallenges.MetadataResolution;

namespace KaraokeCodingChallenges.Metadata.Tests
{
    public class MetadataResolutionServiceTests
    {
        [Fact]
        public void ResolveMetadata_UsesEmbeddedMetadataWhenAvailable()
        {
            // Arrange
            MediaMetadataRecord embeddedMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            };

            MediaMetadataRecord filenameMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Artist = "Filename Artist",
                Title = "Filename Title"
            };
            
            // Act
            MediaMetadataRecord result = MetadataResolutionService.ResolveMetadata(embeddedMetadata, filenameMetadata);

            // Assert
            Assert.Equal(embeddedMetadata.Artist, result.Artist);
            Assert.Equal(embeddedMetadata.Title, result.Title);
        }

        [Fact]
        public void ResolveMetadata_UsesFilenameMetadataWhenEmbeddedMetadataMissing()
        {
            // Arrange
            MediaMetadataRecord embeddedMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Artist = null,
                Title = null
            };

            MediaMetadataRecord filenameMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            };

            // Act
            MediaMetadataRecord result = MetadataResolutionService.ResolveMetadata(embeddedMetadata, filenameMetadata);
            
            // Assert
            Assert.Equal(filenameMetadata.Artist, result.Artist);
            Assert.Equal(filenameMetadata.Title, result.Title);
        }

        [Fact]
        public void ResolveMetadata_UsesFilenameMetadataWhenEmbeddedMetadataWhitespace()
        {
            // Arrange
            MediaMetadataRecord embeddedMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Artist = "   ",
                Title = ""
            };

            MediaMetadataRecord filenameMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            };

            // Act
            MediaMetadataRecord result =
                MetadataResolutionService.ResolveMetadata(embeddedMetadata, filenameMetadata);

            // Assert
            Assert.Equal(filenameMetadata.Artist, result.Artist);
            Assert.Equal(filenameMetadata.Title, result.Title);
        }

        [Fact]
        public void ResolveMetadata_UsesEmbeddedTrackNumberWhenAvailable()
        {
            // Arrange
            MediaMetadataRecord embeddedMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                TrackNumber = 1
            };

            MediaMetadataRecord filenameMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                TrackNumber = 2
            };

            // Act
            MediaMetadataRecord result = MetadataResolutionService.ResolveMetadata(embeddedMetadata, filenameMetadata);
            
            // Assert
            Assert.Equal(embeddedMetadata.TrackNumber, result.TrackNumber);
        }

        [Fact]
        public void ResolveMetadata_UsesFilenameTrackNumberWhenEmbeddedTrackNumberMissing()
        {
            // Arrange
            MediaMetadataRecord embeddedMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                TrackNumber = null
            };

            MediaMetadataRecord filenameMetadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                TrackNumber = 2
            };

            // Act
            MediaMetadataRecord result = MetadataResolutionService.ResolveMetadata(embeddedMetadata, filenameMetadata);

            // Assert
            Assert.Equal(filenameMetadata.TrackNumber, result.TrackNumber);
        }
    }
}