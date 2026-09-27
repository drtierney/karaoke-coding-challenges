

namespace KaraokeCodingChallenges.SmartLists.Tests;

public  class SmartListRulesTests
{
    [Fact]
    public void Any_WhenOneRuleMatches_ReturnsMatchingRule()
    {
        SmartListRule<string> startsWithA = new("Starts with A", value => value.StartsWith("A"));

        SmartListRule<string> endsWithZ = new("Ends with Z", value => value.EndsWith("Z"));

        SmartListRule<string> combinedRule = SmartListRules.Any(startsWithA, endsWithZ);

        Assert.True(combinedRule.Matches("Apple"));
    }

    [Fact]
    public void Any_WhenAllRulesMatch_ReturnsMatchingRule()
    {
        SmartListRule<string> startsWithHello = new("Starts with Hello", value => value.StartsWith("Hello"));

        SmartListRule<string> endsWithWorld = new("Ends with World", value => value.EndsWith("World"));

        SmartListRule<string> combinedRule = SmartListRules.Any(startsWithHello, endsWithWorld);

        Assert.True(combinedRule.Matches("Hello World"));
    }

    [Fact]
    public void Any_WhenNoRulesMatch_ReturnsNonMatchingRule()
    {
        SmartListRule<string> containsJava = new("Contains Java", value => value.Contains("Java"));

        SmartListRule<string> lengthOfTen = new("Length of 10", value => value.Length.Equals(10));

        SmartListRule<string> combinedRule = SmartListRules.Any(containsJava, lengthOfTen);

        Assert.False(combinedRule.Matches("CSharp"));
    }

    [Fact]
    public void Any_WhenNoRulesProvided_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SmartListRules.Any<string>());
    }

    [Fact]
    public void Any_WhenRulesAreNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SmartListRules.Any<string>(null!));
    }

    [Fact]
    public void Any_WhenRuleIsNull_ThrowsArgumentException()
    {
        SmartListRule<string> validRule = new("Starts with A", value => value.StartsWith("A"));

        Assert.Throws<ArgumentException>(() => SmartListRules.Any(validRule, null!));
    }

    [Fact]
    public void All_WhenAllRulesMatch_ReturnsMatchingRule()
    {
        SmartListRule<string> startsWithHello = new("Starts with Hello", value => value.StartsWith("Hello"));

        SmartListRule<string> endsWithWorld = new("Ends with World", value => value.EndsWith("World"));

        SmartListRule<string> combinedRule = SmartListRules.All(startsWithHello, endsWithWorld);

        Assert.True(combinedRule.Matches("Hello World"));
    }

    [Fact]
    public void All_WhenOneRuleDoesNotMatch_ReturnsNonMatchingRule()
    {
        SmartListRule<string> startsWithHello = new("Starts with Hello", value => value.StartsWith("Hello"));

        SmartListRule<string> endsWithWorld = new("Ends with World", value => value.EndsWith("World"));

        SmartListRule<string> combinedRule = SmartListRules.All(startsWithHello, endsWithWorld);

        Assert.False(combinedRule.Matches("Hello CSharp"));
    }

    [Fact]
    public void All_WhenNoRulesProvided_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SmartListRules.All<string>());
    }

    [Fact]
    public void All_WhenRulesAreNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SmartListRules.All<string>(null!));
    }

    [Fact]
    public void All_WhenRuleIsNull_ThrowsArgumentException()
    {
        SmartListRule<string> validRule = new("Starts with A", value => value.StartsWith("A"));

        Assert.Throws<ArgumentException>(() => SmartListRules.All(validRule, null!));
    }
}
