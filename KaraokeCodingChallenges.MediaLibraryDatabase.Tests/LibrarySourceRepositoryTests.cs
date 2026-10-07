using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests;

public class LibrarySourceRepositoryTests
{
    [Fact]
    public void Add_ReturnsGeneratedId()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        LibrarySourceRecord source = new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        };

        long id = repository.Add(source);

        Assert.True(id > 0);
    }

    [Fact]
    public void GetById_WhenSourceExists_ReturnsSource()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        LibrarySourceRecord source = new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        };

        long id = repository.Add(source);

        LibrarySourceRecord? result = repository.GetById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(source.Path, result.Path);
        Assert.Equal(source.Type, result.Type);
        Assert.Equal(source.Enabled, result.Enabled);
    }

    [Fact]
    public void GetById_WhenSourceDoesNotExist_ReturnsNull()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        LibrarySourceRecord? result = repository.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllSources()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        repository.Add(new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        });

        repository.Add(new LibrarySourceRecord
        {
            Path = @"D:\Karaoke",
            Type = LibrarySourceType.Karaoke,
            Enabled = false
        });

        IReadOnlyList<LibrarySourceRecord> results = repository.GetAll();

        Assert.Equal(2, results.Count);

        Assert.Equal(@"D:\Music", results[0].Path);
        Assert.Equal(LibrarySourceType.Music, results[0].Type);
        Assert.True(results[0].Enabled);

        Assert.Equal(@"D:\Karaoke", results[1].Path);
        Assert.Equal(LibrarySourceType.Karaoke, results[1].Type);
        Assert.False(results[1].Enabled);
    }

    [Fact]
    public void Update_WhenSourceExists_UpdatesSource()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        long id = repository.Add(new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        });

        LibrarySourceRecord updatedSource =
            new LibrarySourceRecord
            {
                Id = id,
                Path = @"D:\Media",
                Type = LibrarySourceType.Mixed,
                Enabled = false
            };

        bool updated = repository.Update(updatedSource);

        LibrarySourceRecord? result = repository.GetById(id);

        Assert.True(updated);
        Assert.NotNull(result);
        Assert.Equal(updatedSource.Path, result.Path);
        Assert.Equal(updatedSource.Type, result.Type);
        Assert.Equal(updatedSource.Enabled, result.Enabled);
    }

    [Fact]
    public void Update_WhenSourceDoesNotExist_ReturnsFalse()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        LibrarySourceRecord source = new LibrarySourceRecord
        {
            Id = 999,
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        };

        bool result = repository.Update(source);

        Assert.False(result);
    }

    [Fact]
    public void Delete_WhenSourceExists_ReturnsTrue()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        long id = repository.Add(new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        });

        bool deleted = repository.Delete(id);

        Assert.True(deleted);
        Assert.Null(repository.GetById(id));
    }

    [Fact]
    public void Delete_WhenSourceDoesNotExist_ReturnsFalse()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        bool result = repository.Delete(999);

        Assert.False(result);
    }

    [Fact]
    public void Add_WhenPathDiffersOnlyByCase_ThrowsSqliteException()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        repository.Add(new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        });

        LibrarySourceRecord duplicateSource =
            new LibrarySourceRecord
            {
                Path = @"D:\MUSIC",
                Type = LibrarySourceType.Music,
                Enabled = true
            };

        Assert.Throws<SqliteException>(() => repository.Add(duplicateSource));
    }

    [Fact]
    public void Delete_WhenSourceHasTracks_ThrowsSqliteException()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        LibrarySourceRepository repository = new LibrarySourceRepository(context.ConnectionString);

        long sourceId = repository.Add(new LibrarySourceRecord
        {
            Path = @"D:\Music",
            Type = LibrarySourceType.Music,
            Enabled = true
        });

        using SqliteConnection connection = context.CreateConnection();

        context.InsertTrack(connection, sourceId);

        Assert.Throws<SqliteException>(() => repository.Delete(sourceId));
    }
}
