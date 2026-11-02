
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests;

public class DatabasePersistenceIntegrationTests
{
    [Fact]
    public void PersistedTrack_WhenConnectionIsReopened_RetainsData()
    {
        using MediaLibraryDatabaseTestContext context = new();

        long sourceId;

        using (SqliteConnection connection = context.CreateConnection())
        {
            sourceId = context.InsertLibrarySource(connection);
        }

        MediaTrackRecord track = new()
        {
            SourceId = sourceId,
            Title = "Bohemian Rhapsody",
            Artist = "Queen",
            FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
            DurationSeconds = 354,
            IsKaraoke = false
        };

        MediaTrackRepository repository = new(context.ConnectionString);

        long trackId = repository.Add(track);

        // Verify persistence through a new repository instance.
        MediaTrackRepository reopenedRepository = new(context.ConnectionString);

        MediaTrackRecord? persistedTrack = reopenedRepository.GetByFilePath(track.FilePath);

        Assert.NotNull(persistedTrack);

        Assert.Equal(trackId, persistedTrack.Id);
        Assert.Equal(track.SourceId, persistedTrack.SourceId);
        Assert.Equal(track.Title, persistedTrack.Title);
        Assert.Equal(track.Artist, persistedTrack.Artist);
        Assert.Equal(track.FilePath, persistedTrack.FilePath);
        Assert.Equal(track.DurationSeconds, persistedTrack.DurationSeconds);
        Assert.Equal(track.IsKaraoke, persistedTrack.IsKaraoke);
    }

    [Fact]
    public void UpdatedTrack_WhenConnectionIsReopened_RetainsChanges()
    {
        using MediaLibraryDatabaseTestContext context = new();

        long sourceId;

        using (SqliteConnection connection = context.CreateConnection())
        {
            sourceId = context.InsertLibrarySource(connection);
        }

        MediaTrackRecord track = new()
        {
            SourceId = sourceId,
            Title = "Bohemian Rhapsody",
            Artist = "Queen",
            FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
            DurationSeconds = 354,
            IsKaraoke = false
        };

        MediaTrackRepository repository = new(context.ConnectionString);

        long trackId = repository.Add(track);

        MediaTrackRecord updatedTrack = track with
        {
            Id = trackId,
            Title = "Bohemian Rhapsody - Live",
            DurationSeconds = 378
        };

        MediaTrackRepository updateRepository = new(context.ConnectionString);

        bool updated = updateRepository.Update(updatedTrack);

        // Verify the updated record through a new repository instance.
        MediaTrackRepository verificationRepository = new(context.ConnectionString);

        MediaTrackRecord? persistedTrack = verificationRepository.GetById(trackId);

        Assert.True(updated);
        Assert.NotNull(persistedTrack);

        Assert.Equal(trackId, persistedTrack.Id);
        Assert.Equal(track.SourceId, persistedTrack.SourceId);
        Assert.Equal(updatedTrack.Title, persistedTrack.Title);
        Assert.Equal(track.Artist, persistedTrack.Artist);
        Assert.Equal(track.FilePath, persistedTrack.FilePath);
        Assert.Equal(updatedTrack.DurationSeconds, persistedTrack.DurationSeconds);
        Assert.Equal(track.IsKaraoke, persistedTrack.IsKaraoke);
    }

    [Fact]
    public void UpdateMultipleTracks_WhenTransactionFails_RollsBackAllChanges()
    {
        using MediaLibraryDatabaseTestContext context = new();

        long sourceId;

        using (SqliteConnection connection = context.CreateConnection())
        {
            sourceId = context.InsertLibrarySource(connection);
        }

        MediaTrackRecord trackOne = new()
        {
            SourceId = sourceId,
            Title = "Bohemian Rhapsody",
            Artist = "Queen",
            FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
            DurationSeconds = 354,
            IsKaraoke = false
        };

        MediaTrackRecord trackTwo = new()
        {
            SourceId = sourceId,
            Title = "Don't Stop Me Now",
            Artist = "Queen",
            FilePath = Path.Combine("Music", "Queen - Don't Stop Me Now.mp3"),
            DurationSeconds = 201,
            IsKaraoke = false
        };

        MediaTrackRepository repository = new(context.ConnectionString);

        long trackOneId = repository.Add(trackOne);
        long trackTwoId = repository.Add(trackTwo);

        MediaTrackRecord updatedTrackOne = trackOne with
        {
            Id = trackOneId,
            DurationSeconds = 1
        };

        MediaTrackRecord updatedTrackTwo = trackTwo with
        {
            Id = trackTwoId,
            DurationSeconds = -1
        };

        bool firstUpdateSucceeded;

        using (SqliteConnection connection = context.CreateConnection())
        {
            using SqliteTransaction transaction = connection.BeginTransaction();

            // First update succeeds within the transaction.
            firstUpdateSucceeded = repository.Update(
                updatedTrackOne,
                connection,
                transaction);

            MediaTrackRecord? trackDuringTransaction =
                repository.GetByFilePath(
                    updatedTrackOne.FilePath,
                    connection,
                    transaction);

            Assert.True(firstUpdateSucceeded);
            Assert.NotNull(trackDuringTransaction);
            Assert.Equal(1, trackDuringTransaction.DurationSeconds);

            // Second update violates the database duration constraint.
            Assert.Throws<SqliteException>(() =>
                repository.Update(updatedTrackTwo, connection, transaction));

            // Roll back both updates.
            transaction.Rollback();
        }

        // Verify the original values remain after rollback.
        MediaTrackRecord? persistedTrackOne = repository.GetById(trackOneId);
        MediaTrackRecord? persistedTrackTwo = repository.GetById(trackTwoId);

        Assert.NotNull(persistedTrackOne);
        Assert.NotNull(persistedTrackTwo);

        Assert.Equal(trackOne.DurationSeconds, persistedTrackOne.DurationSeconds);
        Assert.Equal(trackTwo.DurationSeconds, persistedTrackTwo.DurationSeconds);
    }


    [Fact]
    public void TemporaryDatabases_WhenContextsAreDisposed_RemainIsolatedAndCleanUp()
    {
        string pathOne;
        string pathTwo;

        using (MediaLibraryDatabaseTestContext contextOne = new())
        using (MediaLibraryDatabaseTestContext contextTwo = new())
        {
            pathOne = contextOne.DatabasePath;
            pathTwo = contextTwo.DatabasePath;

            Assert.NotEqual(pathOne, pathTwo);

            long sourceId;

            using (SqliteConnection connection = contextOne.CreateConnection())
            {
                sourceId = contextOne.InsertLibrarySource(connection);
            }

            MediaTrackRecord track = new()
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false
            };

            MediaTrackRepository repositoryOne = new(contextOne.ConnectionString);
            MediaTrackRepository repositoryTwo = new(contextTwo.ConnectionString);

            long trackId = repositoryOne.Add(track);

            // Verify the track exists in the first database.
            MediaTrackRecord? trackInFirstDatabase = repositoryOne.GetById(trackId);

            Assert.NotNull(trackInFirstDatabase);

            // Verify the second database remains isolated.
            MediaTrackRecord? trackInSecondDatabase = repositoryTwo.GetById(trackId);

            Assert.Null(trackInSecondDatabase);

            // Verify both database files exist before disposal.
            Assert.True(File.Exists(pathOne));
            Assert.True(File.Exists(pathTwo));
        }

        // Verify both temporary database files were deleted after disposal.
        Assert.False(File.Exists(pathOne));
        Assert.False(File.Exists(pathTwo));
    }

}
