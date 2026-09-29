using KaraokeCodingChallenges.Playlist;
using KaraokeCodingChallenges.PlaylistFile;

namespace KaraokeCodingChallenges.PlaylistFile.Tests
{
    public class M3uPlaylistReaderTests
    {
        [Fact]
        public void M3uPlaylistReader_WhenFileContainsAbsolutePaths_ReturnsTracks()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory, "Test Playlist.m3u");

            string[] trackPaths =
            [
                Path.Combine(tempDirectory, "Music", "Queen - Bohemian Rhapsody.mp3"),
                Path.Combine(tempDirectory, "Music", "Paramore - The Only Exception.mp3")
            ];

            File.WriteAllLines(playlistPath, trackPaths);

            try
            {
                // Act
                M3uPlaylistReader reader = new();
                MediaPlaylist playlist = reader.Read(playlistPath);

                // Assert
                Assert.Equal("Test Playlist", playlist.Name);
                Assert.Equal(2, playlist.Count);
                Assert.Equal(trackPaths[0], playlist.Tracks[0].FilePath);
                Assert.Equal(trackPaths[1], playlist.Tracks[1].FilePath);
                Assert.Equal("Queen - Bohemian Rhapsody", playlist.Tracks[0].Title);
                Assert.Equal("Paramore - The Only Exception", playlist.Tracks[1].Title);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistReader_WhenFileContainsRelativePaths_ResolvesAbsolutePaths()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory, "Queen Playlist.m3u");

            string relativeTrackPath = Path.Combine("Queen", "Bohemian Rhapsody.mp3");
            File.WriteAllLines(playlistPath, [relativeTrackPath]);

            try
            {
                // Act
                M3uPlaylistReader reader = new();
                MediaPlaylist playlist = reader.Read(playlistPath);

                // Assert
                string expectedPath = Path.GetFullPath(Path.Combine(tempDirectory, relativeTrackPath));

                Assert.Single(playlist.Tracks);
                Assert.Equal(expectedPath, playlist.Tracks[0].FilePath);
                Assert.Equal("Bohemian Rhapsody", playlist.Tracks[0].Title);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistReader_WhenFileContainsBlankLines_IgnoresBlankLines()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory, "Test Playlist.m3u");

            string firstTrackPath = Path.Combine(tempDirectory, "Music", "Queen - Bohemian Rhapsody.mp3");

            string secondTrackPath = Path.Combine(tempDirectory, "Music", "Paramore - The Only Exception.mp3");

            string[] lines =
            [
                firstTrackPath,
                "",
                "  ",
                secondTrackPath
            ];

            File.WriteAllLines(playlistPath, lines);

            try
            {
                // Act
                M3uPlaylistReader reader = new();
                MediaPlaylist playlist = reader.Read(playlistPath);

                // Assert
                Assert.Equal(2, playlist.Count);
                Assert.Equal(firstTrackPath, playlist.Tracks[0].FilePath);
                Assert.Equal(secondTrackPath, playlist.Tracks[1].FilePath);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistReader_WhenFileContainsComments_IgnoresComments()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory,"Test Playlist.m3u");

            string firstTrackPath = Path.Combine(
                tempDirectory,
                "Music",
                "Queen - Bohemian Rhapsody.mp3");

            string secondTrackPath = Path.Combine(
                tempDirectory,
                "Music",
                "Paramore - The Only Exception.mp3");

            string[] lines =
            [
                "#EXTM3U",
                firstTrackPath,
                "# This is a comment",
                secondTrackPath
            ];

            File.WriteAllLines(playlistPath, lines);

            try
            {
                // Act
                M3uPlaylistReader reader = new();

                MediaPlaylist playlist = reader.Read(playlistPath);

                // Assert
                Assert.Equal(2, playlist.Count);
                Assert.Equal(firstTrackPath, playlist.Tracks[0].FilePath);
                Assert.Equal(secondTrackPath, playlist.Tracks[1].FilePath);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistReader_WhenFileContainsMultipleTracks_PreservesTrackOrder()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory, "Test Playlist.m3u");

            string[] trackPaths =
            [
                Path.Combine(tempDirectory, "Music", "Track One.mp3"),
                Path.Combine(tempDirectory, "Music", "Track Two.mp3"),
                Path.Combine(tempDirectory, "Music", "Track Three.mp3")
            ];

            File.WriteAllLines(playlistPath, trackPaths);

            try
            {
                // Act
                M3uPlaylistReader reader = new();
                MediaPlaylist playlist = reader.Read(playlistPath);

                // Assert
                Assert.Equal(3, playlist.Count);
                Assert.Equal(trackPaths[0], playlist.Tracks[0].FilePath);
                Assert.Equal(trackPaths[1], playlist.Tracks[1].FilePath);
                Assert.Equal(trackPaths[2], playlist.Tracks[2].FilePath);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistReader_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.m3u");
            M3uPlaylistReader reader = new();

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => reader.Read(filePath));
        }
    }
}
