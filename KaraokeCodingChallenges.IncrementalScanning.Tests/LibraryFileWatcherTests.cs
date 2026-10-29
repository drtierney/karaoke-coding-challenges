using KaraokeCodingChallenges.IncrementalScanning.Watchers;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class LibraryFileWatcherTests
{
    [Fact]
    public async Task Stop_WithPendingChange_PreventsNotification()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), $"karaoke-watcher-test-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            using LibraryFileWatcher watcher = new LibraryFileWatcher(directoryPath, TimeSpan.FromMilliseconds(500));

            TaskCompletionSource rawEventReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);

            int callbackCount = 0;

            watcher.RawFileChanged += (sender, e) =>
            {
                rawEventReceived.TrySetResult();
            };

            watcher.FileChanged += (sender, e) =>
            {
                Interlocked.Increment(ref callbackCount);
            };

            string filePath = Path.Combine(directoryPath, "test.txt");

            await File.WriteAllTextAsync(filePath, "Original content");

            watcher.Start();

            await File.AppendAllTextAsync(filePath, "\nUpdated content");

            await rawEventReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));

            watcher.Stop();

            await Task.Delay(650);

            Assert.Equal(0, callbackCount);
        }
        finally
        {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public async Task Dispose_WithPendingChange_PreventsNotification()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), $"karaoke-watcher-test-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            string filePath = Path.Combine(directoryPath, "test.txt");

            await File.WriteAllTextAsync(filePath, "Original content");

            LibraryFileWatcher watcher = new LibraryFileWatcher(directoryPath, TimeSpan.FromMilliseconds(500));

            TaskCompletionSource rawEventReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);

            int callbackCount = 0;

            watcher.RawFileChanged += (sender, e) =>
            {
                rawEventReceived.TrySetResult();
            };

            watcher.FileChanged += (sender, e) =>
            {
                Interlocked.Increment(ref callbackCount);
            };

            watcher.Start();

            await File.AppendAllTextAsync(filePath, "\nUpdated content");

            await rawEventReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));

            watcher.Dispose();

            await Task.Delay(650);

            Assert.Equal(0, callbackCount);
        }
        finally
        {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void Start_AfterDispose_ThrowsObjectDisposedException()
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"karaoke-watcher-test-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            LibraryFileWatcher watcher = new LibraryFileWatcher(directoryPath);

            watcher.Dispose();

            Assert.Throws<ObjectDisposedException>(() => watcher.Start());
        }
        finally
        {
            Directory.Delete(directoryPath, recursive: true);
        }
    }
}
