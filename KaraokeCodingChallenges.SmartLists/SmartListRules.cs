namespace KaraokeCodingChallenges.SmartLists;

public static class SmartListRules
{
    private static void ValidateRules<T>(SmartListRule<T>[] rules)
    {
        if (rules is null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        if (rules.Length == 0)
        {
            throw new ArgumentException("At least one rule must be provided.", nameof(rules));
        }

        if (rules.Any(rule => rule is null))
        {
            throw new ArgumentException("Rules cannot contain null values.", nameof(rules));
        }
    }

    public static SmartListRule<T> Any<T>(params SmartListRule<T>[] rules)
    {
        ValidateRules(rules);

        return new SmartListRule<T>(
            name: "Any",
            predicate: item => rules.Any(rule => rule.Matches(item)));
    }

    public static SmartListRule<T> All<T>(params SmartListRule<T>[] rules)
    {
        ValidateRules(rules);

        return new SmartListRule<T>(
            name: "All",
            predicate: item => rules.All(rule => rule.Matches(item)));
    }
}
