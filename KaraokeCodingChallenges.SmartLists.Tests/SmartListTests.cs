using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists.Tests;

public class SmartListTests
{
    [Fact]
    public void SmartList_WhenNameIsNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartList<MediaTrackModel>(null!));
    }

    [Fact]
    public void SmartList_WhenNameIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartList<MediaTrackModel>(string.Empty));
    }

    [Fact]
    public void SmartList_WhenNameIsWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartList<MediaTrackModel>("   "));
    }

    [Fact]
    public void SmartList_WhenCreated_StoresName()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        Assert.Equal("Smart List", smartList.Name);
    }

    [Fact]
    public void SmartList_WhenCreated_StartsWithNoRules()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        Assert.Empty(smartList.Rules);
    }

    [Fact]
    public void AddRule_AddsRuleToSmartList()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        Assert.Contains(rule, smartList.Rules);
    }

    [Fact]
    public void AddRule_PreservesInsertionOrder()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule1 = new("Karaoke tracks", track => track.IsKaraoke);

        SmartListRule<MediaTrackModel> rule2 = new("Queen tracks", track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        Assert.Equal(new[] { rule1, rule2 }, smartList.Rules);
    }

    [Fact]
    public void RemoveRule_WhenRuleExists_ReturnsTrue()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        bool result = smartList.RemoveRule(rule);
        Assert.True(result);
    }

    [Fact]
    public void RemoveRule_WhenRuleDoesNotExist_ReturnsFalse()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule = new("Karaoke tracks", track => track.IsKaraoke);

        bool result = smartList.RemoveRule(rule);
        Assert.False(result);
    }

    [Fact]
    public void ClearRules_RemovesAllRules()
    {
        SmartList<MediaTrackModel> smartList = new("Smart List");

        SmartListRule<MediaTrackModel> rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        smartList.ClearRules();

        Assert.Empty(smartList.Rules);
    }

    [Fact]
    public void Constructor_WithInvalidMatchMode_ThrowsArgumentOutOfRangeException()
    {
        SmartListMatchMode invalidMode = (SmartListMatchMode)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => new SmartList<MediaTrackModel>("Smart List", invalidMode));
    }
}