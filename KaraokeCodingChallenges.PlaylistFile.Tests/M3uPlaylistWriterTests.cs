using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playlist;

namespace KaraokeCodingChallenges.PlaylistFile.Tests
{
    public class M3uPlaylistWriterTests
    {
        [Fact]
        public void M3uPlaylistWriter_WhenPlaylistContainsTracks_WritesTrackPaths()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory,"Test Playlist.m3u");

            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track One",
                    "",
                    @"D:\Music\Track One.mp3"
                )
            );

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track Two",
                    "",
                    @"D:\Music\Track Two.mp3"
                )
            );

            try
            {
                // Act
                M3uPlaylistWriter writer = new();
                writer.Write(playlistPath, playlist);

                // Assert
                string[] lines = File.ReadAllLines(playlistPath);

                Assert.Equal(2, lines.Length);
                Assert.Equal(@"D:\Music\Track One.mp3", lines[0]);
                Assert.Equal(@"D:\Music\Track Two.mp3", lines[1]);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistWriter_WhenPlaylistContainsMultipleTracks_PreservesTrackOrder()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory,"Test Playlist.m3u");

            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track One",
                    "",
                    @"D:\Music\Track One.mp3"
                )
            );

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track Two",
                    "",
                    @"D:\Music\Track Two.mp3"
                )
            );

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track Three",
                    "",
                    @"D:\Music\Track Three.mp3"
                )
            );

            try
            {
                // Act
                M3uPlaylistWriter writer = new();
                writer.Write(playlistPath, playlist);

                // Assert
                string[] lines = File.ReadAllLines(playlistPath);

                Assert.Equal(@"D:\Music\Track One.mp3", lines[0]);
                Assert.Equal(@"D:\Music\Track Two.mp3", lines[1]);
                Assert.Equal(@"D:\Music\Track Three.mp3", lines[2]);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistWriter_WhenPlaylistIsEmpty_CreatesEmptyFile()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string playlistPath = Path.Combine(tempDirectory,"Empty Playlist.m3u");

            MediaPlaylist playlist = new()
            {
                Name = "Empty Playlist"
            };

            try
            {
                // Act
                M3uPlaylistWriter writer = new();
                writer.Write(playlistPath, playlist);

                // Assert
                Assert.True(File.Exists(playlistPath));
                string[] lines = File.ReadAllLines(playlistPath);
                Assert.Empty(lines);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void M3uPlaylistWriter_WhenUsingRelativePaths_WritesPathsRelativeToPlaylist()
        {
            // Arrange
            string tempDirectory = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            string playlistDirectory = Path.Combine(tempDirectory, "Playlists");
            string musicDirectory = Path.Combine(tempDirectory, "Music");

            Directory.CreateDirectory(playlistDirectory);
            Directory.CreateDirectory(musicDirectory);

            string playlistPath = Path.Combine(playlistDirectory, "Relative Playlist.m3u");

            MediaPlaylist playlist = new()
            {
                Name = "Relative Playlist"
            };

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track One",
                    "",
                    Path.Combine(musicDirectory, "Track One.mp3")
                )
            );

            playlist.AddTrack(
                new MediaTrackModel(
                    "Track Two",
                    "",
                    Path.Combine(musicDirectory, "Track Two.mp3")
                )
            );

            try
            {
                // Act
                M3uPlaylistWriter writer = new();

                writer.Write(
                    playlistPath,
                    playlist,
                    useRelativePaths: true
                );

                // Assert
                string[] lines = File.ReadAllLines(playlistPath);

                Assert.Equal(Path.Combine("..", "Music", "Track One.mp3"), lines[0]);

                Assert.Equal(Path.Combine("..", "Music", "Track Two.mp3"), lines[1]);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}
