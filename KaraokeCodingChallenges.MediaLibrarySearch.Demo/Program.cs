using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibrarySearch;
using KaraokeCodingChallenges.MediaLibrarySearch.Models;

Console.WriteLine("Challenge 039 - Database-Backed Search");
Console.WriteLine();

string databasePath = Path.Combine(Path.GetTempPath(), $"karaoke-challenge-039-{Guid.NewGuid()}.db");

string connectionString = $"Data Source={databasePath};Pooling=False";

try
{
    MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

    initializer.Initialize();

    Console.WriteLine($"Database initialized at: {databasePath}");
    Console.WriteLine();

    LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);
    MediaTrackRepository trackRepository = new MediaTrackRepository(connectionString);

    string musicPath = Path.Combine(Path.GetTempPath(), "Music");
    string karaokePath = Path.Combine(Path.GetTempPath(), "Karaoke");

    // Add library sources
    long musicSourceId =
        sourceRepository.Add(new LibrarySourceRecord
        {
            Path = musicPath,
            Type = LibrarySourceType.Music,
            Enabled = true
        });

    long karaokeSourceId =
        sourceRepository.Add(new LibrarySourceRecord
        {
            Path = karaokePath,
            Type = LibrarySourceType.Karaoke,
            Enabled = true
        });

    // Insert sample tracks
    AddTrack(trackRepository, musicSourceId, musicPath, "Bohemian Rhapsody", "Queen", 354);
    AddTrack(trackRepository, musicSourceId, musicPath, "Don't Stop Me Now", "Queen", 251);
    AddTrack(trackRepository, musicSourceId, musicPath, "Misery Business", "Paramore", 259);
    AddTrack(trackRepository, musicSourceId, musicPath, "Bohemian Like You", "The Dandy Warhols", 195);
    AddTrack(trackRepository, musicSourceId, musicPath, "100% Pure Love", "Crystal Waters", 217);
    AddTrack(trackRepository, karaokeSourceId, karaokePath, "Bohemian Rhapsody", "Queen", 355, isKaraoke: true);

    // Execute search demonstrations
    SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(connectionString);

    MediaTrackSearchCriteria allCriteria = new MediaTrackSearchCriteria();

    PrintResults("All Tracks:", searchService.Search(allCriteria));

    MediaTrackSearchCriteria titleCriteria = new MediaTrackSearchCriteria
    {
        Title = "Bohemian"
    };

    PrintResults("Title Search - Bohemian:", searchService.Search(titleCriteria));

    MediaTrackSearchCriteria artistCriteria = new MediaTrackSearchCriteria
    {
        Artist = "Queen"
    };

    PrintResults("Artist Search - Queen:", searchService.Search(artistCriteria));

    MediaTrackSearchCriteria bothCriteria = new MediaTrackSearchCriteria
    {
        Title = "Bohemian",
        Artist = "Queen",
        MatchMode = MediaTrackSearchMatchMode.All
    };

    PrintResults("Match Mode All - Bohemian AND Queen:", searchService.Search(bothCriteria));

    MediaTrackSearchCriteria eitherCriteria = new MediaTrackSearchCriteria
    {
        Title = "Bohemian",
        Artist = "Queen",
        MatchMode = MediaTrackSearchMatchMode.Any
    };

    PrintResults("Match Mode Any - Bohemian OR Queen:", searchService.Search(eitherCriteria));

    MediaTrackSearchCriteria containsPercentageCriteria = new MediaTrackSearchCriteria
    {
        Title = "%"
    };

    PrintResults("Contains Percentage Search:", searchService.Search(containsPercentageCriteria));

    MediaTrackSearchCriteria unknownTrackCriteria = new MediaTrackSearchCriteria
    {
        Title = "Unknown Track",
        Artist = "Unknown Artist"
    };

    PrintResults("Unknown Track Search:", searchService.Search(unknownTrackCriteria));
}
finally
{
    if (File.Exists(databasePath))
    {
        File.Delete(databasePath);
    }
}

// Helper methods
static void PrintResults(string heading, IReadOnlyList<MediaTrackRecord> tracks)
{
    Console.WriteLine(heading);

    if (tracks.Count == 0)
    {
        Console.WriteLine("  No matching tracks.");
        Console.WriteLine();
        return;
    }

    string trackLabel = tracks.Count == 1 ? "track" : "tracks";

    Console.WriteLine($"  {tracks.Count} matching {trackLabel}");

    foreach (MediaTrackRecord track in tracks)
    {
        string type = track.IsKaraoke ? "Karaoke" : "Music";

        Console.WriteLine($"  {track.Artist} - {track.Title} [{type}]");
    }

    Console.WriteLine();
}

static void AddTrack(
    MediaTrackRepository repository,
    long sourceId,
    string sourcePath,
    string title,
    string artist,
    int durationSeconds,
    bool isKaraoke = false)
{
    string filePath = Path.Combine(sourcePath, $"{artist} - {title}.mp3");

    MediaTrackRecord track = new MediaTrackRecord
    {
        SourceId = sourceId,
        Title = title,
        Artist = artist,
        FilePath = filePath,
        DurationSeconds = durationSeconds,
        IsKaraoke = isKaraoke
    };

    repository.Add(track);
}
