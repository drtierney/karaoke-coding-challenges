namespace KaraokeCodingChallenges.PlaybackHistory;

public class PlaybackHistoryManager
{
    private readonly List<PlaybackHistoryEntry> _history = [];

    public void RecordPlay(string trackPath, DateTimeOffset playedAt)
    {
        if (string.IsNullOrWhiteSpace(trackPath))
        {
            throw new ArgumentException("Track path cannot be empty.", nameof(trackPath));
        }

        PlaybackHistoryEntry entry = new()
        {
            TrackPath = trackPath,
            PlayedAt = playedAt
        };

        _history.Add(entry);
    }

    public int GetPlayCount(string trackPath)
    {
        return _history.Count(entry => entry.TrackPath == trackPath);
    }

    public DateTimeOffset? GetLastPlayed(string trackPath)
    {
        return _history
            .Where(entry => entry.TrackPath == trackPath)
            .OrderByDescending(entry => entry.PlayedAt)
            .Select(entry => (DateTimeOffset?)entry.PlayedAt)
            .FirstOrDefault();
    }

    public IReadOnlyCollection<PlaybackHistoryEntry> GetRecentHistory()
    {
        return _history
            .OrderByDescending(entry => entry.PlayedAt)
            .ToList()
            .AsReadOnly();
    }

}
