namespace KaraokeCodingChallenges.SmartLists;

public class SmartList<T>
{
    private readonly List<SmartListRule<T>> _rules = [];

    public string Name { get; }

    public SmartListMatchMode MatchMode { get; }

    public IReadOnlyList<SmartListRule<T>> Rules => _rules.AsReadOnly();

    public int RuleCount => _rules.Count;

    public SmartList(string name, SmartListMatchMode matchMode = SmartListMatchMode.All)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Smart list name cannot be empty.", nameof(name));
        }

        if (!Enum.IsDefined(matchMode))
        {
            throw new ArgumentOutOfRangeException(nameof(matchMode), matchMode, "Unsupported smart list match mode.");
        }

        Name = name;
        MatchMode = matchMode;
    }

    public void AddRule(SmartListRule<T> rule)
    {
        _rules.Add(rule);
    }

    public bool RemoveRule(SmartListRule<T> rule)
    {
        return _rules.Remove(rule);
    }

    public void ClearRules()
    {
        _rules.Clear();
    }
}
