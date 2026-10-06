using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests
{
    public sealed class MediaLibraryDatabaseTestContext : IDisposable
    {
        public string DatabasePath { get; }

        public string ConnectionString { get; }

        public MediaLibraryDatabaseTestContext()
        {
            DatabasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

            ConnectionString = $"Data Source={DatabasePath};Pooling=False";

            MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(ConnectionString);

            initializer.Initialize();
        }

        public SqliteConnection CreateConnection()
        {
            SqliteConnection connection = new SqliteConnection(ConnectionString);

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = "PRAGMA foreign_keys = ON;";

            command.ExecuteNonQuery();

            return connection;
        }

        public long InsertLibrarySource(
            SqliteConnection connection,
            string path = @"D:\Music",
            int type = 0,
            int enabled = 1)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO LibrarySources (Path, Type, Enabled)
                VALUES ($path, $type, $enabled);

                SELECT last_insert_rowid();
                """;

            command.Parameters.AddWithValue("$path", path);
            command.Parameters.AddWithValue("$type", type);
            command.Parameters.AddWithValue("$enabled", enabled);

            return (long)command.ExecuteScalar()!;
        }

        public long InsertTrack(
            SqliteConnection connection,
            long sourceId,
            string title = "Bohemian Rhapsody",
            string artist = "Queen",
            string filePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            int durationSeconds = 354,
            int isKaraoke = 0)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO Tracks (
                    SourceId,
                    Title,
                    Artist,
                    FilePath,
                    DurationSeconds,
                    IsKaraoke
                )
                VALUES (
                    $sourceId,
                    $title,
                    $artist,
                    $filePath,
                    $durationSeconds,
                    $isKaraoke
                );

                SELECT last_insert_rowid();
                """;

            command.Parameters.AddWithValue("$sourceId", sourceId);
            command.Parameters.AddWithValue("$title", title);
            command.Parameters.AddWithValue("$artist", artist);
            command.Parameters.AddWithValue("$filePath", filePath);
            command.Parameters.AddWithValue("$durationSeconds", durationSeconds);
            command.Parameters.AddWithValue("$isKaraoke", isKaraoke);

            return (long)command.ExecuteScalar()!;
        }

        public void InsertKaraokeFile(
            SqliteConnection connection,
            long trackId,
            string filePath = @"D:\Karaoke\Queen - Bohemian Rhapsody.cdg")
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO KaraokeFiles (TrackId, FilePath)
                VALUES ($trackId, $filePath);
                """;

            command.Parameters.AddWithValue("$trackId", trackId);
            command.Parameters.AddWithValue("$filePath", filePath);

            command.ExecuteNonQuery();
        }

        public long InsertPlaylist(
            SqliteConnection connection,
            string guid = "11111111-1111-1111-1111-111111111111",
            string name = "Test Playlist")
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO Playlists (Guid, Name)
                VALUES ($guid, $name);

                SELECT last_insert_rowid();
                """;

            command.Parameters.AddWithValue("$guid", guid);
            command.Parameters.AddWithValue("$name", name);

            return (long)command.ExecuteScalar()!;
        }

        public long InsertPlaylistTrack(
            SqliteConnection connection,
            long playlistId,
            long trackId,
            int position)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO PlaylistTracks (
                    PlaylistId,
                    TrackId,
                    Position
                )
                VALUES (
                    $playlistId,
                    $trackId,
                    $position
                );

                SELECT last_insert_rowid();
                """;

            command.Parameters.AddWithValue("$playlistId", playlistId);
            command.Parameters.AddWithValue("$trackId", trackId);
            command.Parameters.AddWithValue("$position", position);

            return (long)command.ExecuteScalar()!;
        }

        public long InsertPlaybackHistory(
            SqliteConnection connection,
            long trackId,
            string playedAt = "2026-01-01T12:00:00+00:00")
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO PlaybackHistory (TrackId, PlayedAt)
                VALUES ($trackId, $playedAt);

                SELECT last_insert_rowid();
                """;

            command.Parameters.AddWithValue("$trackId", trackId);
            command.Parameters.AddWithValue("$playedAt", playedAt);

            return (long)command.ExecuteScalar()!;
        }

        public long ExecuteCount(
            SqliteConnection connection,
            string sql,
            string parameterName,
            long parameterValue)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = sql;
            command.Parameters.AddWithValue(parameterName, parameterValue);

            return (long)command.ExecuteScalar()!;
        }

        public void Dispose()
        {
            if (File.Exists(DatabasePath))
            {
                File.Delete(DatabasePath);
            }
        }
    }
}
