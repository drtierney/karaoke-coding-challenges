namespace KaraokeCodingChallenges.PlaybackHistory.Tests;

public class PlaybackHistoryManagerTests
{
    [Fact]
    public void PlaybackHistoryManager_WhenCreated_HasNoHistory()
    {
        PlaybackHistoryManager manager = new();

        IReadOnlyCollection<PlaybackHistoryEntry> history = manager.GetRecentHistory();

        Assert.Empty(history);
    }

    [Fact]
    public void RecordPlay_WhenTrackIsPlayed_AddsHistoryEntry()
    {
        PlaybackHistoryManager manager = new();

        DateTimeOffset playedAt = new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero);

        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", playedAt);

        IReadOnlyCollection<PlaybackHistoryEntry> history = manager.GetRecentHistory();

        Assert.Single(history);

        PlaybackHistoryEntry entry = history.Single();

        Assert.Equal("Queen - Bohemian Rhapsody.mp3", entry.TrackPath);
        Assert.Equal(playedAt, entry.PlayedAt);
    }

    [Fact]
    public void GetPlayCount_WhenTrackIsPlayedOnce_ReturnsOne()
    {
        PlaybackHistoryManager manager = new();

        DateTimeOffset playedAt = new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero);

        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", playedAt);

        int playCount = manager.GetPlayCount("Queen - Bohemian Rhapsody.mp3");

        Assert.Equal(1, playCount);
    }

    [Fact]
    public void GetLastPlayed_WhenTrackIsPlayed_ReturnsMostRecentTime()
    {
        PlaybackHistoryManager manager = new();

        DateTimeOffset firstPlay = new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero);

        DateTimeOffset secondPlay = new(2026, 9, 24, 19, 0, 0, TimeSpan.Zero);

        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", firstPlay);
        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", secondPlay);

        DateTimeOffset? lastPlayed = manager.GetLastPlayed("Queen - Bohemian Rhapsody.mp3");

        Assert.Equal(secondPlay, lastPlayed);
    }

    [Fact]
    public void GetLastPlayed_WhenTrackHasNotBeenPlayed_ReturnsNull()
    {
        PlaybackHistoryManager manager = new();

        DateTimeOffset? lastPlayed = manager.GetLastPlayed("Queen - Bohemian Rhapsody.mp3");

        Assert.Null(lastPlayed);
    }

    [Fact]
    public void GetRecentHistory_ReturnsNewestEntriesFirst()
    {
        PlaybackHistoryManager manager = new();

        DateTimeOffset firstPlay = new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero);

        DateTimeOffset secondPlay = new(2026, 9, 24, 19, 0, 0, TimeSpan.Zero);

        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", firstPlay);
        manager.RecordPlay("Journey - Don't Stop Believin'.mp3", secondPlay);

        IReadOnlyCollection<PlaybackHistoryEntry> history = manager.GetRecentHistory();

        Assert.Equal("Journey - Don't Stop Believin'.mp3", history.First().TrackPath);
    }

    [Fact]
    public void GetPlayCount_WhenTrackIsPlayedMultipleTimes_ReturnsTotal()
    {
        PlaybackHistoryManager manager = new();

        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero));

        manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", new(2026, 9, 24, 19, 0, 0, TimeSpan.Zero));

        int playCount = manager.GetPlayCount("Queen - Bohemian Rhapsody.mp3");

        Assert.Equal(2, playCount);
    }

    [Fact]
    public void GetPlayCount_WhenTrackHasNotBeenPlayed_ReturnsZero()
    {
        PlaybackHistoryManager manager = new();

        int playCount = manager.GetPlayCount("Queen - Bohemian Rhapsody.mp3");

        Assert.Equal(0, playCount);
    }

    [Fact]
    public void GetRecentHistory_WhenNoTracksPlayed_ReturnsEmptyCollection()
    {
        PlaybackHistoryManager manager = new();

        IReadOnlyCollection<PlaybackHistoryEntry> history = manager.GetRecentHistory();

        Assert.Empty(history);
    }

    [Fact]
    public void RecordPlay_WhenTrackPathIsEmpty_ThrowsArgumentException()
    {
        PlaybackHistoryManager manager = new();

        Assert.Throws<ArgumentException>(() => manager.RecordPlay("", new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero)));
    }
}