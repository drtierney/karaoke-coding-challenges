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
    }
}
