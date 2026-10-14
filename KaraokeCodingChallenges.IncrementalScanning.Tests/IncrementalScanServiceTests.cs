using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MetadataReader;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class CountingMetadataReader : IMetadataReader
{
    public int ReadCount { get; private set; }

    public EmbeddedMetadata Read(string filePath)
    {
        ReadCount++;

        return new EmbeddedMetadata
        {
            Title = "Test Track",
            Artist = "Test Artist"
        };
    }
}


public class FailingMetadataReader : IMetadataReader
{
    public int ReadCount { get; private set; }

    public EmbeddedMetadata Read(string filePath)
    {
        ReadCount++;

        throw new InvalidDataException("Simulated metadata extraction failure.");
    }
}


public class IncrementalScanServiceTests
{

    [Fact]
    public void CheckFile_ReturnsNew_WhenTrackDoesNotExist()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"new-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            FileChangeStatus result = service.CheckFile(filePath);

            Assert.Equal(FileChangeStatus.New, result);
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
    public void CheckFile_ReturnsModified_WhenTrackHasNoFingerprint()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"modified-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = filePath,
                DurationSeconds = 0,
                IsKaraoke = false,
                FileSize = null,
                LastModifiedUtc = null
            });

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            FileChangeStatus result = service.CheckFile(filePath);

            Assert.Equal(FileChangeStatus.Modified, result);
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
    public void CheckFile_ReturnsUnchanged_WhenFingerprintMatches()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"unchanged-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            FileInfo fileInfo = new FileInfo(filePath);

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = filePath,
                DurationSeconds = 0,
                IsKaraoke = false,
                FileSize = fileInfo.Length,
                LastModifiedUtc = fileInfo.LastWriteTimeUtc
            });

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            FileChangeStatus result = service.CheckFile(filePath);

            Assert.Equal(FileChangeStatus.Unchanged, result);
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
    public void CheckFile_ReturnsModified_WhenFileSizeChanges()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"modified-size-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileFingerprint originalFingerprint = fingerprintProvider.GetFingerprint(filePath);

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = filePath,
                DurationSeconds = 0,
                IsKaraoke = false,
                FileSize = originalFingerprint.FileSize,
                LastModifiedUtc = originalFingerprint.LastModifiedUtc
            });

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            // Change the file size after storing its original fingerprint.
            File.WriteAllText(filePath, "Test - Modified");

            FileChangeStatus result = service.CheckFile(filePath);

            Assert.Equal(FileChangeStatus.Modified, result);
            Assert.NotEqual(originalFingerprint.FileSize, new FileInfo(filePath).Length);
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
    public void CheckFile_ReturnsModified_WhenLastModifiedUtcChanges()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"modified-timestamp-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileFingerprint originalFingerprint = fingerprintProvider.GetFingerprint(filePath);

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = filePath,
                DurationSeconds = 0,
                IsKaraoke = false,
                FileSize = originalFingerprint.FileSize,
                LastModifiedUtc = originalFingerprint.LastModifiedUtc
            });

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            // Change only the file's modification timestamp.
            DateTime modifiedTimestamp = originalFingerprint.LastModifiedUtc.AddMinutes(-5);

            File.SetLastWriteTimeUtc(filePath, modifiedTimestamp);

            FileChangeStatus result = service.CheckFile(filePath);

            Assert.Equal(FileChangeStatus.Modified, result);
            Assert.Equal(originalFingerprint.FileSize, new FileInfo(filePath).Length);
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
    public void ProcessFile_SkipsMetadataReading_WhenFileIsUnchanged()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"unchanged-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            FileInfo fileInfo = new FileInfo(filePath);

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Test Track",
                Artist = "Test Artist",
                FilePath = filePath,
                DurationSeconds = 0,
                IsKaraoke = false,
                FileSize = fileInfo.Length,
                LastModifiedUtc = fileInfo.LastWriteTimeUtc
            });

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            MediaScanResult? result = service.ProcessFile(filePath, sourceId, metadataReader);

            Assert.Null(result);
            Assert.Equal(0, metadataReader.ReadCount);
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
    public void ProcessFile_ScansAndPersists_WhenFileIsNew()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"new-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector, persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            FileFingerprint expectedFingerprint = fingerprintProvider.GetFingerprint(filePath);

            MediaScanResult? result = service.ProcessFile(filePath, sourceId, metadataReader);

            // Metadata scanning
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, metadataReader.ReadCount);

            // Database persistence
            MediaTrackRecord? persistedTrack =
                repository.GetByFilePath(filePath);

            Assert.NotNull(persistedTrack);
            Assert.Equal(sourceId, persistedTrack.SourceId);
            Assert.Equal("Test Track", persistedTrack.Title);
            Assert.Equal("Test Artist", persistedTrack.Artist);

            // Fingerprint persistence
            Assert.Equal(expectedFingerprint.FileSize, persistedTrack.FileSize);

            Assert.Equal(expectedFingerprint.LastModifiedUtc, persistedTrack.LastModifiedUtc);
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
    public void ProcessFile_RescansAndUpdates_WhenFileIsModified()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"modified-track-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(
                repository,
                fingerprintProvider,
                changeDetector,
                persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            // First scan
            MediaScanResult? firstResult = service.ProcessFile(filePath, sourceId, metadataReader);

            MediaTrackRecord? originalTrack = repository.GetByFilePath(filePath);

            Assert.NotNull(firstResult);
            Assert.True(firstResult.IsSuccess);
            Assert.NotNull(originalTrack);
            Assert.Equal(1, metadataReader.ReadCount);

            long originalId = originalTrack.Id;
            long originalFileSize = originalTrack.FileSize!.Value;

            // Modify the file
            File.WriteAllText(filePath, "Test - Modified");

            // Second scan
            MediaScanResult? secondResult = service.ProcessFile(filePath, sourceId, metadataReader);

            MediaTrackRecord? updatedTrack = repository.GetByFilePath(filePath);

            FileFingerprint updatedFingerprint = fingerprintProvider.GetFingerprint(filePath);

            // Verify the rescan
            Assert.NotNull(secondResult);
            Assert.True(secondResult.IsSuccess);
            Assert.Equal(2, metadataReader.ReadCount);

            // Verify the existing record was updated
            Assert.NotNull(updatedTrack);
            Assert.Equal(originalId, updatedTrack.Id);
            Assert.NotEqual(originalFileSize, updatedTrack.FileSize);

            // Verify the updated fingerprint was persisted
            Assert.Equal(updatedFingerprint.FileSize, updatedTrack.FileSize);
            Assert.Equal(updatedFingerprint.LastModifiedUtc, updatedTrack.LastModifiedUtc);
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
    public void ProcessFile_PreservesExistingTrack_WhenMetadataScanFails()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string filePath = Path.Combine(Path.GetTempPath(), $"failed-rescan-{Guid.NewGuid():N}.mp3");

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            File.WriteAllText(filePath, "Test");

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.GetDirectoryName(filePath)!,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service = new IncrementalScanService(
                repository,
                fingerprintProvider,
                changeDetector,
                persistenceService);

            // Initial successful scan
            CountingMetadataReader successfulReader = new CountingMetadataReader();

            MediaScanResult? firstResult = service.ProcessFile(filePath, sourceId, successfulReader);

            MediaTrackRecord? originalTrack = repository.GetByFilePath(filePath);

            Assert.NotNull(firstResult);
            Assert.True(firstResult.IsSuccess);
            Assert.NotNull(originalTrack);
            Assert.Equal(1, successfulReader.ReadCount);

            long originalId = originalTrack.Id;
            long originalFileSize = originalTrack.FileSize!.Value;
            DateTime originalLastModifiedUtc = originalTrack.LastModifiedUtc!.Value;

            string originalTitle = originalTrack.Title;

            // Modify the file to trigger a rescan
            File.WriteAllText(filePath, "Test - Modified");

            // Attempt the rescan with a failing metadata reader
            FailingMetadataReader failingReader = new FailingMetadataReader();

            MediaScanResult? failedResult = service.ProcessFile(filePath, sourceId, failingReader);

            // Verify the failed scan was attempted
            Assert.NotNull(failedResult);
            Assert.False(failedResult.IsSuccess);
            Assert.Equal(1, failingReader.ReadCount);

            // Verify the original track remains unchanged
            MediaTrackRecord? persistedTrack = repository.GetByFilePath(filePath);

            Assert.NotNull(persistedTrack);
            Assert.Equal(originalId, persistedTrack.Id);
            Assert.Equal(originalTitle, persistedTrack.Title);
            Assert.Equal(originalFileSize, persistedTrack.FileSize);
            Assert.Equal(originalLastModifiedUtc, persistedTrack.LastModifiedUtc);

            // Verify the file still requires scanning
            Assert.Equal(FileChangeStatus.Modified, service.CheckFile(filePath));
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
    public void ProcessFiles_SkipsAllFiles_WhenFingerprintsMatch()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string directoryPath = Path.Combine(Path.GetTempPath(), $"incremental-files-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            string[] filePaths =
            [
                Path.Combine(directoryPath, "track1.mp3"),
                Path.Combine(directoryPath, "track2.mp3"),
                Path.Combine(directoryPath, "track3.mp3")
            ];

            foreach (string filePath in filePaths)
            {
                File.WriteAllText(filePath, "Test");
            }

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = directoryPath,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service =
                new IncrementalScanService(
                    repository,
                    fingerprintProvider,
                    changeDetector,
                    persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            // Perform the initial scans to populate SQLite.
            foreach (string filePath in filePaths)
            {
                MediaScanResult? result = service.ProcessFile(
                    filePath,
                    sourceId,
                    metadataReader);

                Assert.NotNull(result);
                Assert.True(result.IsSuccess);
            }

            Assert.Equal(3, metadataReader.ReadCount);

            // Scan the same files again.
            IncrementalScanSummary summary = service.ProcessFiles(
                filePaths,
                sourceId,
                metadataReader);

            // All files should be skipped.
            Assert.Equal(3, summary.Total);
            Assert.Equal(0, summary.Added);
            Assert.Equal(0, summary.Updated);
            Assert.Equal(3, summary.Skipped);
            Assert.Equal(0, summary.Failed);

            // No additional metadata reads should have occurred.
            Assert.Equal(3, metadataReader.ReadCount);
        }
        finally
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void ProcessFiles_AddsNewFiles_AndUpdatesSummary()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string directoryPath = Path.Combine(Path.GetTempPath(), $"incremental-files-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            string[] filePaths =
            [
                Path.Combine(directoryPath, "track1.mp3"),
                Path.Combine(directoryPath, "track2.mp3"),
                Path.Combine(directoryPath, "track3.mp3")
            ];

            foreach (string filePath in filePaths)
            {
                File.WriteAllText(filePath, "Test");
            }

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = directoryPath,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service =
                new IncrementalScanService(
                    repository,
                    fingerprintProvider,
                    changeDetector,
                    persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            // Process three new files.
            IncrementalScanSummary summary = service.ProcessFiles(
                filePaths,
                sourceId,
                metadataReader);

            // Verify scan statistics.
            Assert.Equal(3, summary.Total);
            Assert.Equal(3, summary.Added);
            Assert.Equal(0, summary.Updated);
            Assert.Equal(0, summary.Skipped);
            Assert.Equal(0, summary.Failed);

            // Verify each file was scanned exactly once.
            Assert.Equal(3, metadataReader.ReadCount);

            // Verify all tracks were persisted with fingerprints.
            foreach (string filePath in filePaths)
            {
                MediaTrackRecord? track = repository.GetByFilePath(filePath);

                Assert.NotNull(track);
                Assert.Equal(sourceId, track.SourceId);
                Assert.Equal("Test Track", track.Title);
                Assert.Equal("Test Artist", track.Artist);
                Assert.NotNull(track.FileSize);
                Assert.NotNull(track.LastModifiedUtc);
            }
        }
        finally
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }


    [Fact]
    public void ProcessFiles_ReportsCorrectSummary_WhenFilesHaveMixedStatuses()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string directoryPath = Path.Combine(Path.GetTempPath(), $"incremental-files-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            string newFilePath = Path.Combine(directoryPath, "new-track.mp3");
            string modifiedFilePath = Path.Combine(directoryPath, "modified-track.mp3");
            string unchangedFilePath = Path.Combine(directoryPath, "unchanged-track.mp3");

            string[] filePaths =
            [
                newFilePath,
                modifiedFilePath,
                unchangedFilePath
            ];

            foreach (string filePath in filePaths)
            {
                File.WriteAllText(filePath, "Test");
            }

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = directoryPath,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service =
                new IncrementalScanService(
                    repository,
                    fingerprintProvider,
                    changeDetector,
                    persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            // Establish existing records for modified and unchanged files.
            MediaScanResult? modifiedInitialResult = service.ProcessFile(
                modifiedFilePath,
                sourceId,
                metadataReader);

            MediaScanResult? unchangedInitialResult = service.ProcessFile(
                unchangedFilePath,
                sourceId,
                metadataReader);

            Assert.NotNull(modifiedInitialResult);
            Assert.True(modifiedInitialResult.IsSuccess);

            Assert.NotNull(unchangedInitialResult);
            Assert.True(unchangedInitialResult.IsSuccess);

            Assert.Equal(2, metadataReader.ReadCount);

            MediaTrackRecord? originalModifiedTrack = repository.GetByFilePath(modifiedFilePath);

            Assert.NotNull(originalModifiedTrack);

            long originalModifiedTrackId = originalModifiedTrack.Id;
            long originalModifiedFileSize = originalModifiedTrack.FileSize!.Value;

            // Change one file while leaving the other unchanged.
            File.WriteAllText(modifiedFilePath, "Test - Modified");

            IncrementalScanSummary summary = service.ProcessFiles(
                filePaths,
                sourceId,
                metadataReader);

            // Statistics
            Assert.Equal(3, summary.Total);
            Assert.Equal(1, summary.Added);
            Assert.Equal(1, summary.Updated);
            Assert.Equal(1, summary.Skipped);
            Assert.Equal(0, summary.Failed);

            // Two reads during setup, two additional reads during the batch.
            Assert.Equal(4, metadataReader.ReadCount);

            // New track inserted
            MediaTrackRecord? newTrack = repository.GetByFilePath(newFilePath);

            Assert.NotNull(newTrack);
            Assert.NotNull(newTrack.FileSize);
            Assert.NotNull(newTrack.LastModifiedUtc);

            // Modified track updated without changing its ID
            MediaTrackRecord? updatedTrack = repository.GetByFilePath(modifiedFilePath);

            Assert.NotNull(updatedTrack);
            Assert.Equal(originalModifiedTrackId, updatedTrack.Id);
            Assert.NotEqual(originalModifiedFileSize, updatedTrack.FileSize);

            FileFingerprint updatedFingerprint = fingerprintProvider.GetFingerprint(modifiedFilePath);

            Assert.Equal(updatedFingerprint.FileSize, updatedTrack.FileSize);
            Assert.Equal(updatedFingerprint.LastModifiedUtc, updatedTrack.LastModifiedUtc);

            // Unchanged track still exists
            MediaTrackRecord? unchangedTrack = repository.GetByFilePath(unchangedFilePath);

            Assert.NotNull(unchangedTrack);
        }
        finally
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }


    [Fact]
    public void ProcessFiles_ContinuesProcessing_WhenFileFails()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-scan-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        string directoryPath = Path.Combine(Path.GetTempPath(), $"incremental-files-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directoryPath);

        try
        {
            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

            initializer.Initialize();

            string firstFilePath = Path.Combine(directoryPath, "track1.mp3");
            string missingFilePath = Path.Combine(directoryPath, "missing.mp3");
            string secondFilePath = Path.Combine(directoryPath, "track2.mp3");

            File.WriteAllText(firstFilePath, "Test 1");
            File.WriteAllText(secondFilePath, "Test 2");

            // Deliberately do not create missing.mp3.
            string[] filePaths =
            [
                firstFilePath,
                missingFilePath,
                secondFilePath
            ];

            LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = directoryPath,
                Type = LibrarySourceType.Music,
                Enabled = true
            });

            MediaTrackRepository repository = new MediaTrackRepository(connectionString);

            FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

            FileChangeDetector changeDetector = new FileChangeDetector();

            ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

            IncrementalScanService service =
                new IncrementalScanService(
                    repository,
                    fingerprintProvider,
                    changeDetector,
                    persistenceService);

            CountingMetadataReader metadataReader = new CountingMetadataReader();

            IncrementalScanSummary summary = service.ProcessFiles(
                filePaths,
                sourceId,
                metadataReader);

            // Statistics
            Assert.Equal(3, summary.Total);
            Assert.Equal(2, summary.Added);
            Assert.Equal(0, summary.Updated);
            Assert.Equal(0, summary.Skipped);
            Assert.Equal(1, summary.Failed);

            // Only the two existing files should have their metadata read.
            Assert.Equal(2, metadataReader.ReadCount);

            // Both valid files were persisted.
            MediaTrackRecord? firstTrack = repository.GetByFilePath(firstFilePath);

            MediaTrackRecord? secondTrack = repository.GetByFilePath(secondFilePath);

            Assert.NotNull(firstTrack);
            Assert.NotNull(secondTrack);

            Assert.NotNull(firstTrack.FileSize);
            Assert.NotNull(firstTrack.LastModifiedUtc);

            Assert.NotNull(secondTrack.FileSize);
            Assert.NotNull(secondTrack.LastModifiedUtc);

            // Assert - Missing file was not persisted.
            MediaTrackRecord? missingTrack = repository.GetByFilePath(missingFilePath);

            Assert.Null(missingTrack);

            // Verify the summary accounts for every input file.
            Assert.Equal(
                summary.Total,
                summary.Added + summary.Updated + summary.Skipped + summary.Failed);
        }
        finally
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

}
