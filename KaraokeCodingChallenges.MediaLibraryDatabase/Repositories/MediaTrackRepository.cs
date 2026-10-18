using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories
{
    public class MediaTrackRepository : IMediaTrackRepository
    {
        private readonly string _connectionString;

        public MediaTrackRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static MediaTrackRecord MapMediaTrack(SqliteDataReader reader)
        {
            return new MediaTrackRecord
            {
                Id = reader.GetInt64(0),
                SourceId = reader.GetInt64(1),
                Title = reader.GetString(2),
                Artist = reader.GetString(3),
                FilePath = reader.GetString(4),
                DurationSeconds = reader.GetInt32(5),
                IsKaraoke = reader.GetInt32(6) == 1,
                FileSize = reader.IsDBNull(7)
                    ? null
                    : reader.GetInt64(7),
                LastModifiedUtc = reader.IsDBNull(8)
                    ? null
                    : DateTime.ParseExact(
                        reader.GetString(8),
                        "o",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.RoundtripKind),
                IsAvailable = reader.GetInt32(9) == 1
            };
        }

        public long Add(MediaTrackRecord track)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            INSERT INTO Tracks (
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            )
            VALUES (
                $sourceId,
                $title,
                $artist,
                $filePath,
                $durationSeconds,
                $isKaraoke,
                $fileSize,
                $lastModifiedUtc,
                $isAvailable
            );

            SELECT last_insert_rowid();
            """;

            command.Parameters.AddWithValue("$sourceId", track.SourceId);
            command.Parameters.AddWithValue("$title", track.Title);
            command.Parameters.AddWithValue("$artist", track.Artist);
            command.Parameters.AddWithValue("$filePath", track.FilePath);
            command.Parameters.AddWithValue("$durationSeconds", track.DurationSeconds);
            command.Parameters.AddWithValue("$isKaraoke", track.IsKaraoke ? 1 : 0);
            command.Parameters.AddWithValue("$fileSize", (object?)track.FileSize ?? DBNull.Value);
            command.Parameters.AddWithValue("$lastModifiedUtc", (object?)track.LastModifiedUtc?.ToString("o") ?? DBNull.Value);
            command.Parameters.AddWithValue("$isAvailable", track.IsAvailable ? 1 : 0);

            return (long)command.ExecuteScalar()!;
        }

        public long Add(MediaTrackRecord track, SqliteConnection connection, SqliteTransaction transaction)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = """
            INSERT INTO Tracks (
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            )
            VALUES (
                $sourceId,
                $title,
                $artist,
                $filePath,
                $durationSeconds,
                $isKaraoke,
                $fileSize,
                $lastModifiedUtc,
                $isAvailable
            );

            SELECT last_insert_rowid();
            """;

            command.Parameters.AddWithValue("$sourceId", track.SourceId);
            command.Parameters.AddWithValue("$title", track.Title);
            command.Parameters.AddWithValue("$artist", track.Artist);
            command.Parameters.AddWithValue("$filePath", track.FilePath);
            command.Parameters.AddWithValue("$durationSeconds", track.DurationSeconds);
            command.Parameters.AddWithValue("$isKaraoke", track.IsKaraoke ? 1 : 0);
            command.Parameters.AddWithValue("$fileSize", (object?)track.FileSize ?? DBNull.Value);
            command.Parameters.AddWithValue("$lastModifiedUtc", (object?)track.LastModifiedUtc?.ToString("o") ?? DBNull.Value);
            command.Parameters.AddWithValue("$isAvailable", track.IsAvailable ? 1 : 0);

            return (long)command.ExecuteScalar()!;
        }

        public MediaTrackRecord? GetById(long id)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            SELECT
                Id,
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            FROM Tracks
            WHERE Id = $id;
            """;

            command.Parameters.AddWithValue("$id", id);

            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapMediaTrack(reader);
        }

        public IReadOnlyList<MediaTrackRecord> GetBySourceId(long sourceId)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            SELECT
                Id,
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            FROM Tracks
            WHERE SourceId = $sourceId
            ORDER BY Id;
            """;

            command.Parameters.AddWithValue("$sourceId", sourceId);

            using SqliteDataReader reader = command.ExecuteReader();

            List<MediaTrackRecord> tracks = new();

            while (reader.Read())
            {
                tracks.Add(MapMediaTrack(reader));
            }

            return tracks;
        }

        public MediaTrackRecord? GetByFilePath(string filePath)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            SELECT
                Id,
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            FROM Tracks
            WHERE FilePath = $filePath;
            """;

            command.Parameters.AddWithValue("$filePath", filePath);

            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapMediaTrack(reader);
        }

        public MediaTrackRecord? GetByFilePath(string filePath, SqliteConnection connection, SqliteTransaction transaction)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = """
            SELECT
                Id,
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            FROM Tracks
            WHERE FilePath = $filePath;
            """;

            command.Parameters.AddWithValue("$filePath", filePath);

            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapMediaTrack(reader);
        }

        public IReadOnlyList<MediaTrackRecord> GetAll()
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            SELECT
                Id,
                SourceId,
                Title,
                Artist,
                FilePath,
                DurationSeconds,
                IsKaraoke,
                FileSize,
                LastModifiedUtc,
                IsAvailable
            FROM Tracks
            ORDER BY Id;
            """;

            using SqliteDataReader reader = command.ExecuteReader();

            List<MediaTrackRecord> tracks = new List<MediaTrackRecord>();

            while (reader.Read())
            {
                tracks.Add(MapMediaTrack(reader));
            }

            return tracks;
        }

        public bool Update(MediaTrackRecord track)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            UPDATE Tracks
            SET
                SourceId = $sourceId,
                Title = $title,
                Artist = $artist,
                FilePath = $filePath,
                DurationSeconds = $durationSeconds,
                IsKaraoke = $isKaraoke,
                FileSize = COALESCE($fileSize, FileSize),
                LastModifiedUtc = COALESCE($lastModifiedUtc, LastModifiedUtc),
                IsAvailable = $isAvailable
            WHERE Id = $id;
            """;

            command.Parameters.AddWithValue("$id", track.Id);
            command.Parameters.AddWithValue("$sourceId", track.SourceId);
            command.Parameters.AddWithValue("$title", track.Title);
            command.Parameters.AddWithValue("$artist", track.Artist);
            command.Parameters.AddWithValue("$filePath", track.FilePath);
            command.Parameters.AddWithValue("$durationSeconds", track.DurationSeconds);
            command.Parameters.AddWithValue("$isKaraoke", track.IsKaraoke ? 1 : 0);
            command.Parameters.AddWithValue("$fileSize", (object?)track.FileSize ?? DBNull.Value);
            command.Parameters.AddWithValue("$lastModifiedUtc", (object?)track.LastModifiedUtc?.ToString("o") ?? DBNull.Value);
            command.Parameters.AddWithValue("$isAvailable", track.IsAvailable ? 1 : 0);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Update(MediaTrackRecord track, SqliteConnection connection, SqliteTransaction transaction)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = """
            UPDATE Tracks
            SET
                SourceId = $sourceId,
                Title = $title,
                Artist = $artist,
                FilePath = $filePath,
                DurationSeconds = $durationSeconds,
                IsKaraoke = $isKaraoke,
                FileSize = COALESCE($fileSize, FileSize),
                LastModifiedUtc = COALESCE($lastModifiedUtc, LastModifiedUtc),
                IsAvailable = $isAvailable
            WHERE Id = $id;
            """;

            command.Parameters.AddWithValue("$id", track.Id);
            command.Parameters.AddWithValue("$sourceId", track.SourceId);
            command.Parameters.AddWithValue("$title", track.Title);
            command.Parameters.AddWithValue("$artist", track.Artist);
            command.Parameters.AddWithValue("$filePath", track.FilePath);
            command.Parameters.AddWithValue("$durationSeconds", track.DurationSeconds);
            command.Parameters.AddWithValue("$isKaraoke", track.IsKaraoke ? 1 : 0);
            command.Parameters.AddWithValue("$fileSize", (object?)track.FileSize ?? DBNull.Value);
            command.Parameters.AddWithValue("$lastModifiedUtc", (object?)track.LastModifiedUtc?.ToString("o") ?? DBNull.Value);
            command.Parameters.AddWithValue("$isAvailable", track.IsAvailable ? 1 : 0);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(long id)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
            DELETE FROM Tracks
            WHERE Id = $id;
            """;

            command.Parameters.AddWithValue("$id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
    }
}
