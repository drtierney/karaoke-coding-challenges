using KaraokeCodingChallenges.MediaLibraryDatabase;
using Microsoft.Data.Sqlite;

string databasePath =
    Path.Combine(
        Path.GetTempPath(),
        "karaoke-challenge-036-demo.db");

if (File.Exists(databasePath))
{
    File.Delete(databasePath);
}

string connectionString =
    $"Data Source={databasePath};Pooling=False";

MediaLibraryDatabaseInitializer initializer =
    new MediaLibraryDatabaseInitializer(connectionString);

Console.WriteLine("Media Library Database Schema Demo");
Console.WriteLine();

initializer.Initialize();

Console.WriteLine("Database initialized at:");
Console.WriteLine(databasePath);
Console.WriteLine();

using SqliteConnection connection =
    new SqliteConnection(connectionString);

connection.Open();

using (SqliteCommand foreignKeysCommand = connection.CreateCommand())
{
    foreignKeysCommand.CommandText =
        "PRAGMA foreign_keys = ON;";

    foreignKeysCommand.ExecuteNonQuery();
}

using (SqliteCommand foreignKeysEnabledCommand = connection.CreateCommand())
{
    foreignKeysEnabledCommand.CommandText =
        "PRAGMA foreign_keys;";

    long foreignKeysEnabled =
        (long)foreignKeysEnabledCommand.ExecuteScalar()!;

    Console.WriteLine(
        $"Foreign key enforcement: " +
        $"{(foreignKeysEnabled == 1 ? "Enabled" : "Disabled")}");

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

    using SqliteDataReader reader =
        tableCommand.ExecuteReader();

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

    using SqliteDataReader reader =
        indexCommand.ExecuteReader();

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

static void PrintForeignKeys(
    SqliteConnection connection,
    string tableName)
{
    using SqliteCommand command =
        connection.CreateCommand();

    command.CommandText =
        $"PRAGMA foreign_key_list({tableName});";

    using SqliteDataReader reader =
        command.ExecuteReader();

    while (reader.Read())
    {
        string referencedTable =
            reader.GetString(2);

        string fromColumn =
            reader.GetString(3);

        string toColumn =
            reader.GetString(4);

        string onDelete =
            reader.GetString(6);

        Console.WriteLine(
            $"- {tableName}.{fromColumn} -> " +
            $"{referencedTable}.{toColumn} " +
            $"[ON DELETE {onDelete}]");
    }
}
