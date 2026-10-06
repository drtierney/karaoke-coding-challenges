using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests
{
    public class MediaLibraryDatabaseInitializerTests
    {
        [Fact]
        public void Initialize_CreatesExpectedTables()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                SELECT name
                FROM sqlite_master
                WHERE type = 'table'
                ORDER BY name;
                """;

            using SqliteDataReader reader = command.ExecuteReader();

            List<string> tableNames = new List<string>();

            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }

            Assert.Contains("LibrarySources", tableNames);
            Assert.Contains("Tracks", tableNames);
            Assert.Contains("KaraokeFiles", tableNames);
            Assert.Contains("Playlists", tableNames);
            Assert.Contains("PlaylistTracks", tableNames);
            Assert.Contains("PlaybackHistory", tableNames);
        }

        [Fact]
        public void Initialize_CreatesLibrarySourcesWithExpectedColumns()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText =
                "PRAGMA table_info(LibrarySources);";

            using SqliteDataReader reader = command.ExecuteReader();

            Dictionary<string, string> columns =
                new Dictionary<string, string>();

            while (reader.Read())
            {
                string name = reader.GetString(1);
                string type = reader.GetString(2);

                columns.Add(name, type);
            }

            Assert.Equal("INTEGER", columns["Id"]);
            Assert.Equal("TEXT", columns["Path"]);
            Assert.Equal("INTEGER", columns["Type"]);
            Assert.Equal("INTEGER", columns["Enabled"]);
        }

        [Fact]
        public void Initialize_LibrarySourcesRejectsDuplicatePathIgnoringCase()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            context.InsertLibrarySource(
                connection,
                @"D:\Music");

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO LibrarySources (Path, Type, Enabled)
                VALUES ($path, $type, $enabled);
                """;

            command.Parameters.AddWithValue("$path", @"D:\MUSIC");
            command.Parameters.AddWithValue("$type", 0);
            command.Parameters.AddWithValue("$enabled", 1);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_LibrarySourcesRejectsInvalidType()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO LibrarySources (Path, Type, Enabled)
                VALUES ($path, $type, $enabled);
                """;

            command.Parameters.AddWithValue("$path", @"D:\Music");
            command.Parameters.AddWithValue("$type", 99);
            command.Parameters.AddWithValue("$enabled", 1);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_LibrarySourcesRejectsInvalidEnabledValue()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO LibrarySources (Path, Type, Enabled)
                VALUES ($path, $type, $enabled);
                """;

            command.Parameters.AddWithValue("$path", @"D:\Music");
            command.Parameters.AddWithValue("$type", 0);
            command.Parameters.AddWithValue("$enabled", 2);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_TracksRejectsNegativeDuration()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

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
                """;

            command.Parameters.AddWithValue("$sourceId", sourceId);
            command.Parameters.AddWithValue("$title", "Bohemian Rhapsody");
            command.Parameters.AddWithValue("$artist", "Queen");
            command.Parameters.AddWithValue(
                "$filePath",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");
            command.Parameters.AddWithValue("$durationSeconds", -1);
            command.Parameters.AddWithValue("$isKaraoke", 0);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_TracksRejectsInvalidIsKaraokeValue()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

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
                """;

            command.Parameters.AddWithValue("$sourceId", sourceId);
            command.Parameters.AddWithValue("$title", "Bohemian Rhapsody");
            command.Parameters.AddWithValue("$artist", "Queen");
            command.Parameters.AddWithValue(
                "$filePath",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");
            command.Parameters.AddWithValue("$durationSeconds", 354);
            command.Parameters.AddWithValue("$isKaraoke", 2);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_TracksRejectsUnknownSourceId()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

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
                """;

            command.Parameters.AddWithValue("$sourceId", 999);
            command.Parameters.AddWithValue("$title", "Bohemian Rhapsody");
            command.Parameters.AddWithValue("$artist", "Queen");
            command.Parameters.AddWithValue(
                "$filePath",
                @"D:\Music\Queen - Bohemian Rhapsody.mp3");
            command.Parameters.AddWithValue("$durationSeconds", 354);
            command.Parameters.AddWithValue("$isKaraoke", 0);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_KaraokeFilesAllowsOneFilePerTrack()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(
                    connection,
                    path: @"D:\Karaoke",
                    type: 1);

            long trackId =
                context.InsertTrack(
                    connection,
                    sourceId,
                    filePath: @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
                    isKaraoke: 1);

            context.InsertKaraokeFile(
                connection,
                trackId);

            long count = context.ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM KaraokeFiles
                WHERE TrackId = $trackId;
                """,
                "$trackId",
                trackId);

            Assert.Equal(1, count);
        }

        [Fact]
        public void Initialize_KaraokeFilesRejectsSecondFileForSameTrack()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(
                    connection,
                    path: @"D:\Karaoke",
                    type: 1);

            long trackId =
                context.InsertTrack(
                    connection,
                    sourceId,
                    filePath: @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
                    isKaraoke: 1);

            context.InsertKaraokeFile(
                connection,
                trackId,
                @"D:\Karaoke\Queen - Bohemian Rhapsody.cdg");

            Assert.Throws<SqliteException>(
                () => context.InsertKaraokeFile(
                    connection,
                    trackId,
                    @"D:\Karaoke\Queen - Bohemian Rhapsody 2.cdg"));
        }

        [Fact]
        public void Initialize_DeletingTrackDeletesKaraokeFile()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(
                    connection,
                    path: @"D:\Karaoke",
                    type: 1);

            long trackId =
                context.InsertTrack(
                    connection,
                    sourceId,
                    filePath: @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
                    isKaraoke: 1);

            context.InsertKaraokeFile(
                connection,
                trackId);

            using SqliteCommand deleteCommand =
                connection.CreateCommand();

            deleteCommand.CommandText = """
                DELETE FROM Tracks
                WHERE Id = $trackId;
                """;

            deleteCommand.Parameters.AddWithValue(
                "$trackId",
                trackId);

            deleteCommand.ExecuteNonQuery();

            long count = context.ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM KaraokeFiles
                WHERE TrackId = $trackId;
                """,
                "$trackId",
                trackId);

            Assert.Equal(0, count);
        }

        [Fact]
        public void Initialize_PlaylistTracksAllowsDuplicateTrackIds()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long trackId =
                context.InsertTrack(connection, sourceId);

            long playlistId =
                context.InsertPlaylist(connection);

            context.InsertPlaylistTrack(
                connection,
                playlistId,
                trackId,
                0);

            context.InsertPlaylistTrack(
                connection,
                playlistId,
                trackId,
                1);

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                SELECT COUNT(*)
                FROM PlaylistTracks
                WHERE PlaylistId = $playlistId
                  AND TrackId = $trackId;
                """;

            command.Parameters.AddWithValue("$playlistId", playlistId);
            command.Parameters.AddWithValue("$trackId", trackId);

            long count =
                (long)command.ExecuteScalar()!;

            Assert.Equal(2, count);
        }

        [Fact]
        public void Initialize_PlaylistTracksRejectsDuplicatePosition()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long firstTrackId =
                context.InsertTrack(
                    connection,
                    sourceId,
                    filePath: @"D:\Music\Track1.mp3");

            long secondTrackId =
                context.InsertTrack(
                    connection,
                    sourceId,
                    filePath: @"D:\Music\Track2.mp3");

            long playlistId =
                context.InsertPlaylist(connection);

            context.InsertPlaylistTrack(
                connection,
                playlistId,
                firstTrackId,
                0);

            Assert.Throws<SqliteException>(
                () => context.InsertPlaylistTrack(
                    connection,
                    playlistId,
                    secondTrackId,
                    0));
        }

        [Fact]
        public void Initialize_DeletingPlaylistDeletesPlaylistTracks()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long trackId =
                context.InsertTrack(connection, sourceId);

            long playlistId =
                context.InsertPlaylist(connection);

            context.InsertPlaylistTrack(
                connection,
                playlistId,
                trackId,
                0);

            using SqliteCommand deleteCommand =
                connection.CreateCommand();

            deleteCommand.CommandText = """
                DELETE FROM Playlists
                WHERE Id = $playlistId;
                """;

            deleteCommand.Parameters.AddWithValue(
                "$playlistId",
                playlistId);

            deleteCommand.ExecuteNonQuery();

            long count = context.ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM PlaylistTracks
                WHERE PlaylistId = $playlistId;
                """,
                "$playlistId",
                playlistId);

            Assert.Equal(0, count);
        }

        [Fact]
        public void Initialize_DeletingTrackReferencedByPlaylistIsRejected()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long trackId =
                context.InsertTrack(connection, sourceId);

            long playlistId =
                context.InsertPlaylist(connection);

            context.InsertPlaylistTrack(
                connection,
                playlistId,
                trackId,
                0);

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                DELETE FROM Tracks
                WHERE Id = $trackId;
                """;

            command.Parameters.AddWithValue(
                "$trackId",
                trackId);

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }

        [Fact]
        public void Initialize_PlaybackHistoryAllowsMultipleEntriesForTrack()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long trackId =
                context.InsertTrack(connection, sourceId);

            context.InsertPlaybackHistory(
                connection,
                trackId,
                "2026-01-01T12:00:00+00:00");

            context.InsertPlaybackHistory(
                connection,
                trackId,
                "2026-01-01T13:00:00+00:00");

            long count = context.ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM PlaybackHistory
                WHERE TrackId = $trackId;
                """,
                "$trackId",
                trackId);

            Assert.Equal(2, count);
        }

        [Fact]
        public void Initialize_PlaybackHistoryRejectsUnknownTrackId()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            Assert.Throws<SqliteException>(
                () => context.InsertPlaybackHistory(
                    connection,
                    999));
        }

        [Fact]
        public void Initialize_DeletingTrackDeletesPlaybackHistory()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long trackId =
                context.InsertTrack(connection, sourceId);

            context.InsertPlaybackHistory(
                connection,
                trackId);

            using SqliteCommand deleteCommand =
                connection.CreateCommand();

            deleteCommand.CommandText = """
                DELETE FROM Tracks
                WHERE Id = $trackId;
                """;

            deleteCommand.Parameters.AddWithValue(
                "$trackId",
                trackId);

            deleteCommand.ExecuteNonQuery();

            long count = context.ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM PlaybackHistory
                WHERE TrackId = $trackId;
                """,
                "$trackId",
                trackId);

            Assert.Equal(0, count);
        }

        [Fact]
        public void Initialize_CreatesExpectedIndexes()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                SELECT name
                FROM sqlite_master
                WHERE type = 'index'
                  AND name NOT LIKE 'sqlite_autoindex_%'
                ORDER BY name;
                """;

            using SqliteDataReader reader = command.ExecuteReader();

            List<string> indexNames = new List<string>();

            while (reader.Read())
            {
                indexNames.Add(reader.GetString(0));
            }

            Assert.Contains("IX_Tracks_SourceId", indexNames);
            Assert.Contains("IX_PlaylistTracks_PlaylistId", indexNames);
            Assert.Contains("IX_PlaylistTracks_TrackId", indexNames);
            Assert.Contains("IX_PlaybackHistory_TrackId", indexNames);
            Assert.Contains("IX_PlaybackHistory_PlayedAt", indexNames);
        }

        [Fact]
        public void Initialize_PlaylistTracksRejectsNegativePosition()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            long sourceId =
                context.InsertLibrarySource(connection);

            long trackId =
                context.InsertTrack(connection, sourceId);

            long playlistId =
                context.InsertPlaylist(connection);

            Assert.Throws<SqliteException>(
                () => context.InsertPlaylistTrack(
                    connection,
                    playlistId,
                    trackId,
                    -1));
        }

        [Fact]
        public void Initialize_PlaylistsRejectsBlankGuid()
        {
            using MediaLibraryDatabaseTestContext context =
                new MediaLibraryDatabaseTestContext();

            using SqliteConnection connection =
                context.CreateConnection();

            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO Playlists (Guid, Name)
                VALUES ($guid, $name);
                """;

            command.Parameters.AddWithValue("$guid", "   ");
            command.Parameters.AddWithValue("$name", "Test Playlist");

            Assert.Throws<SqliteException>(
                () => command.ExecuteNonQuery());
        }
    }
}
