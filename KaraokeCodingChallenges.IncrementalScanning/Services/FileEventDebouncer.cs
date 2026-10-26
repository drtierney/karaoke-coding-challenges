namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public class FileEventDebouncer
{
    private readonly TimeSpan _debounceInterval;
    private readonly object _sync = new();
    private readonly Dictionary<string, CancellationTokenSource> _pendingEvents = new(StringComparer.OrdinalIgnoreCase);

    public FileEventDebouncer(TimeSpan debounceInterval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(debounceInterval, TimeSpan.Zero);

        _debounceInterval = debounceInterval;
    }

    public async Task DebounceAsync(string filePath, Func<Task> callback)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(callback);

        using CancellationTokenSource cancellation = new CancellationTokenSource();

        lock (_sync)
        {
            if (_pendingEvents.TryGetValue(filePath, out CancellationTokenSource? existing))
            {
                existing.Cancel();
            }

            _pendingEvents[filePath] = cancellation;
        }

        try
        {
            await Task.Delay(_debounceInterval, cancellation.Token);

            cancellation.Token.ThrowIfCancellationRequested();

            await callback();
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // Expected when a pending debounce operation is cancelled.
        }
        finally
        {
            lock (_sync)
            {
                if (_pendingEvents.TryGetValue(filePath, out CancellationTokenSource? current) && ReferenceEquals(current, cancellation))
                {
                    _pendingEvents.Remove(filePath);
                }
            }
        }
    }

    public void CancelPending()
    {
        lock (_sync)
        {
            foreach (CancellationTokenSource cancellation in _pendingEvents.Values)
            {
                cancellation.Cancel();
            }
        }
    }

    public void CancelPending(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        lock (_sync)
        {
            if (_pendingEvents.TryGetValue(filePath, out CancellationTokenSource? cancellation))
            {
                cancellation.Cancel();
            }
        }
    }
}
