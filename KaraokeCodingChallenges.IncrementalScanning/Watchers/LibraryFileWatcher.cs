using KaraokeCodingChallenges.IncrementalScanning.Services;

namespace KaraokeCodingChallenges.IncrementalScanning.Watchers;

public class LibraryFileWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly FileEventDebouncer _debouncer;
    private readonly object _lifecycleSync = new();
    private bool _isRunning;
    private bool _isDisposed;

    public event FileSystemEventHandler? FileCreated;
    public event FileSystemEventHandler? RawFileChanged;
    public event FileSystemEventHandler? FileChanged;
    public event FileSystemEventHandler? FileDeleted;
    public event RenamedEventHandler? FileRenamed;

    public event ErrorEventHandler? WatcherError;

    public LibraryFileWatcher(string directoryPath, TimeSpan? debounceInterval = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
        }

        _debouncer = new FileEventDebouncer(debounceInterval ?? TimeSpan.FromMilliseconds(500));

        _watcher = new FileSystemWatcher(directoryPath);

        _watcher.Created += OnFileCreated;
        _watcher.Changed += OnFileChanged;
        _watcher.Deleted += OnFileDeleted;
        _watcher.Renamed += OnFileRenamed;
        _watcher.Error += OnWatcherError;

        _watcher.IncludeSubdirectories = true;
    }

    public void Start()
    {
        lock (_lifecycleSync)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            _isRunning = true;
            _watcher.EnableRaisingEvents = true;
        }
    }

    public void Stop()
    {
        lock (_lifecycleSync)
        {
            if (_isDisposed)
            {
                return;
            }

            _isRunning = false;
            _watcher.EnableRaisingEvents = false;
            _debouncer.CancelPending();
        }
    }

    public void Dispose()
    {
        lock (_lifecycleSync)
        {
            if (_isDisposed)
            {
                return;
            }

            _isRunning = false;
            _watcher.EnableRaisingEvents = false;
            _debouncer.CancelPending();

            _isDisposed = true;
            _watcher.Dispose();
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        if (!IsRunning())
        {
            return;
        }

        FileCreated?.Invoke(this, e);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        lock (_lifecycleSync)
        {
            if (!_isRunning || _isDisposed)
            {
                return;
            }

            _ = ProcessFileChangedAsync(e);
        }

        try
        {
            RawFileChanged?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            ReportError(ex);
        }
    }

    private async Task ProcessFileChangedAsync(FileSystemEventArgs e)
    {
        try
        {
            await _debouncer.DebounceAsync(e.FullPath, () =>
            {
                if (IsRunning())
                {
                    FileChanged?.Invoke(this, e);
                }

                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            ReportError(ex);
        }
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        if (!IsRunning())
        {
            return;
        }

        _debouncer.CancelPending(e.FullPath);
        FileDeleted?.Invoke(this, e);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        if (!IsRunning())
        {
            return;
        }

        _debouncer.CancelPending(e.OldFullPath);
        _debouncer.CancelPending(e.FullPath);
        FileRenamed?.Invoke(this, e);
    }

    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        ReportError(e.GetException());
    }

    private void ReportError(Exception exception)
    {
        try
        {
            WatcherError?.Invoke(this, new ErrorEventArgs(exception));
        }
        catch
        {
            // Prevent an error subscriber from crashing the watcher callback.
        }
    }

    private bool IsRunning()
    {
        lock (_lifecycleSync)
        {
            return _isRunning && !_isDisposed;
        }
    }
}
