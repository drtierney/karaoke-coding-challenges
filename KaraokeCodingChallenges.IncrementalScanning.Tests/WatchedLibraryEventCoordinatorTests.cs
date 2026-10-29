using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.IncrementalScanning.Watchers;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MetadataReader;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class WatchedLibraryEventCoordinatorTests
{
    private sealed class FakeMetadataReader : IMetadataReader
    {
        private int _readCount;

        public int ReadCount => Volatile.Read(ref _readCount);

        public EmbeddedMetadata Read(string filePath)
        {
            Interlocked.Increment(ref _readCount);

            return new EmbeddedMetadata
            {
                Title = "Test Track",
                Artist = "Test Artist",
                DurationSeconds = 180
            };
        }
    }

    private sealed class TestEnvironment : IDisposable
    {
        public string DirectoryPath { get; }
        public string ConnectionString { get; }
        public long SourceId { get; }

        public MediaTrackRepository Repository { get; }
        public FakeMetadataReader MetadataReader { get; }
        public WatchedLibraryUpdateService UpdateService { get; }
        public LibraryFileWatcher Watcher { get; }
        public WatchedLibraryEventCoordinator Coordinator { get; }

        public TestEnvironment()
        {
            DirectoryPath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}");

            Directory.CreateDirectory(DirectoryPath);

            string databasePath = Path.Combine(DirectoryPath, "library.db");

            ConnectionString = $"Data Source={databasePath};Pooling=False";

            MediaLibraryDatabaseInitializer initializer = new(ConnectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(ConnectionString);

            SourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = DirectoryPath,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            Repository = new MediaTrackRepository(ConnectionString);
            MetadataReader = new FakeMetadataReader();

            ScannedTrackPersistenceService persistenceService = new(Repository, ConnectionString);

            UpdateService = new WatchedLibraryUpdateService(
                Repository,
                persistenceService,
                MetadataReader,
                new FileFingerprintProvider());

            Watcher = new LibraryFileWatcher(DirectoryPath);

            Coordinator = new WatchedLibraryEventCoordinator(
                Watcher,
                UpdateService,
                SourceId);
        }

        public string GetFilePath(string fileName)
        {
            return Path.Combine(DirectoryPath, fileName);
        }

        public long AddTrack(string filePath)
        {
            return Repository.Add(new MediaTrackRecord
            {
                SourceId = SourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = filePath,
                IsAvailable = true
            });
        }

        public void Dispose()
        {
            Coordinator.Dispose();
            Watcher.Dispose();

            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
    }

    private static async Task WaitUntilAsync(
        Func<bool> condition,
        TimeSpan? timeout = null)
    {
        TimeSpan maxWait = timeout ?? TimeSpan.FromSeconds(5);

        DateTime deadline = DateTime.UtcNow.Add(maxWait);

        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50);
        }

        Assert.True(condition(), "Expected condition was not reached before timeout.");
    }

    [Fact]
    public async Task FileDeleted_WithExistingTrack_MarksDatabaseRecordUnavailable()
    {
        using TestEnvironment environment = new();

        string filePath = environment.GetFilePath("Track1.mp3");

        File.WriteAllText(filePath, "Test audio content");

        long originalTrackId = environment.AddTrack(filePath);

        environment.Watcher.Start();

        File.Delete(filePath);

        await WaitUntilAsync(() => environment.Repository.GetById(originalTrackId)?.IsAvailable == false);

        MediaTrackRecord? storedTrack = environment.Repository.GetById(originalTrackId);

        Assert.NotNull(storedTrack);
        Assert.False(storedTrack.IsAvailable);
        Assert.Equal(originalTrackId, storedTrack.Id);
        Assert.Equal(filePath, storedTrack.FilePath);
        Assert.Single(environment.Repository.GetAll());
    }

    [Fact]
    public async Task FileRenamed_WithExistingTrack_UpdatesPathAndPreservesId()
    {
        using TestEnvironment environment = new();

        string originalPath = environment.GetFilePath("Track1.mp3");
        string renamedPath = environment.GetFilePath("RenamedTrack.mp3");

        File.WriteAllText(originalPath, "Test audio content");

        long originalTrackId = environment.AddTrack(originalPath);

        environment.Watcher.Start();

        File.Move(originalPath, renamedPath);

        await WaitUntilAsync(() => environment.Repository.GetById(originalTrackId)?.FilePath == renamedPath);

        MediaTrackRecord? renamedTrack = environment.Repository.GetById(originalTrackId);

        Assert.NotNull(renamedTrack);
        Assert.Equal(originalTrackId, renamedTrack.Id);
        Assert.Equal(renamedPath, renamedTrack.FilePath);
        Assert.Equal("Test Track", renamedTrack.Title);
        Assert.Equal("Test Artist", renamedTrack.Artist);

        Assert.Null(environment.Repository.GetByFilePath(originalPath));
        Assert.Single(environment.Repository.GetAll());
    }

    [Fact]
    public async Task FileCreated_WithSupportedFile_PersistsTrack()
    {
        using TestEnvironment environment = new();

        string filePath = environment.GetFilePath("NewTrack.mp3");

        environment.Watcher.Start();

        File.WriteAllText(filePath, "Test audio content");

        await WaitUntilAsync(() => environment.Repository.GetByFilePath(filePath) is not null);

        MediaTrackRecord? storedTrack = environment.Repository.GetByFilePath(filePath);

        Assert.NotNull(storedTrack);

        Assert.Equal(environment.SourceId, storedTrack.SourceId);
        Assert.Equal(filePath, storedTrack.FilePath);
        Assert.Equal("Test Track", storedTrack.Title);
        Assert.Equal("Test Artist", storedTrack.Artist);
        Assert.True(storedTrack.IsAvailable);

        Assert.Equal(new FileInfo(filePath).Length, storedTrack.FileSize);
        Assert.Equal(File.GetLastWriteTimeUtc(filePath), storedTrack.LastModifiedUtc);

        Assert.True(environment.MetadataReader.ReadCount >= 1);
        Assert.Single(environment.Repository.GetAll());
    }

    [Fact]
    public async Task FileChanged_WithModifiedFile_UpdatesFingerprintAndPreservesId()
    {
        using TestEnvironment environment = new();

        string filePath = environment.GetFilePath("Track1.mp3");

        File.WriteAllText(filePath, "Original audio content");

        long? originalTrackId = environment.UpdateService.HandleCreated(
            filePath,
            environment.SourceId);

        Assert.NotNull(originalTrackId);

        MediaTrackRecord? originalTrack = environment.Repository.GetById(originalTrackId.Value);

        Assert.NotNull(originalTrack);
        Assert.NotNull(originalTrack.FileSize);

        environment.Watcher.Start();

        File.AppendAllText(filePath, " Additional audio content");

        long expectedFileSize = new FileInfo(filePath).Length;

        await WaitUntilAsync(() =>
        {
            MediaTrackRecord? track = environment.Repository.GetById(originalTrackId.Value);

            return track?.FileSize == expectedFileSize && environment.MetadataReader.ReadCount >= 2;
        });

        MediaTrackRecord? updatedTrack = environment.Repository.GetById(originalTrackId.Value);

        Assert.NotNull(updatedTrack);

        Assert.Equal(originalTrackId.Value, updatedTrack.Id);
        Assert.True(updatedTrack.FileSize > originalTrack.FileSize);
        Assert.Equal(expectedFileSize, updatedTrack.FileSize);

        Assert.Equal(File.GetLastWriteTimeUtc(filePath), updatedTrack.LastModifiedUtc);

        Assert.True(updatedTrack.IsAvailable);
        Assert.Single(environment.Repository.GetAll());
    }

    [Fact]
    public async Task FileCreated_WithUnsupportedExtension_DoesNotPersistTrack()
    {
        using TestEnvironment environment = new();

        string filePath = environment.GetFilePath("Document.txt");

        TaskCompletionSource<bool> eventReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);

        environment.Watcher.FileCreated += (_, e) =>
        {
            if (string.Equals(
                e.FullPath,
                filePath,
                StringComparison.OrdinalIgnoreCase))
            {
                eventReceived.TrySetResult(true);
            }
        };

        environment.Watcher.Start();

        File.WriteAllText(filePath, "Not an audio file");

        await eventReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Allow deferred event processing to finish.
        await Task.Delay(750);

        Assert.Null(environment.Repository.GetByFilePath(filePath));
        Assert.Empty(environment.Repository.GetAll());
        Assert.Equal(0, environment.MetadataReader.ReadCount);
    }

    [Fact]
    public async Task FileRenamed_ToUnsupportedExtension_MarksOriginalTrackUnavailable()
    {
        using TestEnvironment environment = new();

        string originalPath = environment.GetFilePath("Track1.mp3");
        string renamedPath = environment.GetFilePath("Track1.txt");

        File.WriteAllText(originalPath, "Test audio content");

        long originalTrackId = environment.AddTrack(originalPath);

        environment.Watcher.Start();

        File.Move(originalPath, renamedPath);

        await WaitUntilAsync(() => environment.Repository.GetById(originalTrackId)?.IsAvailable == false);

        MediaTrackRecord? storedTrack = environment.Repository.GetById(originalTrackId);

        Assert.NotNull(storedTrack);

        Assert.Equal(originalTrackId, storedTrack.Id);
        Assert.False(storedTrack.IsAvailable);
        Assert.Equal(originalPath, storedTrack.FilePath);

        Assert.Single(environment.Repository.GetAll());
    }

    [Fact]
    public async Task FileRenamed_ToSupportedExtension_PersistsNewTrack()
    {
        using TestEnvironment environment = new();

        string originalPath = environment.GetFilePath("Track1.txt");
        string renamedPath = environment.GetFilePath("Track1.mp3");

        File.WriteAllText(originalPath, "Test audio content");

        environment.Watcher.Start();

        File.Move(originalPath, renamedPath);

        await WaitUntilAsync(() => environment.Repository.GetByFilePath(renamedPath) is not null);

        MediaTrackRecord? storedTrack = environment.Repository.GetByFilePath(renamedPath);

        Assert.NotNull(storedTrack);
        Assert.True(storedTrack.IsAvailable);
        Assert.Equal(renamedPath, storedTrack.FilePath);
        Assert.Equal("Test Track", storedTrack.Title);

        Assert.Null(environment.Repository.GetByFilePath(originalPath));
        Assert.Single(environment.Repository.GetAll());
    }


    [Fact]
    public async Task FileDeleted_BeforePendingScan_CompletesWithoutPersistingTrack()
    {
        using TestEnvironment environment = new();

        string filePath = environment.GetFilePath("TemporaryTrack.mp3");

        TaskCompletionSource<bool> creationReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);

        environment.Watcher.FileCreated += (_, e) =>
        {
            if (string.Equals(
                e.FullPath,
                filePath,
                StringComparison.OrdinalIgnoreCase))
            {
                creationReceived.TrySetResult(true);
            }
        };

        environment.Watcher.Start();

        File.WriteAllText(filePath, "Temporary audio content");

        await creationReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));

        File.Delete(filePath);

        // Allow time for any delayed scan to execute incorrectly.
        await Task.Delay(1500);

        Assert.False(File.Exists(filePath));
        Assert.Null(environment.Repository.GetByFilePath(filePath));
        Assert.Empty(environment.Repository.GetAll());
    }

}
