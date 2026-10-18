using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests
{
    public class MediaTrackRepositoryTests
    {
        [Fact]
        public void Add_ReturnsGeneratedId()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            };

            long id = repository.Add(track);

            Assert.True(id > 0);
        }

        [Fact]
        public void GetById_WhenTrackExists_ReturnsTrack()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            };

            long id = repository.Add(track);

            MediaTrackRecord? result = repository.GetById(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(track.SourceId, result.SourceId);
            Assert.Equal(track.Title, result.Title);
            Assert.Equal(track.Artist, result.Artist);
            Assert.Equal(track.FilePath, result.FilePath);
            Assert.Equal(track.DurationSeconds, result.DurationSeconds);
            Assert.Equal(track.IsKaraoke, result.IsKaraoke);
        }

        [Fact]
        public void GetById_WhenTrackDoesNotExist_ReturnsNull()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord? result = repository.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetBySourceId_ReturnsOnlyTracksForSpecifiedSource()
        {
            using MediaLibraryDatabaseTestContext context = new();

            using SqliteConnection connection = context.CreateConnection();

            long source1Id = context.InsertLibrarySource(connection, @"D:\Karaoke");
            long source2Id = context.InsertLibrarySource(connection, @"E:\Karaoke");

            long queenId = context.InsertTrack(
                connection,
                source1Id,
                title: "Bohemian Rhapsody",
                artist: "Queen",
                filePath: @"D:\Karaoke\Queen.mp3");

            long museId = context.InsertTrack(
                connection,
                source1Id,
                title: "Feeling Good",
                artist: "Muse",
                filePath: @"D:\Karaoke\Muse.mp3");

            long paramoreId = context.InsertTrack(
                connection,
                source2Id,
                title: "Ain't It Fun",
                artist: "Paramore",
                filePath: @"E:\Karaoke\Paramore.mp3");

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            IReadOnlyList<MediaTrackRecord> results = repository.GetBySourceId(source1Id);

            Assert.Equal(2, results.Count);
            Assert.All(results, track => Assert.Equal(source1Id, track.SourceId));
            Assert.Equal([queenId, museId], results.Select(track => track.Id).ToArray());
            Assert.DoesNotContain(results, track => track.Id == paramoreId);
        }

        [Fact]
        public void GetBySourceId_ReturnsEmptyCollection_WhenSourceHasNoTracks()
        {
            using MediaLibraryDatabaseTestContext context = new();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            IReadOnlyList<MediaTrackRecord> results = repository.GetBySourceId(sourceId);

            Assert.Empty(results);
        }

        [Fact]
        public void GetByFilePath_WhenTrackExists_ReturnsTrack()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            };

            long id = repository.Add(track);

            MediaTrackRecord? result = repository.GetByFilePath(@"D:\Music\Queen - Bohemian Rhapsody.mp3");

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(track.SourceId, result.SourceId);
            Assert.Equal(track.Title, result.Title);
            Assert.Equal(track.Artist, result.Artist);
            Assert.Equal(track.FilePath, result.FilePath);
            Assert.Equal(track.DurationSeconds, result.DurationSeconds);
            Assert.Equal(track.IsKaraoke, result.IsKaraoke);
        }

        [Fact]
        public void GetByFilePath_WhenTrackDoesNotExist_ReturnsNull()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord? result = repository.GetByFilePath(@"D:\Music\Missing.mp3");

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ReturnsAllTracks()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            });

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Misery Business",
                Artist = "Paramore",
                FilePath = @"D:\Music\Paramore - Misery Business.mp3",
                DurationSeconds = 212,
                IsKaraoke = false
            });

            IReadOnlyList<MediaTrackRecord> results = repository.GetAll();

            Assert.Equal(2, results.Count);

            Assert.Equal("Bohemian Rhapsody", results[0].Title);
            Assert.Equal("Queen", results[0].Artist);

            Assert.Equal("Misery Business", results[1].Title);
            Assert.Equal("Paramore", results[1].Artist);
        }

        [Fact]
        public void Update_WhenTrackExists_UpdatesTrack()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            long id = repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            });

            MediaTrackRecord updatedTrack = new MediaTrackRecord
            {
                Id = id,
                SourceId = sourceId,
                Title = "Bohemian Rhapsody - Remastered",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 355,
                IsKaraoke = true
            };

            bool updated = repository.Update(updatedTrack);

            MediaTrackRecord? result = repository.GetById(id);

            Assert.True(updated);
            Assert.NotNull(result);
            Assert.Equal(updatedTrack.SourceId, result.SourceId);
            Assert.Equal(updatedTrack.Title, result.Title);
            Assert.Equal(updatedTrack.Artist, result.Artist);
            Assert.Equal(updatedTrack.FilePath, result.FilePath);
            Assert.Equal(updatedTrack.DurationSeconds, result.DurationSeconds);
            Assert.Equal(updatedTrack.IsKaraoke, result.IsKaraoke);
        }

        [Fact]
        public void Update_WhenTrackDoesNotExist_ReturnsFalse()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord track = new MediaTrackRecord
            {
                Id = 999,
                SourceId = 1,
                Title = "Missing Track",
                Artist = "Unknown",
                FilePath = @"D:\Music\Missing.mp3",
                DurationSeconds = 180,
                IsKaraoke = false
            };

            bool result = repository.Update(track);

            Assert.False(result);
        }

        [Fact]
        public void Delete_WhenTrackExists_ReturnsTrue()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            long id = repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            });

            bool deleted = repository.Delete(id);

            Assert.True(deleted);
            Assert.Null(repository.GetById(id));
        }

        [Fact]
        public void Delete_WhenTrackDoesNotExist_ReturnsFalse()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            bool result = repository.Delete(999);

            Assert.False(result);
        }

        [Fact]
        public void Add_WhenFilePathDiffersOnlyByCase_ThrowsSqliteException()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            });

            MediaTrackRecord duplicateTrack = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\MUSIC\QUEEN - BOHEMIAN RHAPSODY.MP3",
                DurationSeconds = 354,
                IsKaraoke = false
            };

            Assert.Throws<SqliteException>(() => repository.Add(duplicateTrack));
        }

        [Fact]
        public void Add_WhenSourceDoesNotExist_ThrowsSqliteException()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = 999,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            };

            Assert.Throws<SqliteException>(() => repository.Add(track));
        }

        [Fact]
        public void Add_PersistsFingerprintValues()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            DateTime lastModifiedUtc = new DateTime(2026, 10, 12, 18, 0, 0, DateTimeKind.Utc);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false,
                FileSize = 12345678,
                LastModifiedUtc = lastModifiedUtc
            };

            long id = repository.Add(track);

            MediaTrackRecord? result = repository.GetById(id);

            Assert.NotNull(result);
            Assert.Equal(track.FileSize, result.FileSize);
            Assert.Equal(track.LastModifiedUtc, result.LastModifiedUtc);
        }

        [Fact]
        public void Add_PersistsNullFingerprints_WhenValuesAreNotProvided()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false
            };

            long id = repository.Add(track);

            MediaTrackRecord? result = repository.GetById(id);

            Assert.NotNull(result);
            Assert.Null(result.FileSize);
            Assert.Null(result.LastModifiedUtc);
        }

        [Fact]
        public void Update_PersistsModifiedFingerprintValues()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            DateTime lastModifiedUtc = new DateTime(2026, 10, 12, 18, 0, 0, DateTimeKind.Utc);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false,
                FileSize = 12345678,
                LastModifiedUtc = lastModifiedUtc
            };

            long id = repository.Add(track);

            MediaTrackRecord? insertedTrack = repository.GetById(id);

            Assert.NotNull(insertedTrack);

            // Update the fingerprint values
            insertedTrack.FileSize = 87654321;
            insertedTrack.LastModifiedUtc = new DateTime(2026, 10, 13, 12, 0, 0, DateTimeKind.Utc);

            bool updated = repository.Update(insertedTrack);

            MediaTrackRecord? result = repository.GetById(id);

            Assert.True(updated);
            Assert.NotNull(result);
            Assert.Equal(insertedTrack.FileSize, result.FileSize);
            Assert.Equal(insertedTrack.LastModifiedUtc, result.LastModifiedUtc);
        }

        [Fact]
        public void Update_PreservesExistingFingerprint_WhenIncomingValuesAreNull()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            DateTime lastModifiedUtc = new DateTime(2026, 10, 12, 18, 0, 0, DateTimeKind.Utc);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false,
                FileSize = 12345678,
                LastModifiedUtc = lastModifiedUtc
            };

            long id = repository.Add(track);

            MediaTrackRecord? insertedTrack = repository.GetById(id);

            Assert.NotNull(insertedTrack);

            // Update the track without providing fingerprint values (set them to null)
            insertedTrack.FileSize = null;
            insertedTrack.LastModifiedUtc = null;

            bool updated = repository.Update(insertedTrack);

            MediaTrackRecord? result = repository.GetById(id);

            Assert.True(updated);
            Assert.NotNull(result);
            Assert.Equal(track.FileSize, result.FileSize);
            Assert.Equal(track.LastModifiedUtc, result.LastModifiedUtc);
        }

        [Fact]
        public void Add_WithTransaction_PersistsFingerprintValues()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            DateTime lastModifiedUtc = new DateTime(2026, 10, 12, 18, 0, 0, DateTimeKind.Utc);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false,
                FileSize = 12345678,
                LastModifiedUtc = lastModifiedUtc
            };

            using SqliteTransaction transaction = connection.BeginTransaction();

            long id = repository.Add(track, connection, transaction);

            transaction.Commit();

            MediaTrackRecord? result = repository.GetById(id);

            Assert.NotNull(result);
            Assert.Equal(track.FileSize, result.FileSize);
            Assert.Equal(track.LastModifiedUtc, result.LastModifiedUtc);
        }

        [Fact]
        public void Update_WithTransaction_PersistsModifiedFingerprintValues()
        {
            using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new MediaTrackRepository(context.ConnectionString);

            DateTime lastModifiedUtc = new DateTime(2026, 10, 12, 18, 0, 0, DateTimeKind.Utc);

            MediaTrackRecord track = new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
                DurationSeconds = 354,
                IsKaraoke = false,
                FileSize = 12345678,
                LastModifiedUtc = lastModifiedUtc
            };

            long id = repository.Add(track);

            MediaTrackRecord? insertedTrack = repository.GetById(id);

            Assert.NotNull(insertedTrack);

            // Update the fingerprint values
            insertedTrack.FileSize = 87654321;
            insertedTrack.LastModifiedUtc = new DateTime(2026, 10, 13, 12, 0, 0, DateTimeKind.Utc);

            using SqliteTransaction transaction = connection.BeginTransaction();

            bool updated = repository.Update(insertedTrack, connection, transaction);

            transaction.Commit();

            MediaTrackRecord? result = repository.GetById(id);

            Assert.True(updated);
            Assert.NotNull(result);
            Assert.Equal(insertedTrack.FileSize, result.FileSize);
            Assert.Equal(insertedTrack.LastModifiedUtc, result.LastModifiedUtc);
        }

        [Fact]
        public void Add_PersistsUnavailableTrack()
        {
            using MediaLibraryDatabaseTestContext context = new();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new(context.ConnectionString);

            MediaTrackRecord track = new()
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = false,
                IsAvailable = false
            };

            long trackId = repository.Add(track);

            MediaTrackRecord? result = repository.GetById(trackId);

            Assert.NotNull(result);
            Assert.Equal(trackId, result.Id);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public void Add_WithTransaction_PersistsAvailability()
        {
            using MediaLibraryDatabaseTestContext context = new();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new(context.ConnectionString);

            MediaTrackRecord track = new()
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = false,
                IsAvailable = false
            };

            long trackId;

            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                trackId = repository.Add(track, connection, transaction);

                transaction.Commit();
            }

            MediaTrackRecord? result = repository.GetById(trackId);

            Assert.NotNull(result);
            Assert.Equal(trackId, result.Id);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public void Update_ChangesTrackAvailability()
        {
            using MediaLibraryDatabaseTestContext context = new();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new(context.ConnectionString);

            MediaTrackRecord track = new()
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            };

            long trackId = repository.Add(track);

            // Verify initial availability
            MediaTrackRecord? storedTrack = repository.GetById(trackId);

            Assert.NotNull(storedTrack);
            Assert.True(storedTrack.IsAvailable);

            // Mark the track unavailable
            storedTrack.IsAvailable = false;

            bool updated = repository.Update(storedTrack);

            MediaTrackRecord? unavailableTrack = repository.GetById(trackId);

            Assert.True(updated);
            Assert.NotNull(unavailableTrack);
            Assert.False(unavailableTrack.IsAvailable);
            Assert.Equal(trackId, unavailableTrack.Id);

            // Restore availability
            unavailableTrack.IsAvailable = true;

            bool restored = repository.Update(unavailableTrack);

            MediaTrackRecord? restoredTrack = repository.GetById(trackId);

            Assert.True(restored);
            Assert.NotNull(restoredTrack);
            Assert.True(restoredTrack.IsAvailable);
            Assert.Equal(trackId, restoredTrack.Id);
        }

        [Fact]
        public void Update_WithTransaction_PersistsAvailability()
        {
            using MediaLibraryDatabaseTestContext context = new();

            using SqliteConnection connection = context.CreateConnection();

            long sourceId = context.InsertLibrarySource(connection);

            MediaTrackRepository repository = new(context.ConnectionString);

            long trackId = repository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = @"D:\Music\Queen.mp3",
                DurationSeconds = 354,
                IsKaraoke = false
            });

            MediaTrackRecord? track = repository.GetById(trackId);

            Assert.NotNull(track);

            track.IsAvailable = false;

            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                bool updated = repository.Update(
                    track,
                    connection,
                    transaction);

                Assert.True(updated);

                transaction.Commit();
            }

            MediaTrackRecord? result = repository.GetById(trackId);

            Assert.NotNull(result);
            Assert.False(result.IsAvailable);
            Assert.Equal(trackId, result.Id);
        }

    }
}
