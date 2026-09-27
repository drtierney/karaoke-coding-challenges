namespace KaraokeCodingChallenges.SmartLists.Tests;

public class SmartListTests
{
    [Fact]
    public void SmartList_WhenNameIsNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartList(null!));
    }

    [Fact]
    public void SmartList_WhenNameIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartList(string.Empty));
    }

    [Fact]
    public void SmartList_WhenNameIsWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmartList("   "));
    }

    [Fact]
    public void SmartList_WhenCreated_StoresName()
    {
        SmartList smartList = new("Smart List");

        Assert.Equal("Smart List", smartList.Name);
    }

    [Fact]
    public void SmartList_WhenCreated_StartsWithNoRules()
    {
        SmartList smartList = new("Smart List");

        Assert.Empty(smartList.Rules);
    }

    [Fact]
    public void AddRule_AddsRuleToSmartList()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        Assert.Contains(rule, smartList.Rules);
    }

    [Fact]
    public void AddRule_PreservesInsertionOrder()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule1 = new("Karaoke tracks", track => track.IsKaraoke);

        SmartListRule rule2 = new("Queen tracks", track => track.Artist == "Queen");

        smartList.AddRule(rule1);
        smartList.AddRule(rule2);

        Assert.Equal(new[] { rule1, rule2 }, smartList.Rules);
    }

    [Fact]
    public void RemoveRule_WhenRuleExists_ReturnsTrue()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        bool result = smartList.RemoveRule(rule);
        Assert.True(result);
    }

    [Fact]
    public void RemoveRule_WhenRuleDoesNotExist_ReturnsFalse()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule = new("Karaoke tracks", track => track.IsKaraoke);

        bool result = smartList.RemoveRule(rule);
        Assert.False(result);
    }

    [Fact]
    public void ClearRules_RemovesAllRules()
    {
        SmartList smartList = new("Smart List");

        SmartListRule rule = new("Karaoke tracks", track => track.IsKaraoke);

        smartList.AddRule(rule);

        smartList.ClearRules();

        Assert.Empty(smartList.Rules);
    }
}