using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists.Tests;

public class MediaTrackRulesTests
{
    [Fact]
    public void ArtistContains_WhenArtistContainsValue_ReturnsMatchingRule()
    {
        SmartListRule<MediaTrackModel> rule = MediaTrackRules.ArtistContains("queen");

        MediaTrackModel track = new("Bohemian Rhapsody", "Queen", @"D:\Music\Queen - Bohemian Rhapsody.mp3");

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void ArtistContains_WhenArtistDoesNotContainValue_ReturnsNonMatchingRule()
    {
        SmartListRule<MediaTrackModel> rule = MediaTrackRules.ArtistContains("queen");

        MediaTrackModel track = new("Misery Business", "Paramore", @"D:\Music\Paramore - Misery Business.mp3");

        Assert.False(rule.Matches(track));
    }

    [Fact]
    public void ArtistContains_WhenValueUsesDifferentCase_ReturnsMatchingRule()
    {
        SmartListRule<MediaTrackModel> rule = MediaTrackRules.ArtistContains("QUEEN");

        MediaTrackModel track = new("Bohemian Rhapsody", "Queen", @"D:\Music\Queen - Bohemian Rhapsody.mp3");

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void ArtistContains_WhenValueIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => MediaTrackRules.ArtistContains(string.Empty));
    }

    [Fact]
    public void ArtistContains_WhenValueIsWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => MediaTrackRules.ArtistContains("   "));
    }

    [Fact]
    public void ArtistContains_WhenValueIsNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => MediaTrackRules.ArtistContains(null!));
    }

    [Fact]
    public void TitleContains_WhenTitleContainsValue_ReturnsMatchingRule()
    {
        SmartListRule<MediaTrackModel> rule = MediaTrackRules.TitleContains("rhapsody");

        MediaTrackModel track = new("Bohemian Rhapsody", "Queen", @"D:\Music\Queen - Bohemian Rhapsody.mp3");

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void TitleContains_WhenTitleDoesNotContainValue_ReturnsNonMatchingRule()
    {
        SmartListRule<MediaTrackModel> rule = MediaTrackRules.TitleContains("rhapsody");

        MediaTrackModel track = new("Misery Business", "Paramore", @"D:\Music\Paramore - Misery Business.mp3");

        Assert.False(rule.Matches(track));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TitleContains_WhenValueIsInvalid_ThrowsArgumentException(string? value)
    {
        Assert.Throws<ArgumentException>(() => MediaTrackRules.TitleContains(value!));
    }

    [Fact]
    public void SearchContains_WhenArtistContainsValue_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Music\Queen - Bohemian Rhapsody.mp3");

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.SearchContains("Queen");

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void SearchContains_WhenTitleContainsValue_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "Queen of Hearts",
            "Various Artists",
            @"D:\Music\Various Artists - Queen of Hearts.mp3");

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.SearchContains("Queen");

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void SearchContains_WhenNoFieldsContainValue_ReturnsNonMatchingRule()
    {
        MediaTrackModel track = new(
            "Misery Business",
            "Paramore",
            @"D:\Music\Paramore - Misery Business.mp3");

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.SearchContains("Queen");

        Assert.False(rule.Matches(track));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SearchContains_WhenValueIsInvalid_ThrowsArgumentException(string? value)
    {
        Assert.Throws<ArgumentException>(() => MediaTrackRules.SearchContains(value!));
    }

    [Fact]
    public void KaraokeOnly_WhenTrackIsKaraoke_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
            isKaraoke: true);

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.KaraokeOnly();

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void KaraokeOnly_WhenTrackIsMusic_ReturnsNonMatchingRule()
    {
        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            isKaraoke: false);

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.KaraokeOnly();

        Assert.False(rule.Matches(track));
    }

    [Fact]
    public void MinimumDuration_WhenTrackMeetsMinimum_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            durationSeconds: 354);

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.MinimumDuration(300);

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void MinimumDuration_WhenTrackIsBelowMinimum_ReturnsNonMatchingRule()
    {
        MediaTrackModel track = new(
            "Misery Business",
            "Paramore",
            @"D:\Music\Paramore - Misery Business.mp3",
            durationSeconds: 220);

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.MinimumDuration(300);

        Assert.False(rule.Matches(track));
    }

    [Fact]
    public void MinimumDuration_WhenTrackEqualsMinimum_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "The Last Song",
            "The All-American Rejects",
            @"D:\Music\The All-American Rejects - The Last Song.mp3",
            durationSeconds: 300);

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.MinimumDuration(300);

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void MinimumDuration_WhenValueIsNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MediaTrackRules.MinimumDuration(-1));
    }

    [Fact]
    public void MinimumDuration_WhenValueIsZero_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "Misery Business",
            "Paramore",
            @"D:\Music\Paramore - Misery Business.mp3",
            durationSeconds: 220);

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.MinimumDuration(0);

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void FilePathContains_WhenFilePathContainsValue_ReturnsMatchingRule()
    {
        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3");

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.FilePathContains("KARAOKE");

        Assert.True(rule.Matches(track));
    }

    [Fact]
    public void FilePathContains_WhenFilePathDoesNotContainValue_ReturnsNonMatchingRule()
    {
        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Music\Queen - Bohemian Rhapsody.mp3");

        SmartListRule<MediaTrackModel> rule = MediaTrackRules.FilePathContains("Karaoke");

        Assert.False(rule.Matches(track));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FilePathContains_WhenValueIsInvalid_ThrowsArgumentException(string? value)
    {
        Assert.Throws<ArgumentException>(() => MediaTrackRules.FilePathContains(value!));
    }
}
