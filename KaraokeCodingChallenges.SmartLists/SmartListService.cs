namespace KaraokeCodingChallenges.SmartLists;

public class SmartListService
{
    public IReadOnlyList<T> Apply<T>(SmartList<T> smartList, IEnumerable<T> items)
    {
        if (smartList.RuleCount == 0)
        {
            return items.ToList();
        }
        return items
            .Where(item => smartList.MatchMode switch
            {
                SmartListMatchMode.All => smartList.Rules.All(rule => rule.Matches(item)),
                SmartListMatchMode.Any => smartList.Rules.Any(rule => rule.Matches(item)),
                _ => throw new ArgumentOutOfRangeException(nameof(smartList.MatchMode), smartList.MatchMode, "Unsupported smart list match mode.")
            })
            .ToList();
    }
}