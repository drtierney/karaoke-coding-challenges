using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MetadataReader;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class WatchedLibraryUpdateServiceTests
{
    private sealed class FakeMetadataReader : IMetadataReader
    {
        public int ReadCount { get; private set; }

        public EmbeddedMetadata Read(string filePath)
        {
            ReadCount++;

            return new EmbeddedMetadata
            {
                Title = "Test Track",
                Artist = "Test Artist",
                DurationSeconds = 180
            };
        }
    }

    private static WatchedLibraryUpdateService CreateService(
        IMediaTrackRepository repository,
        string connectionString,
        IMetadataReader? metadataReader = null)
    {
        ScannedTrackPersistenceService persistenceService = new(
            repository,
            connectionString);

        return new WatchedLibraryUpdateService(
            repository,
            persistenceService,
            metadataReader ?? new FakeMetadataReader(),
            new FileFingerprintProvider());
    }

    [Fact]
    public void HandleDeleted_WithExistingTrack_MarksUnavailable()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = "Music",
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            MediaTrackRecord originalTrack = new()
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = Path.Combine("Music", "Track1.mp3"),
                IsAvailable = true
            };

            long originalTrackId = repository.Add(originalTrack);

            WatchedLibraryUpdateService service = CreateService(repository, connectionString);

            bool deleteHandled = service.HandleDeleted(originalTrack.FilePath);

            Assert.True(deleteHandled);

            MediaTrackRecord? updatedTrack = repository.GetById(originalTrackId);

            Assert.NotNull(updatedTrack);
            Assert.False(updatedTrack.IsAvailable);
            Assert.Equal(originalTrackId, updatedTrack.Id);

            Assert.Equal(originalTrack.Title, updatedTrack.Title);
            Assert.Equal(originalTrack.Artist, updatedTrack.Artist);
            Assert.Equal(originalTrack.FilePath, updatedTrack.FilePath);

        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void HandleDeleted_WithUnknownPath_ReturnsFalse()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = "Music",
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            MediaTrackRepository repository = new(connectionString);

            MediaTrackRecord originalTrack = new()
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = Path.Combine("Music", "Track1.mp3"),
                IsAvailable = true
            };

            long originalTrackId = repository.Add(originalTrack);

            WatchedLibraryUpdateService service = CreateService(repository, connectionString);

            bool deleteHandled = service.HandleDeleted(Path.Combine("Music", "unknown.mp3"));

            Assert.False(deleteHandled);

            MediaTrackRecord? storedTrack = repository.GetById(originalTrackId);

            Assert.NotNull(storedTrack);
            Assert.True(storedTrack.IsAvailable);
            Assert.Equal(originalTrackId, storedTrack.Id);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void HandleRenamed_WithExistingTrack_UpdatesPathAndPreservesId()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = "Music",
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            MediaTrackRepository repository = new(connectionString);

            MediaTrackRecord originalTrack = new()
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = Path.Combine("Music", "Track1.mp3"),
                IsAvailable = true
            };

            long originalTrackId = repository.Add(originalTrack);

            WatchedLibraryUpdateService service = CreateService(repository, connectionString);

            string newFilePath = Path.Combine("Music", "New", "Track1.mp3");

            bool renameHandled = service.HandleRenamed(originalTrack.FilePath, newFilePath);

            Assert.True(renameHandled);

            MediaTrackRecord? renamedTrack = repository.GetById(originalTrackId);

            Assert.NotNull(renamedTrack);
            Assert.Equal(newFilePath, renamedTrack.FilePath);
            Assert.Equal(originalTrackId, renamedTrack.Id);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void HandleRenamed_WithExistingDestination_ReturnsFalse()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = "Music",
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            MediaTrackRepository repository = new(connectionString);

            MediaTrackRecord trackOne = new()
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = Path.Combine("Music", "Track1.mp3"),
                IsAvailable = true
            };

            MediaTrackRecord trackTwo = trackOne with
            {
                Title = "Test Track 2",
                FilePath = Path.Combine("Music", "Track2.mp3")
            };

            long trackOneId = repository.Add(trackOne);
            long trackTwoId = repository.Add(trackTwo);

            WatchedLibraryUpdateService service = CreateService(repository, connectionString);

            bool renameHandled = service.HandleRenamed(trackOne.FilePath, trackTwo.FilePath);

            Assert.False(renameHandled);

            MediaTrackRecord? storedTrackOne = repository.GetById(trackOneId);
            MediaTrackRecord? storedTrackTwo = repository.GetById(trackTwoId);

            Assert.NotNull(storedTrackOne);
            Assert.NotNull(storedTrackTwo);

            Assert.Equal(trackOne.FilePath, storedTrackOne.FilePath);
            Assert.Equal(trackTwo.FilePath, storedTrackTwo.FilePath);
            Assert.Equal(2, repository.GetAll().Count);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void HandleCreated_WithValidFile_PersistsTrackAndFingerprint()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string filePath = Path.ChangeExtension(databasePath, ".mp3");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            File.WriteAllText(filePath, "Test audio content");

            FileInfo fileInfo = new(filePath);
            long expectedFileSize = fileInfo.Length;
            DateTime expectedLastModifiedUtc = fileInfo.LastWriteTimeUtc;

            MediaTrackRepository repository = new(connectionString);

            WatchedLibraryUpdateService service = CreateService(repository, connectionString);

            long? trackId = service.HandleCreated(filePath, sourceId);

            Assert.NotNull(trackId);

            MediaTrackRecord? storedTrack = repository.GetById(trackId.Value);

            Assert.NotNull(storedTrack);

            Assert.Equal(sourceId, storedTrack.SourceId);
            Assert.Equal("Test Track", storedTrack.Title);
            Assert.Equal("Test Artist", storedTrack.Artist);
            Assert.Equal(filePath, storedTrack.FilePath);
            Assert.Equal(180, storedTrack.DurationSeconds);

            Assert.Equal(expectedFileSize, storedTrack.FileSize);
            Assert.Equal(expectedLastModifiedUtc, storedTrack.LastModifiedUtc);

            Assert.True(storedTrack.IsAvailable);
            Assert.False(storedTrack.IsKaraoke);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void HandleChanged_WithUnchangedFingerprint_SkipsRescan()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string filePath = Path.ChangeExtension(databasePath, ".mp3");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            File.WriteAllText(filePath, "Test audio content");

            MediaTrackRepository repository = new(connectionString);

            FakeMetadataReader metadataReader = new();

            WatchedLibraryUpdateService service = CreateService(
                repository,
                connectionString,
                metadataReader);

            // Initial scan and persistence
            long? originalTrackId = service.HandleCreated(filePath, sourceId);

            Assert.NotNull(originalTrackId);
            Assert.Equal(1, metadataReader.ReadCount);

            // Simulate a Changed notification without modifying the file
            long? returnedTrackId = service.HandleChanged(filePath, sourceId);

            // Verify the existing track was returned without rescanning
            Assert.Equal(originalTrackId, returnedTrackId);
            Assert.Equal(1, metadataReader.ReadCount);

            // Verify no duplicate record was created
            MediaTrackRecord? storedTrack = repository.GetById(originalTrackId.Value);

            Assert.NotNull(storedTrack);
            Assert.True(storedTrack.IsAvailable);
            Assert.Single(repository.GetAll());
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }


    [Fact]
    public void HandleChanged_WithModifiedFingerprint_UpdatesTrack()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"watched-library-{Guid.NewGuid():N}.db");

        string filePath = Path.ChangeExtension(databasePath, ".mp3");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);

            LibrarySourceRecord source = new()
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            };

            long sourceId = sourceRepository.Add(source);

            File.WriteAllText(filePath, "Original test content");

            MediaTrackRepository repository = new(connectionString);
            FakeMetadataReader metadataReader = new();

            WatchedLibraryUpdateService service = CreateService(
                repository,
                connectionString,
                metadataReader);

            // Initial scan
            long? originalTrackId = service.HandleCreated(filePath, sourceId);

            Assert.NotNull(originalTrackId);
            Assert.Equal(1, metadataReader.ReadCount);

            MediaTrackRecord? originalTrack = repository.GetById(originalTrackId.Value);

            Assert.NotNull(originalTrack);
            Assert.NotNull(originalTrack.FileSize);

            // Modify the file
            File.AppendAllText(filePath, " Additional test content");

            // Process the changed file
            long? updatedTrackId = service.HandleChanged(filePath, sourceId);

            Assert.Equal(originalTrackId, updatedTrackId);
            Assert.Equal(2, metadataReader.ReadCount);

            // Verify the database was updated
            MediaTrackRecord? updatedTrack = repository.GetById(originalTrackId.Value);

            Assert.NotNull(updatedTrack);
            Assert.Equal(originalTrackId.Value, updatedTrack.Id);

            Assert.True(updatedTrack.FileSize > originalTrack.FileSize);
            Assert.Equal(File.GetLastWriteTimeUtc(filePath), updatedTrack.LastModifiedUtc);

            Assert.Equal(originalTrack.Title, updatedTrack.Title);
            Assert.Equal(originalTrack.Artist, updatedTrack.Artist);
            Assert.True(updatedTrack.IsAvailable);

            // Ensure no duplicate record was created
            Assert.Single(repository.GetAll());
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

}
