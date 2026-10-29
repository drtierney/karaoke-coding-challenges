
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.IncrementalScanning.Watchers;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MetadataReader;

namespace KaraokeCodingChallenges.IncrementalScanning.Demo.Demos;

public static class WatchedLibraryDemo
{
    private sealed class DemoMetadataReader : IMetadataReader
    {
        public EmbeddedMetadata Read(string filePath)
        {
            return new EmbeddedMetadata
            {
                Title = Path.GetFileNameWithoutExtension(filePath),
                Artist = "Demo Artist",
                DurationSeconds = 180
            };
        }
    }

    public static void Run()
    {
        string demoDirectory = Path.Combine(
            Path.GetTempPath(),
            "karaoke-challenge-042-live-library");

        string libraryDirectory = Path.Combine(demoDirectory, "Music");
        string databasePath = Path.Combine(demoDirectory, "library.db");

        // Reset the demonstration on each run.
        if (Directory.Exists(demoDirectory))
        {
            Directory.Delete(demoDirectory, recursive: true);
        }

        Directory.CreateDirectory(libraryDirectory);

        string connectionString = $"Data Source={databasePath};Pooling=False";

        MediaLibraryDatabaseInitializer initializer = new(connectionString);
        initializer.Initialize();

        LibrarySourceRepository sourceRepository = new(connectionString);

        long sourceId = sourceRepository.Add(new LibrarySourceRecord
        {
            Path = libraryDirectory,
            Type = LibrarySourceType.Music,
            Enabled = true
        });

        MediaTrackRepository trackRepository = new(connectionString);

        ScannedTrackPersistenceService persistenceService = new(
            trackRepository,
            connectionString);

        WatchedLibraryUpdateService updateService = new(
            trackRepository,
            persistenceService,
            new DemoMetadataReader(),
            new FileFingerprintProvider());

        using LibraryFileWatcher watcher = new(libraryDirectory);

        using WatchedLibraryEventCoordinator coordinator = new(
            watcher,
            updateService,
            sourceId);

        coordinator.ProcessingError += (_, exception) =>
        {
            Console.Error.WriteLine(
                $"[ERROR] Processing failed: {exception.Message}");
        };

        watcher.WatcherError += (_, e) =>
        {
            Console.Error.WriteLine(
                $"[ERROR] Watcher failure: {e.GetException().Message}");
        };

        string originalPath = Path.Combine(libraryDirectory, "Track1.mp3");
        string renamedPath = Path.Combine(libraryDirectory, "RenamedTrack.mp3");

        Console.WriteLine("Challenge 042 - Live Library Updates Demo");
        Console.WriteLine();

        Console.WriteLine($"Watched directory: {libraryDirectory}");
        Console.WriteLine($"SQLite database:   {databasePath}");
        Console.WriteLine();

        Console.WriteLine("Simulated MP3 files are used for this demonstration.");
        Console.WriteLine("Metadata is provided by a demo metadata reader.");
        Console.WriteLine();

        watcher.Start();

        try
        {
            bool running = true;

            while (running)
            {
                DisplayMenu();

                Console.Write("Select an option: ");

                string? input = Console.ReadLine();

                Console.WriteLine();

                switch (input?.Trim().ToUpperInvariant())
                {
                    case "1":
                        CreateTrack(originalPath);
                        break;

                    case "2":
                        ModifyTrack(originalPath, renamedPath);
                        break;

                    case "3":
                        RenameTrack(originalPath, renamedPath);
                        break;

                    case "4":
                        DeleteTrack(originalPath, renamedPath);
                        break;

                    case "5":
                        DisplayTracks(trackRepository);
                        break;

                    case "Q":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine();
            }
        }
        finally
        {
            watcher.Stop();
        }

        Console.WriteLine("Watcher stopped.");
        Console.WriteLine("Demo data remains in the temporary directory until the next run.");
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  1 - Create track");
        Console.WriteLine("  2 - Modify track");
        Console.WriteLine("  3 - Rename track");
        Console.WriteLine("  4 - Delete track");
        Console.WriteLine("  5 - Display database tracks");
        Console.WriteLine("  Q - Quit");
        Console.WriteLine();
    }

    private static void CreateTrack(string filePath)
    {
        if (File.Exists(filePath))
        {
            Console.WriteLine("Track1.mp3 already exists.");
            return;
        }

        File.WriteAllText(filePath, "Simulated MP3 content");

        Console.WriteLine($"Created: {Path.GetFileName(filePath)}");
        Console.WriteLine("The watcher will scan and persist the new track.");
    }

    private static void ModifyTrack(
        string originalPath,
        string renamedPath)
    {
        string? filePath = GetExistingTrackPath(originalPath, renamedPath);

        if (filePath is null)
        {
            Console.WriteLine("No track found to modify.");
            return;
        }

        File.AppendAllText(filePath, " Additional simulated audio content");

        Console.WriteLine($"Modified: {Path.GetFileName(filePath)}");
        Console.WriteLine("The watcher will compare fingerprints and update SQLite.");
    }

    private static void RenameTrack(
        string originalPath,
        string renamedPath)
    {
        if (!File.Exists(originalPath))
        {
            Console.WriteLine("Track1.mp3 was not found.");
            return;
        }

        if (File.Exists(renamedPath))
        {
            Console.WriteLine("RenamedTrack.mp3 already exists.");
            return;
        }

        File.Move(originalPath, renamedPath);

        Console.WriteLine("Renamed: Track1.mp3 -> RenamedTrack.mp3");
        Console.WriteLine("The watcher will update the existing database record.");
    }

    private static void DeleteTrack(
        string originalPath,
        string renamedPath)
    {
        string? filePath = GetExistingTrackPath(originalPath, renamedPath);

        if (filePath is null)
        {
            Console.WriteLine("No track found to delete.");
            return;
        }

        File.Delete(filePath);

        Console.WriteLine($"Deleted: {Path.GetFileName(filePath)}");
        Console.WriteLine("The watcher will mark the track unavailable.");
    }

    private static string? GetExistingTrackPath(
        string originalPath,
        string renamedPath)
    {
        if (File.Exists(originalPath))
        {
            return originalPath;
        }

        if (File.Exists(renamedPath))
        {
            return renamedPath;
        }

        return null;
    }

    private static void DisplayTracks(MediaTrackRepository repository)
    {
        IReadOnlyList<MediaTrackRecord> tracks = repository.GetAll();

        Console.WriteLine("SQLite Media Library");
        Console.WriteLine(new string('-', 85));

        if (tracks.Count == 0)
        {
            Console.WriteLine("No tracks found.");
            return;
        }

        Console.WriteLine(
            $"{"ID",-5} {"Title",-20} {"Available",-12} {"Size",-12} File");

        Console.WriteLine(new string('-', 85));

        foreach (MediaTrackRecord track in tracks)
        {
            string available = track.IsAvailable ? "Yes" : "No";
            string size = track.FileSize?.ToString() ?? "N/A";

            Console.WriteLine(
                $"{track.Id,-5} " +
                $"{track.Title,-20} " +
                $"{available,-12} " +
                $"{size,-12} " +
                $"{Path.GetFileName(track.FilePath)}");
        }

        Console.WriteLine();

        Console.WriteLine($"Total tracks: {tracks.Count}");
        Console.WriteLine($"Available:    {tracks.Count(track => track.IsAvailable)}");
        Console.WriteLine($"Unavailable:  {tracks.Count(track => !track.IsAvailable)}");
    }
}
