using KaraokeCodingChallenges.Sqlite;

string databasePath = Path.Combine(Path.GetTempPath(), "karaoke-challenge-035-demo.db");

if (File.Exists(databasePath))
{
    File.Delete(databasePath);
}

SqliteDatabase database = new(databasePath);

Console.WriteLine("SQLite Fundamentals Demo");
Console.WriteLine();

database.Initialize();

Console.WriteLine($"Database initialized at:");
Console.WriteLine(databasePath);
Console.WriteLine();

SongRecord song = new()
{
    Artist = "Paramore",
    Title = "The Only Exception",
    Year = 2009
};

int id = database.AddSong(song);

Console.WriteLine("Inserted Song");
Console.WriteLine($"Id: {id}");
Console.WriteLine($"Artist: {song.Artist}");
Console.WriteLine($"Title: {song.Title}");
Console.WriteLine($"Year: {song.Year}");
Console.WriteLine();

SongRecord? retrievedSong = database.GetSong(id);

Console.WriteLine("Retrieved Song");

if (retrievedSong is not null)
{
    Console.WriteLine($"Id: {retrievedSong.Id}");
    Console.WriteLine($"Artist: {retrievedSong.Artist}");
    Console.WriteLine($"Title: {retrievedSong.Title}");
    Console.WriteLine($"Year: {retrievedSong.Year}");
}

Console.WriteLine();

song.Id = id;
song.Artist = "Paramore";
song.Title = "Hard Times";
song.Year = 2017;

bool updated = database.UpdateSong(song);

Console.WriteLine($"Updated Song: {updated}");

SongRecord? updatedSong = database.GetSong(id);

if (updatedSong is not null)
{
    Console.WriteLine($"Id: {updatedSong.Id}");
    Console.WriteLine($"Artist: {updatedSong.Artist}");
    Console.WriteLine($"Title: {updatedSong.Title}");
    Console.WriteLine($"Year: {updatedSong.Year}");
}

Console.WriteLine();

bool deleted = database.DeleteSong(id);

Console.WriteLine($"Deleted Song: {deleted}");

SongRecord? deletedSong = database.GetSong(id);

Console.WriteLine(
    deletedSong is null
        ? "Song no longer exists."
        : "Song still exists.");
