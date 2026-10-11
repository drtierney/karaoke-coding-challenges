using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibrarySearch.Models;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibrarySearch;

public class SqliteMediaTrackSearchService
{
    private readonly string _connectionString;

    public SqliteMediaTrackSearchService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null, empty, or whitespace.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public IReadOnlyList<MediaTrackRecord> Search(MediaTrackSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (!Enum.IsDefined(criteria.MatchMode))
        {
            throw new ArgumentOutOfRangeException(nameof(criteria.MatchMode), criteria.MatchMode, "Invalid match mode specified.");
        }

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
            IsKaraoke
        FROM Tracks
        """;

        List<string> conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(criteria.Title))
        {
            conditions.Add(@"Title LIKE @title ESCAPE '\'");

            command.Parameters.AddWithValue("@title", $"%{EscapeLikePattern(criteria.Title.Trim())}%");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Artist))
        {
            conditions.Add(@"Artist LIKE @artist ESCAPE '\'");

            command.Parameters.AddWithValue("@artist", $"%{EscapeLikePattern(criteria.Artist.Trim())}%");
        }

        string separator = criteria.MatchMode == MediaTrackSearchMatchMode.All
            ? " AND "
            : " OR ";

        if (conditions.Count > 0)
        {
            command.CommandText += " WHERE " + string.Join(separator, conditions);
        }

        command.CommandText += " ORDER BY Id;";

        using SqliteDataReader reader = command.ExecuteReader();

        List<MediaTrackRecord> tracks = new List<MediaTrackRecord>();

        while (reader.Read())
        {
            MediaTrackRecord track = new MediaTrackRecord
            {
                Id = reader.GetInt64(0),
                SourceId = reader.GetInt64(1),
                Title = reader.GetString(2),
                Artist = reader.GetString(3),
                FilePath = reader.GetString(4),
                DurationSeconds = reader.GetInt32(5),
                IsKaraoke = reader.GetInt32(6) == 1
            };

            tracks.Add(track);
        }


        return tracks;
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_");
    }
}
