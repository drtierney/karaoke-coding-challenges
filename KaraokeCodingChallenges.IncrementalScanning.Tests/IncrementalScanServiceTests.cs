using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

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

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector);

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

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector);

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

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector);

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

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector);

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

            IncrementalScanService service = new IncrementalScanService(repository, fingerprintProvider, changeDetector);

            // Change only the file's modification timestamp.
            DateTime modifiedTimestamp = originalFingerprint.LastModifiedUtc.AddMinutes(-5);

            File.SetLastWriteTimeUtc(filePath, modifiedTimestamp);

            FileChangeStatus result = service.CheckFile(filePath);

            Assert.Equal(FileChangeStatus.Modified, result);
            Assert.Equal(originalFingerprint.FileSize,new FileInfo(filePath).Length);
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
