using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MediaMetadata;
using KaraokeCodingChallenges.ScanResult;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests;

public class ScannedTrackPersistenceServiceTests
{
    [Fact]
    public void Persist_WhenScanIsSuccessfulAndTrackDoesNotExist_AddsTrack()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        long? resultId = service.Persist(scanResult, sourceId, false);

        Assert.NotNull(resultId);

        MediaTrackRecord? result = repository.GetById(resultId.Value);

        Assert.NotNull(result);
        Assert.Equal(sourceId, result.SourceId);
        Assert.Equal("Bohemian Rhapsody", result.Title);
        Assert.Equal("Queen", result.Artist);
        Assert.Equal(@"D:\Music\Queen - Bohemian Rhapsody.mp3", result.FilePath);
        Assert.Equal(354, result.DurationSeconds);
        Assert.False(result.IsKaraoke);
    }

    [Fact]
    public void Persist_WhenTrackAlreadyExists_UpdatesTrack()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        MediaScanResult originalScanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        MediaScanResult updatedScanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody - Remastered",
                Artist = "Queen",
                DurationSeconds = 355
            }
        };

        long? originalId = service.Persist(originalScanResult, sourceId, false);

        long? updatedId = service.Persist(updatedScanResult, sourceId, true);

        Assert.NotNull(originalId);
        Assert.NotNull(updatedId);
        Assert.Equal(originalId, updatedId);

        MediaTrackRecord? result = repository.GetById(updatedId.Value);

        Assert.NotNull(result);
        Assert.Equal("Bohemian Rhapsody - Remastered", result.Title);
        Assert.Equal(355, result.DurationSeconds);
        Assert.True(result.IsKaraoke);
    }

    [Fact]
    public void Persist_WhenFilePathDiffersOnlyByCase_UpdatesExistingTrack()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        MediaScanResult originalScanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };
        MediaScanResult updatedScanResult = new MediaScanResult
        {
            FilePath = @"D:\MUSIC\QUEEN - BOHEMIAN RHAPSODY.MP3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\MUSIC\QUEEN - BOHEMIAN RHAPSODY.MP3",
                Title = "Bohemian Rhapsody - Remastered",
                Artist = "Queen",
                DurationSeconds = 355
            }
        };

        long? originalId = service.Persist(originalScanResult, sourceId, false);
        long? updatedId = service.Persist(updatedScanResult, sourceId, true);

        Assert.NotNull(originalId);
        Assert.NotNull(updatedId);

        Assert.Equal(originalId, updatedId);

        MediaTrackRecord? result = repository.GetById(updatedId.Value);

        Assert.NotNull(result);

        Assert.Equal("Bohemian Rhapsody - Remastered", result.Title);
        Assert.Equal(355, result.DurationSeconds);
        Assert.True(result.IsKaraoke);
    }

    [Fact]
    public void Persist_WhenScanCannotBeMapped_ReturnsNullAndDoesNotPersist()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Broken Track.mp3",
            IsSuccess = false,
            Metadata = null
        };

        long? resultId = service.Persist(scanResult, sourceId, false);

        Assert.Null(resultId);
    }

    [Fact]
    public void PersistBatch_WhenMultipleValidScansAreProvided_PersistsAllTracks()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                }
            },
            new MediaScanResult
            {
                FilePath = @"D:\Music\The Beatles - Hey Jude.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\The Beatles - Hey Jude.mp3",
                    Title = "Hey Jude",
                    Artist = "The Beatles",
                    DurationSeconds = 431
                }
            }
        ];

        IReadOnlyList<long> resultIds = service.PersistBatch(scanResults, sourceId, false);

        Assert.Equal(2, resultIds.Count);

        IReadOnlyList<MediaTrackRecord> persistedTracks = repository.GetAll();

        Assert.Equal(2, persistedTracks.Count);

        Assert.Equal("Bohemian Rhapsody", persistedTracks[0].Title);

        Assert.Equal("Hey Jude", persistedTracks[1].Title);
    }

    [Fact]
    public void PersistBatch_WhenDatabaseOperationFails_RollsBackAllChanges()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                }
            },
            new MediaScanResult
            {
                FilePath = @"D:\Music\The Beatles - Hey Jude.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\The Beatles - Hey Jude.mp3",
                    Title = "Hey Jude",
                    Artist = "The Beatles",
                    DurationSeconds = -1
                }
            }
        ];

        Assert.Throws<SqliteException>(() => service.PersistBatch(scanResults, sourceId, false));

        IReadOnlyList<MediaTrackRecord> persistedTracks = repository.GetAll();

        Assert.Empty(persistedTracks);
    }

    [Fact]
    public void PersistBatch_WhenAllDatabaseOperationsSucceed_CommitsAllChanges()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                }
            },
            new MediaScanResult
            {
                FilePath = @"D:\Music\The Beatles - Hey Jude.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\The Beatles - Hey Jude.mp3",
                    Title = "Hey Jude",
                    Artist = "The Beatles",
                    DurationSeconds = 431
                }
            }
        ];

        IReadOnlyList<long> resultIds = service.PersistBatch(scanResults, sourceId, false);

        Assert.Equal(2, resultIds.Count);

        IReadOnlyList<MediaTrackRecord> persistedTracks = repository.GetAll();

        Assert.Equal(2, persistedTracks.Count);
    }

    [Fact]
    public void PersistBatch_WhenSomeScansCannotBeMapped_PersistsValidTracksOnly()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                }
            },
            new MediaScanResult
            {
                FilePath = @"D:\Music\Broken Track.mp3",
                IsSuccess = false,
                Metadata = null
            }
        ];

        IReadOnlyList<long> resultIds = service.PersistBatch(scanResults, sourceId, false);

        Assert.Single(resultIds);

        IReadOnlyList<MediaTrackRecord> persistedTracks = repository.GetAll();

        Assert.Single(persistedTracks);
    }

    [Fact]
    public void PersistBatch_WhenTrackAlreadyExists_UpdatesTrackAndPreservesId()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

        ScannedTrackPersistenceService service = new ScannedTrackPersistenceService(repository, context.ConnectionString);

        MediaScanResult originalScanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };
        MediaScanResult updatedScanResult = new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody - Remastered",
                Artist = "Queen",
                DurationSeconds = 355
            }
        };

        long? originalId = service.Persist(originalScanResult, sourceId, false);

        IReadOnlyList<long> resultIds = service.PersistBatch(new List<MediaScanResult> { updatedScanResult }, sourceId, true);

        Assert.NotNull(originalId);
        Assert.Single(resultIds);
        Assert.Equal(originalId, resultIds[0]);

        MediaTrackRecord? result = repository.GetById(resultIds[0]);

        Assert.NotNull(result);
        Assert.Equal("Bohemian Rhapsody - Remastered", result.Title);
        Assert.Equal(355, result.DurationSeconds);
        Assert.True(result.IsKaraoke);
    }
}
