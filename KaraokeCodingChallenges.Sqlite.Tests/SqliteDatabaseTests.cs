using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.Sqlite.Tests;

public class SqliteDatabaseTests
{
    [Fact]
    public void Initialize_CreatesDatabaseFile()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);

            database.Initialize();

            Assert.True(File.Exists(databasePath));
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void Initialize_CreatesSongsTable()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            using SqliteConnection connection = new($"Data Source={databasePath};Pooling=False");

            connection.Open();

            using SqliteCommand command = new(
                "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'Songs';",
                connection);

            using SqliteDataReader reader = command.ExecuteReader();

            Assert.True(reader.Read());
            Assert.Equal("Songs", reader.GetString(0));
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void AddSong_InsertsSong()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            SongRecord song = new()
            {
                Artist = "Test Artist",
                Title = "Test Title",
                Year = 2026
            };

            int id = database.AddSong(song);

            Assert.True(id > 0);

            using SqliteConnection connection = new($"Data Source={databasePath};Pooling=False");

            connection.Open();

            using SqliteCommand command = new(
                """
                SELECT Id, Artist, Title, Year
                FROM Songs
                WHERE Id = @Id;
                """,
                connection);

            command.Parameters.AddWithValue("@Id", id);

            using SqliteDataReader reader = command.ExecuteReader();

            Assert.True(reader.Read());
            Assert.Equal(id, reader.GetInt32(0));
            Assert.Equal(song.Artist, reader.GetString(1));
            Assert.Equal(song.Title, reader.GetString(2));
            Assert.Equal(song.Year, reader.GetInt32(3));
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void GetSong_ReturnsSong()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            SongRecord song = new()
            {
                Artist = "Test Artist",
                Title = "Test Title",
                Year = 2026
            };

            int id = database.AddSong(song);

            SongRecord? retrievedSong = database.GetSong(id);

            Assert.NotNull(retrievedSong);
            Assert.Equal(id, retrievedSong!.Id);
            Assert.Equal(song.Artist, retrievedSong.Artist);
            Assert.Equal(song.Title, retrievedSong.Title);
            Assert.Equal(song.Year, retrievedSong.Year);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void GetSong_WhenSongDoesNotExist_ReturnsNull()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            SongRecord? retrievedSong = database.GetSong(999);

            Assert.Null(retrievedSong);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void UpdateSong_UpdatesExistingSong()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            SongRecord song = new()
            {
                Artist = "Test Artist",
                Title = "Test Title",
                Year = 2026
            };

            int id = database.AddSong(song);

            song.Id = id;
            song.Artist = "Updated Artist";
            song.Title = "Updated Title";
            song.Year = 2027;

            bool result = database.UpdateSong(song);

            SongRecord? updatedSong = database.GetSong(id);

            Assert.True(result);
            Assert.NotNull(updatedSong);
            Assert.Equal(id, updatedSong!.Id);
            Assert.Equal("Updated Artist", updatedSong.Artist);
            Assert.Equal("Updated Title", updatedSong.Title);
            Assert.Equal(2027, updatedSong.Year);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void UpdateSong_WhenSongDoesNotExist_ReturnsFalse()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            SongRecord song = new()
            {
                Id = 999,
                Artist = "Nonexistent Artist",
                Title = "Nonexistent Title",
                Year = 2028
            };

            bool result = database.UpdateSong(song);

            Assert.False(result);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void DeleteSong_DeletesExistingSong()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            SongRecord song = new()
            {
                Artist = "Test Artist",
                Title = "Test Title",
                Year = 2026
            };

            int id = database.AddSong(song);

            bool result = database.DeleteSong(id);

            SongRecord? deletedSong = database.GetSong(id);

            Assert.True(result);
            Assert.Null(deletedSong);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Fact]
    public void DeleteSong_WhenSongDoesNotExist_ReturnsFalse()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SqliteDatabase database = new(databasePath);
            database.Initialize();

            bool result = database.DeleteSong(999);

            Assert.False(result);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    private static string CreateDatabasePath()
    {
        return Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
    }

    private static void DeleteDatabase(string databasePath)
    {
        if (File.Exists(databasePath))
        {
            File.Delete(databasePath);
        }
    }
}
