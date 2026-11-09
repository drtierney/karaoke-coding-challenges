using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

public class KaraokeFileRepository : IKaraokeFileRepository
{
    private readonly string _connectionString;

    public KaraokeFileRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string? GetByTrackId(long trackId)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        SELECT FilePath
        FROM KaraokeFiles
        WHERE TrackId = $trackId;
        """;

        command.Parameters.AddWithValue("$trackId", trackId);

        return command.ExecuteScalar() as string;
    }

    public void Upsert(long trackId, string cdgFilePath)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO KaraokeFiles (TrackId, FilePath)
        VALUES ($trackId, $filePath)
        ON CONFLICT(TrackId)
        DO UPDATE SET FilePath = excluded.FilePath;
        """;

        command.Parameters.AddWithValue("$trackId", trackId);
        command.Parameters.AddWithValue("$filePath", cdgFilePath);

        command.ExecuteNonQuery();
    }

    public void DeleteByTrackId(long trackId)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        DELETE FROM KaraokeFiles
        WHERE TrackId = $trackId;
        """;

        command.Parameters.AddWithValue("$trackId", trackId);

        command.ExecuteNonQuery();
    }
}
