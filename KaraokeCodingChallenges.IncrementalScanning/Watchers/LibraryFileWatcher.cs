namespace KaraokeCodingChallenges.IncrementalScanning.Watchers;

public class LibraryFileWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;

    public event FileSystemEventHandler? FileCreated;
    public event FileSystemEventHandler? FileChanged;
    public event FileSystemEventHandler? FileDeleted;
    public event RenamedEventHandler? FileRenamed;

    public LibraryFileWatcher(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
        }

        _watcher = new FileSystemWatcher(directoryPath);

        _watcher.Created += OnFileCreated;
        _watcher.Changed += OnFileChanged;
        _watcher.Deleted += OnFileDeleted;
        _watcher.Renamed += OnFileRenamed;

        _watcher.IncludeSubdirectories = true;
    }

    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        FileCreated?.Invoke(this, e);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        FileChanged?.Invoke(this, e);
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        FileDeleted?.Invoke(this, e);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        FileRenamed?.Invoke(this, e);
    }
}
