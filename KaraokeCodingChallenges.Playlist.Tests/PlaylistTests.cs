using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.Playlist.Tests
{
    public class PlaylistTests
    {
        [Fact]
        public void Playlist_WhenCreated_StartsEmpty()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            Assert.Empty(playlist.Tracks);
            Assert.Equal(0, playlist.Count);
        }

        [Fact]
        public void AddTrack_AddsTrackToPlaylist()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            MediaTrackModel track = new(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");

            playlist.AddTrack(track);

            Assert.Single(playlist.Tracks);
            Assert.Equal(1, playlist.Count);
            Assert.Contains(track, playlist.Tracks);
        }

        [Fact]
        public void AddTrack_PreservesInsertionOrder()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            MediaTrackModel track1 = new(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");

            MediaTrackModel track2 = new(
                "The Only Exception",
                "Paramore",
                @"D:\Music\Paramore - The Only Exception.mp3");

            playlist.AddTrack(track1);
            playlist.AddTrack(track2);

            Assert.Equal(track1, playlist.Tracks[0]);
            Assert.Equal(track2, playlist.Tracks[1]);
        }

        [Fact]
        public void RemoveTrack_WhenTrackExists_ReturnsTrue()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            MediaTrackModel track = new(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");

            playlist.AddTrack(track);

            bool removed = playlist.RemoveTrack(track);

            Assert.True(removed);
            Assert.Empty(playlist.Tracks);
            Assert.Equal(0, playlist.Count);
        }

        [Fact]
        public void RemoveTrack_WhenTrackDoesNotExist_ReturnsFalse()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            MediaTrackModel track = new(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");

            bool removed = playlist.RemoveTrack(track);

            Assert.False(removed);
            Assert.Empty(playlist.Tracks);
        }

        [Fact]
        public void Clear_RemovesAllTracks()
        {
            MediaPlaylist playlist = new()
            {
                Name = "Test Playlist"
            };

            MediaTrackModel track1 = new(
                "Bohemian Rhapsody",
                "Queen",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");

            MediaTrackModel track2 = new(
                "The Only Exception",
                "Paramore",
                @"D:\Music\Paramore - The Only Exception.mp3");

            playlist.AddTrack(track1);
            playlist.AddTrack(track2);

            playlist.Clear();

            Assert.Empty(playlist.Tracks);
            Assert.Equal(0, playlist.Count);
        }
    }
}