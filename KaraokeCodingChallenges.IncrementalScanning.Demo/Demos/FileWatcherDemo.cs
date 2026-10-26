using KaraokeCodingChallenges.IncrementalScanning.Watchers;

namespace KaraokeCodingChallenges.IncrementalScanning.Demo.Demos;

public static class FileWatcherDemo
{
    public static void Run(bool showRawEvents = false)
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), "karaoke-challenge-042-demo");

        Directory.CreateDirectory(directoryPath);

        using LibraryFileWatcher watcher = new LibraryFileWatcher(directoryPath);

        if (showRawEvents)
        {
            watcher.RawFileChanged += (sender, e) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] RAW changed: {e.FullPath}");
            };
        }

        watcher.FileCreated += (sender, e) =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] File created: {e.FullPath}");
        };

        watcher.FileChanged += (sender, e) =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] File changed: {e.FullPath}");
        };

        watcher.FileDeleted += (sender, e) =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] File deleted: {e.FullPath}");
        };

        watcher.FileRenamed += (sender, e) =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] File renamed: {e.OldFullPath} --> {e.FullPath}");
        };

        watcher.WatcherError += (sender, e) =>
        {
            Console.Error.WriteLine(
                $"[{DateTime.Now:HH:mm:ss.fff}] Watcher error: {e.GetException().Message}");
        };

        watcher.Start();

        Console.WriteLine($"Watching directory: {directoryPath}");
        Console.WriteLine("Create, modify, rename or delete files in this directory.");
        Console.WriteLine("Press Enter to stop monitoring.");

        Console.ReadLine();

        watcher.Stop();
    }
}
