namespace KaraokeCodingChallenges.SmartLists;

public class SmartList
{
    private readonly List<SmartListRule> _rules = [];

    public string Name { get; }

    public IReadOnlyList<SmartListRule> Rules => _rules.AsReadOnly();

    public int RuleCount => _rules.Count;

    public SmartList(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Smart list name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    public void AddRule(SmartListRule rule)
    {
        _rules.Add(rule);
    }

    public bool RemoveRule(SmartListRule rule)
    {
        return _rules.Remove(rule);
    }

    public void ClearRules()
    {
        _rules.Clear();
    }
}
