using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playlist;

namespace KaraokeCodingChallenges.PlaylistFile.Tests;


public class M3uPlaylistRoundTripTests
{
    [Fact]
    public void M3uPlaylist_WhenWrittenAndReadBack_PreservesTrackPathsAndOrder()
    {
        // Arrange
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        string playlistDirectory = Path.Combine(tempDirectory, "Playlists");
        string musicDirectory = Path.Combine(tempDirectory, "Music");

        Directory.CreateDirectory(playlistDirectory);
        Directory.CreateDirectory(musicDirectory);

        string playlistPath = Path.Combine(playlistDirectory, "Round Trip Playlist.m3u");

        MediaPlaylist originalPlaylist = new()
        {
            Name = "Round Trip Playlist"
        };

        originalPlaylist.AddTrack(
            new MediaTrackModel(
                "Track One",
                "",
                Path.Combine(musicDirectory, "Track One.mp3")
            )
        );

        originalPlaylist.AddTrack(
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
            writer.Write(playlistPath, originalPlaylist, useRelativePaths: true);

            M3uPlaylistReader reader = new();
            MediaPlaylist importedPlaylist = reader.Read(playlistPath);

            // Assert
            Assert.Equal(originalPlaylist.Name, importedPlaylist.Name);
            Assert.Equal(originalPlaylist.Count, importedPlaylist.Count);
            Assert.Equal(originalPlaylist.Tracks[0].FilePath, importedPlaylist.Tracks[0].FilePath);
            Assert.Equal(originalPlaylist.Tracks[1].FilePath,importedPlaylist.Tracks[1].FilePath);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }
}
