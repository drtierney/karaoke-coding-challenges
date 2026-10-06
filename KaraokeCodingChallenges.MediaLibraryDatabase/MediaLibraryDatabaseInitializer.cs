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
        }
    }
}
