using KaraokeCodingChallenges.MediaMetadata;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.SearchPlaylistMilestone.Tests;

public class MediaTrackMapperTests
{
    [Fact]
    public void Map_WhenScanIsSuccessful_MapsMetadataToMediaTrack()
    {
        string filePath = Path.Combine(Path.GetTempPath(), "Music", "Queen", "Bohemian Rhapsody.mp3");

        MediaScanResult scanResult = new()
        {
            FilePath = filePath,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = filePath,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult);

        Assert.NotNull(track);
        Assert.Equal("Bohemian Rhapsody", track.Title);
        Assert.Equal("Queen", track.Artist);
        Assert.Equal(filePath, track.FilePath);
        Assert.Equal(354, track.DurationSeconds);
        Assert.False(track.IsKaraoke);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Map_WhenTitleIsMissing_UsesFileNameAsTitle(string? title)
    {
        string filePath = Path.Combine(Path.GetTempPath(), "Music", "Queen", "Bohemian Rhapsody.mp3");

        MediaScanResult scanResult = new()
        {
            FilePath = filePath,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = filePath,
                Title = title,
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult);

        Assert.NotNull(track);
        Assert.Equal("Bohemian Rhapsody", track.Title);
    }

    [Fact]
    public void Map_WhenScanIsUnsuccessful_ReturnsNull()
    {
        MediaScanResult scanResult = new()
        {
            FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
            IsSuccess = false
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult);

        Assert.Null(track);
    }

    [Fact]
    public void Map_WhenMetadataIsNull_ReturnsNull()
    {
        MediaScanResult scanResult = new()
        {
            FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = null
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult);

        Assert.Null(track);
    }

    [Fact]
    public void Map_WhenTrackIsKaraoke_SetsIsKaraokeToTrue()
    {
        MediaScanResult scanResult = new()
        {
            FilePath = @"D:\Karaoke\Queen\Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Karaoke\Queen\Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult, true);

        Assert.NotNull(track);
        Assert.True(track.IsKaraoke);
    }

    [Fact]
    public void Map_WhenArtistIsMissing_UsesEmptyString()
    {
        MediaScanResult scanResult = new()
        {
            FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = null,
                DurationSeconds = 354
            }
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult);

        Assert.NotNull(track);
        Assert.Equal(string.Empty, track.Artist);
    }

    [Fact]
    public void Map_WhenDurationIsMissing_UsesZero()
    {
        MediaScanResult scanResult = new()
        {
            FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = null
            }
        };

        MediaTrackMapper mapper = new();

        var track = mapper.Map(scanResult);

        Assert.NotNull(track);
        Assert.Equal(0, track.DurationSeconds);
    }
}
