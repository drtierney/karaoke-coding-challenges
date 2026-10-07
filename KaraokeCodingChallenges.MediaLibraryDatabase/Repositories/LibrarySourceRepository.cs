using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

public class LibrarySourceRepository : ILibrarySourceRepository
{
    private readonly string _connectionString;

    public LibrarySourceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private static LibrarySourceRecord MapLibrarySource(SqliteDataReader reader)
    {
        return new LibrarySourceRecord
        {
            Id = reader.GetInt64(0),
            Path = reader.GetString(1),
            Type = (LibrarySourceType)reader.GetInt32(2),
            Enabled = reader.GetInt32(3) == 1
        };
    }

    public long Add(LibrarySourceRecord source)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO LibrarySources (
            Path,
            Type,
            Enabled
        )
        VALUES (
            $path,
            $type,
            $enabled
        );

        SELECT last_insert_rowid();
        """;

        command.Parameters.AddWithValue("$path", source.Path);

        command.Parameters.AddWithValue("$type", (int)source.Type);

        command.Parameters.AddWithValue("$enabled", source.Enabled ? 1 : 0);

        return (long)command.ExecuteScalar()!;
    }

    public LibrarySourceRecord? GetById(long id)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            Id,
            Path,
            Type,
            Enabled
        FROM LibrarySources
        WHERE Id = $id;
        """;

        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return MapLibrarySource(reader);
    }

    public IReadOnlyList<LibrarySourceRecord> GetAll()
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            Id,
            Path,
            Type,
            Enabled
        FROM LibrarySources
        ORDER BY Id;
        """;

        using SqliteDataReader reader = command.ExecuteReader();

        List<LibrarySourceRecord> sources = new List<LibrarySourceRecord>();

        while (reader.Read())
        {
            sources.Add(MapLibrarySource(reader));
        }

        return sources;
    }

    public bool Update(LibrarySourceRecord source)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        UPDATE LibrarySources
        SET
            Path = $path,
            Type = $type,
            Enabled = $enabled
        WHERE Id = $id;
        """;

        command.Parameters.AddWithValue("$id", source.Id);
        command.Parameters.AddWithValue("$path", source.Path);
        command.Parameters.AddWithValue("$type", (int)source.Type);
        command.Parameters.AddWithValue("$enabled", source.Enabled ? 1 : 0);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(long id)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        DELETE FROM LibrarySources
        WHERE Id = $id;
        """;

        command.Parameters.AddWithValue("$id", id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}
