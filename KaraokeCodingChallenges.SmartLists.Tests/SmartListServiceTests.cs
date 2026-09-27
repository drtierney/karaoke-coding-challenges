using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists.Tests;

public class SmartListServiceTests
{
    [Fact]
    public void Apply_WhenNoRules_ReturnsAllTracks()
    {
        SmartList smartList = new("Smart List");

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
        SmartList smartList = new("Smart List");

        SmartListRule rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        MediaTrackModel karaokeTrack = new("Bohemian Rhapsody", "Queen", @"D:\Karaoke\Queen.mp3", isKaraoke: true);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, new[] { karaokeTrack });

        Assert.Contains(karaokeTrack, result);
    }

    [Fact]
    public void Apply_WhenTrackDoesNotMatchSingleRule_ExcludesTrack()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        MediaTrackModel musicTrack = new("Bohemian Rhapsody", "Queen", @"D:\Music\Queen.mp3", isKaraoke: false);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, new[] { musicTrack });

        Assert.DoesNotContain(musicTrack, result);
    }

    [Fact]
    public void Apply_WhenTrackMatchesAllRules_ReturnsTrack()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule1 = new("Karaoke tracks", track => track.IsKaraoke);

        SmartListRule rule2 = new("Queen tracks", track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel karaokeTrack = new("Bohemian Rhapsody", "Queen", @"D:\Karaoke\Queen.mp3", isKaraoke: true);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, new[] { karaokeTrack });

        Assert.Contains(karaokeTrack, result);
    }

    [Fact]
    public void Apply_WhenTrackDoesNotMatchAllRules_ExcludesTrack()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule1 = new("Karaoke tracks", track => track.IsKaraoke);

        SmartListRule rule2 = new("Queen tracks", track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel musicTrack = new("Bohemian Rhapsody", "Queen", @"D:\Karaoke\Queen.mp3", isKaraoke: false);

        SmartListService service = new();

        IReadOnlyList<MediaTrackModel> result = service.Apply(smartList, new[] { musicTrack });

        Assert.DoesNotContain(musicTrack, result);
    }

    [Fact]
    public void Apply_WhenGivenMultipleTracks_ReturnsOnlyTracksMatchingAllRules()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule1 = new("Karaoke tracks", track => track.IsKaraoke);

        SmartListRule rule2 = new("Queen tracks", track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        MediaTrackModel queenKaraoke = new("Bohemian Rhapsody", "Queen", @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3", isKaraoke: true);

        MediaTrackModel queenMusic = new("Don't Stop Me Now", "Queen", @"D:\Music\Queen - Don't Stop Me Now.mp3", isKaraoke: false);

        MediaTrackModel paramoreKaraoke = new("Misery Business", "Paramore", @"D:\Karaoke\Paramore - Misery Business.mp3", isKaraoke: true);

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
}
