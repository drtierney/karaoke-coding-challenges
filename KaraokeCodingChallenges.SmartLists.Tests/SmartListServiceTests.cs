using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists.Tests;

public class SmartListServiceTests
{
    [Fact]
    public void Apply_WhenNoRules_ReturnsAllTracks()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        List<MediaTrackModel> tracks =
        [
            new("Bohemian Rhapsody", "Queen", @"D:\Music\Queen.mp3"),
            new("Misery Business", "Paramore", @"D:\Music\Paramore.mp3")
        ];

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, tracks);

        Assert.Equal(tracks, result);
    }

    [Fact]
    public void Apply_WhenTrackMatchesSingleRule_ReturnsTrack()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule = new(
            "Karaoke tracks",
            track => track.IsKaraoke);

        smartList.AddRule(rule);

        MediaTrackModel karaokeTrack = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen.mp3",
            isKaraoke: true);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { karaokeTrack });

        Assert.Contains(karaokeTrack, result);
    }

    [Fact]
    public void Apply_WhenTrackDoesNotMatchSingleRule_ExcludesTrack()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule = new(
            "Karaoke tracks",
            track => track.IsKaraoke);

        smartList.AddRule(rule);

        MediaTrackModel musicTrack = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Music\Queen.mp3",
            isKaraoke: false);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { musicTrack });

        Assert.DoesNotContain(musicTrack, result);
    }

    [Fact]
    public void Apply_WhenTrackMatchesAllRules_ReturnsTrack()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule1 = new(
            "Karaoke tracks",
            track => track.IsKaraoke);
        SmartListRule<MediaTrackModel> rule2 = new(
            "Queen tracks",
            track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel karaokeTrack = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen.mp3",
            isKaraoke: true);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { karaokeTrack });

        Assert.Contains(karaokeTrack, result);
    }

    [Fact]
    public void Apply_WhenTrackDoesNotMatchAllRules_ExcludesTrack()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule1 = new(
            "Karaoke tracks",
            track => track.IsKaraoke);
        SmartListRule<MediaTrackModel> rule2 = new(
            "Queen tracks",
            track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel musicTrack = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen.mp3",
            isKaraoke: false);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { musicTrack });

        Assert.DoesNotContain(musicTrack, result);
    }

    [Fact]
    public void Apply_WhenGivenMultipleTracks_ReturnsOnlyTracksMatchingAllRules()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule1 = new(
            "Karaoke tracks",
            track => track.IsKaraoke);
        SmartListRule<MediaTrackModel> rule2 = new(
            "Queen tracks",
            track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel queenKaraoke = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
            isKaraoke: true);
        MediaTrackModel queenMusic = new(
            "Don't Stop Me Now",
            "Queen",
            @"D:\Music\Queen - Don't Stop Me Now.mp3",
            isKaraoke: false);
        MediaTrackModel paramoreKaraoke = new(
            "Misery Business",
            "Paramore",
            @"D:\Karaoke\Paramore - Misery Business.mp3",
            isKaraoke: true);

        List<MediaTrackModel> tracks =
        [
            queenKaraoke,
            queenMusic,
            paramoreKaraoke
        ];

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, tracks);

        Assert.Equal(new[] { queenKaraoke }, result);
    }

    [Fact]
    public void Apply_WithMatchAny_WhenTrackMatchesOneRule_ReturnsTrack()
    {
        SmartList<MediaTrackModel> smartList =
            new("Smart List", SmartListMatchMode.Any);

        SmartListRule<MediaTrackModel> rule1 = new(
            "Karaoke tracks",
            track => track.IsKaraoke);
        SmartListRule<MediaTrackModel> rule2 = new(
            "Queen tracks",
            track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel track = new(
            "Misery Business",
            "Paramore",
            @"D:\Karaoke\Paramore - Misery Business.mp3",
            isKaraoke: true);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { track });

        Assert.Contains(track, result);
    }

    [Fact]
    public void Apply_WithMatchAny_WhenTrackMatchesMultipleRules_ReturnsTrack()
    {
        SmartList<MediaTrackModel> smartList =
            new("Smart List", SmartListMatchMode.Any);

        SmartListRule<MediaTrackModel> rule1 = new(
            "Karaoke tracks",
            track => track.IsKaraoke);
        SmartListRule<MediaTrackModel> rule2 = new(
            "Queen tracks",
            track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel track = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
            isKaraoke: true);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { track });

        Assert.Contains(track, result);
    }

    [Fact]
    public void Apply_WithMatchAny_WhenTrackMatchesNoRules_ExcludesTrack()
    {
        SmartList<MediaTrackModel> smartList =
            new("Smart List", SmartListMatchMode.Any);

        SmartListRule<MediaTrackModel> rule1 = new(
            "Karaoke tracks",
            track => track.IsKaraoke);
        SmartListRule<MediaTrackModel> rule2 = new(
            "Queen tracks",
            track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel track = new(
            "Misery Business",
            "Paramore",
            @"D:\Music\Paramore - Misery Business.mp3",
            isKaraoke: false);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result =
            service.Apply(smartList, new[] { track });

        Assert.DoesNotContain(track, result);
    }

    [Fact]
    public void Apply_WithMatchAnyAndNoRules_ReturnsAllTracks()
    {
        SmartList<MediaTrackModel> smartList =
            new("Smart List", SmartListMatchMode.Any);

        List<MediaTrackModel> tracks =
        [
            new("Bohemian Rhapsody", "Queen", @"D:\Music\Queen.mp3"),
            new("Misery Business", "Paramore", @"D:\Music\Paramore.mp3")
        ];

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, tracks);

        Assert.Equal(tracks, result);
    }

    [Fact]
    public void Apply_WithGenericType_FiltersItems()
    {
        SmartList<string> smartList = new("Long names");

        SmartListRule<string> rule = new(
            "Names longer than five characters",
            item => item.Length > 5);

        smartList.AddRule(rule);

        string[] items =
        [
            "Queen",
            "Paramore",
            "Muse"
        ];

        SmartListService service = new();

        IReadOnlyList<string> result = service.Apply(smartList, items);

        Assert.Single(result);
        Assert.Contains("Paramore", result);
    }

    [Fact]
    public void Apply_WithMultipleMediaTrackRules_ReturnsTracksMatchingAllConditions()
    {
        List<MediaTrackModel> tracks =
        [
            new(
                title: "Bohemian Rhapsody",
                artist: "Queen",
                filePath: @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                isKaraoke: true,
                durationSeconds: 354),

            new(
                title: "Another One Bites The Dust",
                artist: "Queen",
                filePath: @"D:\Music\Queen - Another One Bites The Dust.mp3",
                isKaraoke: false,
                durationSeconds: 215),

            new(
                title: "Misery Business",
                artist: "Paramore",
                filePath: @"D:\Music\Paramore - Misery Business.mp3",
                isKaraoke: true,
                durationSeconds: 220),

            new(
                title: "We Will Rock You",
                artist: "Queen",
                filePath: @"D:\Music\Queen - We Will Rock You.mp3",
                isKaraoke: true,
                durationSeconds: 122)
        ];

        SmartList<MediaTrackModel> smartList = new("Long Queen karaoke");

        smartList.AddRule(MediaTrackRules.SearchContains("Queen"));
        smartList.AddRule(MediaTrackRules.KaraokeOnly());
        smartList.AddRule(MediaTrackRules.MinimumDuration(180));

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, tracks);

        MediaTrackModel track = Assert.Single(result);

        Assert.Equal("Bohemian Rhapsody", track.Title);
    }
}
