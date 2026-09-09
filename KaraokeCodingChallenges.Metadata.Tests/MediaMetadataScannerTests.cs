using KaraokeCodingChallenges.MetadataReader;
using MediaMetadataRecord = KaraokeCodingChallenges.MediaMetadata.MediaMetadata;
using MetadataScanner = KaraokeCodingChallenges.MediaMetadataScanner.MediaMetadataScanner;


namespace KaraokeCodingChallenges.Metadata.Tests
{
    public class MediaMetadataScannerTests
    {
        private class FakeMetadataReader : IMetadataReader
        {
            private readonly EmbeddedMetadata _metadata;

            public FakeMetadataReader(EmbeddedMetadata metadata)
            {
                _metadata = metadata;
            }

            public EmbeddedMetadata Read(string filePath)
            {
                return _metadata;
            }
        }

        [Fact]
        public void ReadMetadata_ParsesArtistAndTitleFromFilenameWhenEmbeddedMetadataMissing()
        {

            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(tempDirectory, "Queen - Bohemian Rhapsody.mp3");
                File.WriteAllText(filePath, string.Empty);
                FakeMetadataReader metadataReader = new(new EmbeddedMetadata());

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Equal("Queen", result.Artist);
                Assert.Equal("Bohemian Rhapsody", result.Title);
            }
            finally
            {
                // Clean up the temporary directory and file
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void ReadMetadata_ParsesTrackNumberFromFilenameWhenEmbeddedMetadataMissing()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(tempDirectory, "01 - Queen - Bohemian Rhapsody.mp3");
                File.WriteAllText(filePath, string.Empty);
                FakeMetadataReader metadataReader = new(new EmbeddedMetadata());

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Equal(1, result.TrackNumber);
                Assert.Equal("Queen", result.Artist);
                Assert.Equal("Bohemian Rhapsody", result.Title);
            }
            finally
            {
                // Clean up the temporary directory and file
                Directory.Delete(tempDirectory, true);
            }
        }

        [Theory]
        [InlineData("Queen - Bohemian Rhapsody.mp3", "Queen", "Bohemian Rhapsody")]
        [InlineData("ABBA - Dancing Queen.mp3", "ABBA", "Dancing Queen")]
        [InlineData("Oasis - Wonderwall.mp3", "Oasis", "Wonderwall")]
        public void ReadMetadata_ParsesArtistAndTitleFromDifferentFilenames(string fileName, string expectedArtist, string expectedTitle)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(tempDirectory, fileName);
                File.WriteAllText(filePath, string.Empty);

                FakeMetadataReader metadataReader = new(new EmbeddedMetadata());

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Equal(expectedArtist, result.Artist);
                Assert.Equal(expectedTitle, result.Title);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void ReadMetadata_ThrowsFileNotFoundExceptionWhenFileDoesNotExist()
        {
            // Arrange
            string filePath = @"D:\Music\This File Does Not Exist.mp3";
            FakeMetadataReader metadataReader = new(new EmbeddedMetadata());

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => MetadataScanner.ReadMetadata(filePath, metadataReader));
        }

        [Fact]
        public void ReadMetadata_UsesEmbeddedMetadataWhenAvailable()
        {
            string tempDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(
                    tempDirectory,
                    "Filename Artist - Filename Title.mp3");

                File.WriteAllText(filePath, string.Empty);

                EmbeddedMetadata embeddedMetadata = new()
                {
                    Artist = "Queen",
                    Title = "Bohemian Rhapsody"
                };

                FakeMetadataReader metadataReader = new(embeddedMetadata);

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Equal(embeddedMetadata.Artist, result.Artist);
                Assert.Equal(embeddedMetadata.Title, result.Title);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void ReadMetadata_UsesFilenameAsTitleWhenNoSeparatorExists()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(tempDirectory, "Bohemian Rhapsody.mp3");

                File.WriteAllText(filePath, string.Empty);

                FakeMetadataReader metadataReader = new(new EmbeddedMetadata());

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Null(result.Artist);
                Assert.Equal("Bohemian Rhapsody", result.Title);
                Assert.Null(result.TrackNumber);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void ReadMetadata_UsesFilenameAsTitleWhenThreePartFilenameHasInvalidTrackNumber()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(tempDirectory, "TrackOne - Queen - Bohemian Rhapsody.mp3");

                File.WriteAllText(filePath, string.Empty);

                FakeMetadataReader metadataReader = new(new EmbeddedMetadata());

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Null(result.Artist);
                Assert.Equal("TrackOne - Queen - Bohemian Rhapsody", result.Title);
                Assert.Null(result.TrackNumber);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void ReadMetadata_UsesFilenameOnlyForMissingEmbeddedFields()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Arrange
                string filePath = Path.Combine(tempDirectory, "01 - Queen - Bohemian Rhapsody.mp3");

                File.WriteAllText(filePath, string.Empty);

                EmbeddedMetadata embeddedMetadata = new()
                {
                    Artist = "Embedded Queen",
                    Title = null,
                    TrackNumber = null
                };

                FakeMetadataReader metadataReader = new(embeddedMetadata);

                // Act
                MediaMetadataRecord result = MetadataScanner.ReadMetadata(filePath, metadataReader);

                // Assert
                Assert.Equal("Embedded Queen", result.Artist);
                Assert.Equal("Bohemian Rhapsody", result.Title);
                Assert.Equal(1, result.TrackNumber);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}