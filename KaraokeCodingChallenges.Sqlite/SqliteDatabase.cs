using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.Sqlite;

public class SqliteDatabase
{
    private readonly string _connectionString;

    public SqliteDatabase(string databasePath)
    {
        _connectionString = $"Data Source={databasePath};Pooling=False";
    }

    public void Initialize()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        const string sql = """
            CREATE TABLE IF NOT EXISTS Songs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Artist TEXT NOT NULL,
                Title TEXT NOT NULL,
                Year INTEGER NOT NULL
            );
            """;

        using SqliteCommand command = new(sql, connection);
        command.ExecuteNonQuery();
    }

    public int AddSong(SongRecord song)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        const string sql = """
        INSERT INTO Songs (Artist, Title, Year)
        VALUES (@Artist, @Title, @Year);

        SELECT last_insert_rowid();
        """;

        using SqliteCommand command = new(sql, connection);

        command.Parameters.AddWithValue("@Artist", song.Artist);
        command.Parameters.AddWithValue("@Title", song.Title);
        command.Parameters.AddWithValue("@Year", song.Year);

        long id = (long)command.ExecuteScalar()!;

        return (int)id;
    }

    public SongRecord? GetSong(int id)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        const string sql = """
            SELECT Id, Artist, Title, Year
            FROM Songs
            WHERE Id = @Id;
        """;

        using SqliteCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new SongRecord
            {
                Id = reader.GetInt32(0),
                Artist = reader.GetString(1),
                Title = reader.GetString(2),
                Year = reader.GetInt32(3)
            };
        }

        return null;
    }

    public bool UpdateSong(SongRecord song)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        const string sql = """
        UPDATE Songs
        SET Artist = @Artist,
            Title = @Title,
            Year = @Year
        WHERE Id = @Id;
        """;

        using SqliteCommand command = new(sql, connection);

        command.Parameters.AddWithValue("@Id", song.Id);
        command.Parameters.AddWithValue("@Artist", song.Artist);
        command.Parameters.AddWithValue("@Title", song.Title);
        command.Parameters.AddWithValue("@Year", song.Year);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool DeleteSong(int id)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        const string sql = """
        DELETE FROM Songs
        WHERE Id = @Id;
        """;

        using SqliteCommand command = new(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}
