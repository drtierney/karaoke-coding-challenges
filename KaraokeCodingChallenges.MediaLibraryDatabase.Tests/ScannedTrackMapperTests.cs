using KaraokeCodingChallenges.MediaLibraryDatabase.Mapping;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaMetadata;
using KaraokeCodingChallenges.ScanResult;


namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests;

public class ScannedTrackMapperTests
{
    [Fact]
    public void Map_WhenScanIsSuccessful_ReturnsMediaTrackRecord()
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1, false);

        Assert.NotNull(result);
        Assert.Equal(1, result.SourceId);
        Assert.Equal("Bohemian Rhapsody", result.Title);
        Assert.Equal("Queen", result.Artist);
        Assert.Equal(@"D:\Music\Queen - Bohemian Rhapsody.mp3", result.FilePath);
        Assert.Equal(354, result.DurationSeconds);
        Assert.False(result.IsKaraoke);
    }

    [Fact]
    public void Map_WhenScanFails_ReturnsNull()
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Broken Track.mp3",
            IsSuccess = false,
            Metadata = null
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1, false);

        Assert.Null(result);
    }

    [Fact]
    public void Map_WhenMetadataIsNull_ReturnsNull()
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Missing Metadata.mp3",
            IsSuccess = true,
            Metadata = null
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Map_WhenTitleIsMissing_UsesFileName(string? title)
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = title,
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1, false);

        Assert.NotNull(result);
        Assert.Equal("Queen - Bohemian Rhapsody", result.Title);
    }

    [Fact]
    public void Map_WhenArtistIsMissing_UsesEmptyString()
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Unknown Artist - Track.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Unknown Artist - Track.mp3",
                Title = "Unknown Artist - Track",
                Artist = null,
                DurationSeconds = 200
            }
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1, false);

        Assert.NotNull(result);

        Assert.Equal(string.Empty, result.Artist);
    }

    [Fact]
    public void Map_WhenDurationIsMissing_UsesZero()
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Unknown Duration - Track.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Unknown Duration - Track.mp3",
                Title = "Unknown Duration - Track",
                Artist = "Some Artist",
                DurationSeconds = null
            }
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1, false);

        Assert.NotNull(result);

        Assert.Equal(0, result.DurationSeconds);
    }

    [Fact]
    public void Map_WhenIsKaraokeIsTrue_SetIsKaraoke()
    {
        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Karaoke Track.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Karaoke Track.mp3",
                Title = "Karaoke Track",
                Artist = "Some Artist",
                DurationSeconds = 180
            }
        };

        MediaTrackRecord? result = ScannedTrackMapper.Map(scanResult, 1, true);

        Assert.NotNull(result);

        Assert.True(result.IsKaraoke);
    }
}
