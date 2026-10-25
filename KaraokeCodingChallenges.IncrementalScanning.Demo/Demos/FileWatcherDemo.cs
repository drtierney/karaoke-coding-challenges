using KaraokeCodingChallenges.IncrementalScanning.Watchers;

namespace KaraokeCodingChallenges.IncrementalScanning.Demo.Demos;

public static class FileWatcherDemo
{
    public static void Run()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), "karaoke-challenge-042-demo");

        Directory.CreateDirectory(directoryPath);

        using LibraryFileWatcher watcher = new LibraryFileWatcher(directoryPath);

        watcher.FileCreated += (sender, e) =>
        {
            Console.WriteLine($"File created: {e.FullPath}");
        };

        watcher.FileChanged += (sender, e) =>
        {
            Console.WriteLine($"File changed: {e.FullPath}");
        };

        watcher.FileDeleted += (sender, e) =>
        {
            Console.WriteLine($"File deleted: {e.FullPath}");
        };

        watcher.FileRenamed += (sender, e) =>
        {
            Console.WriteLine($"File renamed: {e.OldFullPath} --> {e.FullPath}");
        };

        watcher.Start();

        Console.WriteLine($"Watching directory: {directoryPath}");
        Console.WriteLine("Create, modify, rename or delete files in this directory.");
        Console.WriteLine("Press Enter to stop monitoring.");

        Console.ReadLine();

        watcher.Stop();
    }
}
