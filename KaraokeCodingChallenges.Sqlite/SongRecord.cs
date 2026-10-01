namespace KaraokeCodingChallenges.Sqlite;

public class SongRecord
{
    public int Id { get; set; }

    public required string Artist { get; set; }

    public required string Title { get; set; }

    public int Year { get; set; }
}
