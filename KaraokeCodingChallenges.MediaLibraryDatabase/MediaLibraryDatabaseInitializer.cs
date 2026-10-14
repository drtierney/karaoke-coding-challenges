
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase
{
    public class MediaLibraryDatabaseInitializer
    {
        private readonly string _connectionString;

        public MediaLibraryDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);

            connection.Open();

            using SqliteCommand foreignKeysCommand = connection.CreateCommand();

            foreignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";

            foreignKeysCommand.ExecuteNonQuery();

            Assembly assembly = typeof(MediaLibraryDatabaseInitializer).Assembly;

            using Stream? stream = assembly.GetManifestResourceStream("KaraokeCodingChallenges.MediaLibraryDatabase.Sql.CreateSchema.sql");

            if (stream is null)
            {
                throw new InvalidOperationException("Unable to load embedded database schema.");
            }

            using StreamReader reader = new StreamReader(stream);

            string schemaSql = reader.ReadToEnd();

            using SqliteCommand schemaCommand = connection.CreateCommand();

            schemaCommand.CommandText = schemaSql;

            schemaCommand.ExecuteNonQuery();

            EnsureFingerprintColumns(connection);
        }

        private static void EnsureFingerprintColumns(SqliteConnection connection)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText = "PRAGMA table_info(Tracks);";

            HashSet<string> existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string columnName = reader.GetString(1);
                    existingColumns.Add(columnName);
                }
            }

            Dictionary<string, string> fingerprintColumns = new Dictionary<string, string>
            {
                { "FileSize", "INTEGER NULL" },
                { "LastModifiedUtc", "TEXT NULL" }
            };

            foreach (KeyValuePair<string, string> column in fingerprintColumns)
            {
                if (!existingColumns.Contains(column.Key))
                {
                    using SqliteCommand addColumnCommand = connection.CreateCommand();

                    addColumnCommand.CommandText = $"ALTER TABLE Tracks ADD COLUMN {column.Key} {column.Value};";

                    addColumnCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
