using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists;

public class SmartListRule
{
    public string Name { get; }

    private readonly Func<MediaTrackModel, bool> _predicate;

    public SmartListRule(string name, Func<MediaTrackModel, bool> predicate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Smart list rule name cannot be empty.", nameof(name));
        }

        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        Name = name;
    }

    public bool Matches(MediaTrackModel track)
    {
        return _predicate(track);
    }
}