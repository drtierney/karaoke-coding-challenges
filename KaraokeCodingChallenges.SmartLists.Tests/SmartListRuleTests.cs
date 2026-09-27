using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists.Tests;

public class SmartListRuleTests
{
    [Fact]
    public void SmartListRule_WhenNameIsNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartListRule<MediaTrackModel>(null!, track => track.IsKaraoke));
    }

    [Fact]
    public void SmartListRule_WhenNameIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartListRule<MediaTrackModel>(string.Empty, track => track.IsKaraoke));
    }

    [Fact]
    public void SmartListRule_WhenNameIsWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartListRule<MediaTrackModel>("   ", track => track.IsKaraoke));
    }

    [Fact]
    public void SmartListRule_WhenPredicateIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SmartListRule<MediaTrackModel>("Karaoke tracks", null!));
    }

    [Fact]
    public void Matches_WhenTrackMatchesRule_ReturnsTrue()
    {
        MediaTrackModel track = new("Radio Ga Ga", "Queen", @"D:\Music\Queen - Radio Ga Ga.mp3", isKaraoke: true);

        SmartListRule<MediaTrackModel> rule = new("Karaoke tracks", track => track.IsKaraoke);

        bool result = rule.Matches(track);

        Assert.True(result);
    }

    [Fact]
    public void Matches_WhenTrackDoesNotMatchRule_ReturnsFalse()
    {
        MediaTrackModel track = new("Radio Ga Ga", "Queen", @"D:\Music\Queen - Radio Ga Ga.mp3", isKaraoke: false);

        SmartListRule<MediaTrackModel> rule = new("Karaoke tracks", track => track.IsKaraoke);

        bool result = rule.Matches(track);

        Assert.False(result);
    }
}