using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using Microsoft.Data.Sqlite;

string databasePath = Path.Combine(Path.GetTempPath(), "karaoke-media-library-demo.db");

if (File.Exists(databasePath))
{
    File.Delete(databasePath);
}

string connectionString = $"Data Source={databasePath};Pooling=False";

MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

Console.WriteLine("Media Library Database Demo");
Console.WriteLine();

initializer.Initialize();

Console.WriteLine("Database initialized at:");
Console.WriteLine(databasePath);
Console.WriteLine();

using SqliteConnection connection = new SqliteConnection(connectionString);

connection.Open();

using (SqliteCommand foreignKeysCommand = connection.CreateCommand())
{
    foreignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";

    foreignKeysCommand.ExecuteNonQuery();
}

using (SqliteCommand foreignKeysEnabledCommand = connection.CreateCommand())
{
    foreignKeysEnabledCommand.CommandText = "PRAGMA foreign_keys;";

    long foreignKeysEnabled = (long)foreignKeysEnabledCommand.ExecuteScalar()!;

    Console.WriteLine($"Foreign key enforcement: {(foreignKeysEnabled == 1 ? "Enabled" : "Disabled")}");

    Console.WriteLine();
}

Console.WriteLine("Tables");

using (SqliteCommand tableCommand = connection.CreateCommand())
{
    tableCommand.CommandText = """
        SELECT name
        FROM sqlite_master
        WHERE type = 'table'
          AND name NOT LIKE 'sqlite_%'
        ORDER BY name;
        """;

    using SqliteDataReader reader = tableCommand.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine($"- {reader.GetString(0)}");
    }
}

Console.WriteLine();
Console.WriteLine("Indexes");

using (SqliteCommand indexCommand = connection.CreateCommand())
{
    indexCommand.CommandText = """
        SELECT name
        FROM sqlite_master
        WHERE type = 'index'
          AND name NOT LIKE 'sqlite_autoindex_%'
        ORDER BY name;
        """;

    using SqliteDataReader reader = indexCommand.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine($"- {reader.GetString(0)}");
    }
}

Console.WriteLine();
Console.WriteLine("Foreign Keys");

PrintForeignKeys(connection, "Tracks");
PrintForeignKeys(connection, "KaraokeFiles");
PrintForeignKeys(connection, "PlaylistTracks");
PrintForeignKeys(connection, "PlaybackHistory");

Console.WriteLine();
Console.WriteLine("Schema initialization complete.");

Console.WriteLine();
Console.WriteLine("Database Data Access Layer");
Console.WriteLine();

LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

MediaTrackRepository trackRepository = new MediaTrackRepository(connectionString);

long musicSourceId =
    sourceRepository.Add(new LibrarySourceRecord
    {
        Path = @"D:\Music",
        Type = LibrarySourceType.Music,
        Enabled = true
    });

long karaokeSourceId =
    sourceRepository.Add(new LibrarySourceRecord
    {
        Path = @"D:\Karaoke",
        Type = LibrarySourceType.Karaoke,
        Enabled = true
    });

Console.WriteLine("Library Sources");
Console.WriteLine();

foreach (LibrarySourceRecord source in sourceRepository.GetAll())
{
    Console.WriteLine($"{source.Id} - {source.Type} - {source.Path}");
}

Console.WriteLine();

long firstTrackId =
    trackRepository.Add(new MediaTrackRecord
    {
        SourceId = musicSourceId,
        Title = "Don't Stop Me Now",
        Artist = "Queen",
        FilePath = @"D:\Music\Queen - Don't Stop Me Now.mp3",
        DurationSeconds = 209,
        IsKaraoke = false
    });

long secondTrackId =
    trackRepository.Add(new MediaTrackRecord
    {
        SourceId = karaokeSourceId,
        Title = "Bohemian Rhapsody",
        Artist = "Queen",
        FilePath = @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
        DurationSeconds = 354,
        IsKaraoke = true
    });

Console.WriteLine("Tracks");
Console.WriteLine();

foreach (MediaTrackRecord track in trackRepository.GetAll())
{
    Console.WriteLine($"{track.Id} - {track.Artist} - {track.Title} [{(track.IsKaraoke ? "Karaoke" : "Music")}]");
}

Console.WriteLine();

MediaTrackRecord? firstTrack = trackRepository.GetById(firstTrackId);

if (firstTrack is not null)
{
    MediaTrackRecord updatedTrack =
        firstTrack with
        {
            DurationSeconds = 210
        };

    bool updated = trackRepository.Update(updatedTrack);

    Console.WriteLine($"Updated track: {updated}");
}

bool deleted = trackRepository.Delete(secondTrackId);

Console.WriteLine($"Deleted track: {deleted}");

Console.WriteLine();
Console.WriteLine("Remaining Tracks");
Console.WriteLine();

foreach (MediaTrackRecord track in trackRepository.GetAll())
{
    Console.WriteLine($"{track.Id} - {track.Artist} - {track.Title} ({track.DurationSeconds} seconds)");
}

static void PrintForeignKeys(SqliteConnection connection, string tableName)
{
    using SqliteCommand command =
        connection.CreateCommand();

    command.CommandText =
        $"PRAGMA foreign_key_list({tableName});";

    using SqliteDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        string referencedTable = reader.GetString(2);

        string fromColumn = reader.GetString(3);

        string toColumn = reader.GetString(4);

        string onDelete = reader.GetString(6);

        Console.WriteLine($"- {tableName}.{fromColumn} -> {referencedTable}.{toColumn} [ON DELETE {onDelete}]");
    }
}
