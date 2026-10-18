
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class LibraryReconciliationPersistenceServiceTests
{
    [Fact]
    public void Apply_ThrowsInvalidOperationException_WhenScanFailed()
    {
        LibraryReconciliationResult result = new()
        {
            SourceId = 1,
            StoredTrackCount = 2,
            ScannedTrackCount = 0,
            MissingTracks = [],
            PresentTracks = [],
            ScanCompletedSuccessfully = false
        };

        LibraryReconciliationPersistenceService service = new("Data Source=:memory:");

        Assert.Throws<InvalidOperationException>(() => service.Apply(result));
    }

    [Fact]
    public void Apply_MarksMissingTracksUnavailable_WhenScanSucceeded()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = @"D:\Karaoke",
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long queenId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Karaoke\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = true
            });

            long museId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Feeling Good",
                Artist = "Muse",
                FilePath = @"D:\Karaoke\Muse.mp3",
                DurationSeconds = 218,
                IsKaraoke = true
            });

            MediaTrackRecord? queenRecord = trackRepository.GetById(queenId);
            MediaTrackRecord? museRecord = trackRepository.GetById(museId);

            Assert.NotNull(queenRecord);
            Assert.NotNull(museRecord);

            LibraryReconciliationResult result = new()
            {
                SourceId = sourceId,
                StoredTrackCount = 2,
                ScannedTrackCount = 1,
                MissingTracks = [museRecord],
                PresentTracks = [queenRecord],
                ScanCompletedSuccessfully = true
            };

            LibraryReconciliationPersistenceService service = new(connectionString);

            service.Apply(result);

            MediaTrackRecord? queenAfter = trackRepository.GetById(queenId);
            MediaTrackRecord? museAfter = trackRepository.GetById(museId);

            Assert.NotNull(queenAfter);
            Assert.NotNull(museAfter);

            Assert.True(queenAfter.IsAvailable);
            Assert.False(museAfter.IsAvailable);

            Assert.Equal(queenId, queenAfter.Id);
            Assert.Equal(museId, museAfter.Id);
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
    public void Apply_RollsBackChanges_WhenTrackDoesNotBelongToSource()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long source1Id = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = @"D:\Karaoke",
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long source2Id = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = @"E:\Karaoke",
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long queenId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = source1Id,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Karaoke\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = true
            });

            long museId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = source2Id,
                Title = "Feeling Good",
                Artist = "Muse",
                FilePath = @"E:\Karaoke\Muse.mp3",
                DurationSeconds = 218,
                IsKaraoke = true
            });

            MediaTrackRecord? queenRecord = trackRepository.GetById(queenId);
            MediaTrackRecord? museRecord = trackRepository.GetById(museId);

            Assert.NotNull(queenRecord);
            Assert.NotNull(museRecord);

            LibraryReconciliationResult result = new()
            {
                SourceId = source1Id,
                StoredTrackCount = 1,
                ScannedTrackCount = 0,
                MissingTracks = [queenRecord, museRecord],
                PresentTracks = [],
                ScanCompletedSuccessfully = true
            };

            LibraryReconciliationPersistenceService service = new(connectionString);

            Assert.Throws<InvalidOperationException>(() => service.Apply(result));

            MediaTrackRecord? queenAfter = trackRepository.GetById(queenId);
            MediaTrackRecord? museAfter = trackRepository.GetById(museId);

            Assert.NotNull(queenAfter);
            Assert.NotNull(museAfter);

            Assert.True(queenAfter.IsAvailable);
            Assert.True(museAfter.IsAvailable);

            Assert.Equal(queenId, queenAfter.Id);
            Assert.Equal(museId, museAfter.Id);
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
    public void Apply_RestoresAvailability_WhenPreviouslyMissingTrackIsPresent()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = @"D:\Karaoke",
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long museId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Feeling Good",
                Artist = "Muse",
                FilePath = @"D:\Karaoke\Muse.mp3",
                DurationSeconds = 218,
                IsKaraoke = true,
                IsAvailable = false
            });

            MediaTrackRecord? museRecord = trackRepository.GetById(museId);

            Assert.NotNull(museRecord);
            Assert.False(museRecord.IsAvailable);

            LibraryReconciliationResult result = new()
            {
                SourceId = sourceId,
                StoredTrackCount = 1,
                ScannedTrackCount = 1,
                MissingTracks = [],
                PresentTracks = [museRecord],
                ScanCompletedSuccessfully = true
            };

            LibraryReconciliationPersistenceService service = new(connectionString);

            service.Apply(result);

            MediaTrackRecord? museAfter = trackRepository.GetById(museId);

            Assert.NotNull(museAfter);
            Assert.True(museAfter.IsAvailable);
            Assert.Equal(museId, museAfter.Id);
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
    public void Apply_DoesNotChangeAvailability_WhenScanFailed()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = @"D:\Karaoke",
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long queenId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Karaoke\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = true,
                IsAvailable = true
            });

            long museId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Feeling Good",
                Artist = "Muse",
                FilePath = @"D:\Karaoke\Muse.mp3",
                DurationSeconds = 218,
                IsKaraoke = true,
                IsAvailable = false
            });

            MediaTrackRecord? queenRecord = trackRepository.GetById(queenId);
            MediaTrackRecord? museRecord = trackRepository.GetById(museId);

            Assert.NotNull(queenRecord);
            Assert.NotNull(museRecord);

            LibraryReconciliationResult result = new()
            {
                SourceId = sourceId,
                StoredTrackCount = 2,
                ScannedTrackCount = 1,
                MissingTracks = [queenRecord],
                PresentTracks = [museRecord],
                ScanCompletedSuccessfully = false
            };

            LibraryReconciliationPersistenceService service = new(connectionString);

            Assert.Throws<InvalidOperationException>(() => service.Apply(result));

            // Verify the database remains unchanged
            MediaTrackRecord? queenAfter = trackRepository.GetById(queenId);
            MediaTrackRecord? museAfter = trackRepository.GetById(museId);

            Assert.NotNull(queenAfter);
            Assert.NotNull(museAfter);

            Assert.True(queenAfter.IsAvailable);
            Assert.False(museAfter.IsAvailable);

            Assert.Equal(queenId, queenAfter.Id);
            Assert.Equal(museId, museAfter.Id);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

}
