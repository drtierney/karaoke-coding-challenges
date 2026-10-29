using KaraokeCodingChallenges.IncrementalScanning.Watchers;

namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public sealed class WatchedLibraryEventCoordinator : IDisposable
{
    private readonly LibraryFileWatcher _watcher;
    private readonly WatchedLibraryUpdateService _updateService;
    private readonly long _sourceId;
    private readonly bool _isKaraoke;

    private readonly FileEventDebouncer _debouncer = new(TimeSpan.FromMilliseconds(500));

    private bool _isDisposed;

    public event EventHandler<Exception>? ProcessingError;

    public WatchedLibraryEventCoordinator(
        LibraryFileWatcher watcher,
        WatchedLibraryUpdateService updateService,
        long sourceId,
        bool isKaraoke = false)
    {
        ArgumentNullException.ThrowIfNull(watcher);
        ArgumentNullException.ThrowIfNull(updateService);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sourceId);

        _watcher = watcher;
        _updateService = updateService;
        _sourceId = sourceId;
        _isKaraoke = isKaraoke;

        _watcher.FileCreated += OnFileCreated;
        _watcher.FileChanged += OnFileChanged;
        _watcher.FileDeleted += OnFileDeleted;
        _watcher.FileRenamed += OnFileRenamed;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _watcher.FileCreated -= OnFileCreated;
        _watcher.FileChanged -= OnFileChanged;
        _watcher.FileDeleted -= OnFileDeleted;
        _watcher.FileRenamed -= OnFileRenamed;

        _isDisposed = true;

        _debouncer.CancelPending();
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        if (!IsSupportedFile(e.FullPath))
        {
            return;
        }

        _ = ProcessFileAsync(e.FullPath);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (!IsSupportedFile(e.FullPath))
        {
            return;
        }

        _ = ProcessFileAsync(e.FullPath);
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        if (!IsSupportedFile(e.FullPath))
        {
            return;
        }

        try
        {
            _debouncer.CancelPending(e.FullPath);
            _updateService.HandleDeleted(e.FullPath);
        }
        catch (Exception ex)
        {
            ReportProcessingError(ex);
        }
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        bool oldSupported = IsSupportedFile(e.OldFullPath);
        bool newSupported = IsSupportedFile(e.FullPath);

        if (!oldSupported && !newSupported)
        {
            return;
        }

        try
        {
            _debouncer.CancelPending(e.OldFullPath);
            _debouncer.CancelPending(e.FullPath);

            if (oldSupported && newSupported)
            {
                _updateService.HandleRenamed(e.OldFullPath, e.FullPath);
            }
            else if (oldSupported)
            {
                _updateService.HandleDeleted(e.OldFullPath);
            }
            else
            {
                _ = ProcessFileAsync(e.FullPath);
            }
        }
        catch (Exception ex)
        {
            ReportProcessingError(ex);
        }
    }

    private static bool IsSupportedFile(string filePath)
    {
        return string.Equals(Path.GetExtension(filePath), ".mp3", StringComparison.OrdinalIgnoreCase);
    }

    private void ReportProcessingError(Exception exception)
    {
        try
        {
            ProcessingError?.Invoke(this, exception);
        }
        catch
        {
            // Prevent error-reporting failures from escaping the watcher callback.
        }
    }

    private async Task ProcessFileAsync(string filePath)
    {
        try
        {
            await _debouncer.DebounceAsync(filePath, () =>
            {
                if (!_isDisposed && File.Exists(filePath))
                {
                    _updateService.HandleChanged(
                        filePath,
                        _sourceId,
                        _isKaraoke);
                }

                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            ReportProcessingError(ex);
        }
    }
}
