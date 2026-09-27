namespace KaraokeCodingChallenges.SmartLists;

public class SmartListRule<T>
{
    public string Name { get; }

    private readonly Func<T, bool> _predicate;

    public SmartListRule(string name, Func<T, bool> predicate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Smart list rule name cannot be empty.", nameof(name));
        }

        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        Name = name;
    }

    public bool Matches(T item)
    {
        return _predicate(item);
    }
}