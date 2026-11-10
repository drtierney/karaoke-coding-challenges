
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MetadataReader;

// ---------------------------------------------------------
// Challenge 044 - Persistent Library Milestone Demo
// ---------------------------------------------------------

string demoDirectory = Path.Combine(Path.GetTempPath(), "karaoke-challenge-044-demo");

string karaokeDirectory = Path.Combine(demoDirectory, "Karaoke");
string databasePath = Path.Combine(demoDirectory, "library.db");
string connectionString = $"Data Source={databasePath};Pooling=False";

string queenMp3Path = Path.Combine(karaokeDirectory, "Queen.mp3");
string queenCdgPath = Path.Combine(karaokeDirectory, "Queen.cdg");
string abbaMp3Path = Path.Combine(karaokeDirectory, "ABBA.mp3");
string abbaCdgPath = Path.Combine(karaokeDirectory, "ABBA.cdg");

Console.WriteLine("Challenge 044 - Persistent Library Milestone");
Console.WriteLine();

// Start each run with a clean demo environment.
if (Directory.Exists(demoDirectory))
{
    Directory.Delete(demoDirectory, recursive: true);
}

Directory.CreateDirectory(karaokeDirectory);

try
{
    // -----------------------------------------------------
    // Stage 1 - Create simulated library
    // -----------------------------------------------------

    File.WriteAllText(queenMp3Path, "Simulated Queen MP3 content");
    File.WriteAllText(queenCdgPath, "Simulated Queen CDG content");
    File.WriteAllText(abbaMp3Path, "Simulated ABBA MP3 content");

    Console.WriteLine("Demo Environment");
    Console.WriteLine($"Library:  {karaokeDirectory}");
    Console.WriteLine($"Database: {databasePath}");
    Console.WriteLine();

    // -----------------------------------------------------
    // Stage 2 - Initialise SQLite
    // -----------------------------------------------------

    MediaLibraryDatabaseInitializer initializer = new(connectionString);
    initializer.Initialize();

    LibrarySourceRepository sourceRepository = new(connectionString);

    long sourceId = sourceRepository.Add(new LibrarySourceRecord
    {
        Path = karaokeDirectory,
        Type = LibrarySourceType.Karaoke,
        Enabled = true
    });

    // -----------------------------------------------------
    // Stage 3 - Construct services
    // -----------------------------------------------------

    MediaTrackRepository trackRepository = new(connectionString);
    KaraokeFileRepository karaokeFileRepository = new(connectionString);

    FileFingerprintProvider fingerprintProvider = new();
    FileChangeDetector changeDetector = new();

    ScannedTrackPersistenceService persistenceService = new(trackRepository, connectionString);

    KaraokeFilePairingService pairingService = new();

    KaraokeLibraryPersistenceService karaokePersistenceService = new(
        persistenceService,
        karaokeFileRepository,
        pairingService,
        connectionString);

    IncrementalScanService incrementalScanService = new(
        trackRepository,
        fingerprintProvider,
        changeDetector,
        persistenceService,
        karaokePersistenceService);

    LibraryReconciliationService reconciliationService = new();

    LibraryReconciliationPersistenceService reconciliationPersistenceService = new(connectionString);

    LibraryScanOrchestrationService orchestrationService = new(
        trackRepository,
        fingerprintProvider,
        reconciliationService,
        reconciliationPersistenceService,
        incrementalScanService,
        karaokeFileRepository);

    MilestoneDemoMetadataReader metadataReader = new();

    // -----------------------------------------------------
    // Stage 4 - Initial scan
    // -----------------------------------------------------

    RunScan("Pass 1 - Initial Library Scan");

    MediaTrackRecord queenOriginal = trackRepository.GetByFilePath(queenMp3Path) ?? throw new InvalidOperationException("Queen track not found.");

    MediaTrackRecord abbaOriginal = trackRepository.GetByFilePath(abbaMp3Path) ?? throw new InvalidOperationException("ABBA track not found.");

    long queenTrackId = queenOriginal.Id;
    long abbaTrackId = abbaOriginal.Id;

    Console.WriteLine("Initial Persistence Verification");
    Console.WriteLine($"Queen CDG linked: {karaokeFileRepository.GetByTrackId(queenTrackId) == queenCdgPath}");
    Console.WriteLine($"ABBA standalone: {karaokeFileRepository.GetByTrackId(abbaTrackId) is null}");
    Console.WriteLine();

    // -----------------------------------------------------
    // Stage 5 - Repeat scan
    // -----------------------------------------------------

    RunScan("Pass 2 - Repeat Scan (Unchanged Files)");

    // -----------------------------------------------------
    // Stage 6 - Modify an existing MP3
    // -----------------------------------------------------

    File.WriteAllText(queenMp3Path, "Simulated Queen MP3 content - Modified version");

    RunScan("Pass 3 - Modified MP3 Scan");

    MediaTrackRecord queenUpdated = trackRepository.GetByFilePath(queenMp3Path) ?? throw new InvalidOperationException("Updated Queen track not found.");

    Console.WriteLine("Modified Track Verification");
    Console.WriteLine($"Track ID preserved: {queenUpdated.Id == queenTrackId}");
    Console.WriteLine($"File size changed: {queenUpdated.FileSize != queenOriginal.FileSize}");
    Console.WriteLine($"Updated title: {queenUpdated.Title}");
    Console.WriteLine();

    // -----------------------------------------------------
    // Stage 7 - Add CDG to an unchanged MP3
    // -----------------------------------------------------

    File.WriteAllText(abbaCdgPath, "Simulated ABBA CDG content");

    RunScan("Pass 4 - Add CDG Companion");

    Console.WriteLine("New Companion Verification");
    Console.WriteLine($"ABBA CDG linked: {karaokeFileRepository.GetByTrackId(abbaTrackId) == abbaCdgPath}");
    Console.WriteLine();

    // -----------------------------------------------------
    // Stage 8 - Remove a CDG companion
    // -----------------------------------------------------

    File.Delete(queenCdgPath);

    RunScan("Pass 5 - Remove CDG Companion");

    Console.WriteLine("Removed Companion Verification");
    Console.WriteLine($"Queen CDG removed: {karaokeFileRepository.GetByTrackId(queenTrackId) is null}");
    Console.WriteLine($"Queen MP3 preserved: {trackRepository.GetById(queenTrackId) is not null}");
    Console.WriteLine();

    // -----------------------------------------------------
    // Stage 9 - Remove an MP3
    // -----------------------------------------------------

    File.Delete(abbaMp3Path);

    RunScan("Pass 6 - Missing MP3 Reconciliation");

    MediaTrackRecord abbaMissing = trackRepository.GetById(abbaTrackId) ?? throw new InvalidOperationException("Missing ABBA record was deleted.");

    Console.WriteLine("Missing File Verification");
    Console.WriteLine($"ABBA marked unavailable: {!abbaMissing.IsAvailable}");
    Console.WriteLine($"ABBA track ID preserved: {abbaMissing.Id == abbaTrackId}");
    Console.WriteLine();

    // -----------------------------------------------------
    // Stage 10 - Restore missing MP3
    // -----------------------------------------------------

    File.WriteAllText(
        abbaMp3Path,
        "Simulated ABBA MP3 content - Restored");

    RunScan("Pass 7 - Restore Missing MP3");

    MediaTrackRecord abbaRestored =
        trackRepository.GetByFilePath(abbaMp3Path)
        ?? throw new InvalidOperationException("Restored ABBA track not found.");

    Console.WriteLine("Restored File Verification");
    Console.WriteLine($"ABBA available again: {abbaRestored.IsAvailable}");
    Console.WriteLine($"ABBA track ID preserved: {abbaRestored.Id == abbaTrackId}");
    Console.WriteLine($"ABBA CDG linked: {karaokeFileRepository.GetByTrackId(abbaTrackId) == abbaCdgPath}");
    Console.WriteLine($"Fingerprint stored: {abbaRestored.FileSize is not null && abbaRestored.LastModifiedUtc is not null}");
    Console.WriteLine();


    // -----------------------------------------------------
    // Stage 11 - Reopen database and verify persistence
    // -----------------------------------------------------

    Console.WriteLine("Final Database Reload");

    MediaTrackRepository reopenedTrackRepository = new(connectionString);
    KaraokeFileRepository reopenedKaraokeRepository = new(connectionString);

    IReadOnlyList<MediaTrackRecord> finalTracks = reopenedTrackRepository.GetBySourceId(sourceId);

    foreach (MediaTrackRecord track in finalTracks)
    {
        string? cdgPath = reopenedKaraokeRepository.GetByTrackId(track.Id);

        Console.WriteLine($"ID: {track.Id}");
        Console.WriteLine($"Title: {track.Title}");
        Console.WriteLine($"Available: {track.IsAvailable}");
        Console.WriteLine($"Karaoke: {track.IsKaraoke}");
        Console.WriteLine($"MP3: {track.FilePath}");
        Console.WriteLine($"CDG: {cdgPath ?? "None"}");
        Console.WriteLine();
    }

    Console.WriteLine($"Stored tracks: {finalTracks.Count}");
    Console.WriteLine($"Available tracks: {finalTracks.Count(t => t.IsAvailable)}");
    Console.WriteLine($"Total metadata reads: {metadataReader.ReadCount}");
    Console.WriteLine();

    Console.WriteLine("Challenge 044 Demo Complete");

    // -----------------------------------------------------
    // Local helper - Run complete library scan
    // -----------------------------------------------------

    void RunScan(string title)
    {
        string[] discoveredPaths = Directory
            .EnumerateFiles(karaokeDirectory, "*", SearchOption.AllDirectories)
            .Where(path =>
                string.Equals(Path.GetExtension(path), ".mp3", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Path.GetExtension(path), ".cdg", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        (LibraryReconciliationResult reconciliation,
         IncrementalScanSummary scan) = orchestrationService.ProcessLibrary(
            sourceId,
            discoveredPaths,
            discoveryCompletedSuccessfully: true,
            metadataReader,
            isKaraoke: true);

        Console.WriteLine(title);

        Console.WriteLine($"Discovered files: {discoveredPaths.Length}");
        Console.WriteLine($"Tracks scanned:   {scan.Total}");
        Console.WriteLine($"Added:            {scan.Added}");
        Console.WriteLine($"Updated:          {scan.Updated}");
        Console.WriteLine($"Skipped:          {scan.Skipped}");
        Console.WriteLine($"Failed:           {scan.Failed}");
        Console.WriteLine($"Missing tracks:   {reconciliation.MissingTrackCount}");
        Console.WriteLine($"Metadata reads:   {metadataReader.ReadCount}");
        Console.WriteLine();
    }
}
finally
{
    if (Directory.Exists(demoDirectory))
    {
        Directory.Delete(demoDirectory, recursive: true);
    }
}

// ---------------------------------------------------------
// Simulated metadata reader
// ---------------------------------------------------------

public sealed class MilestoneDemoMetadataReader : IMetadataReader
{
    public int ReadCount { get; private set; }

    public EmbeddedMetadata Read(string filePath)
    {
        ReadCount++;

        string content = File.ReadAllText(filePath);
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        string title = content.Contains("Modified", StringComparison.OrdinalIgnoreCase) ? $"{fileName} - Modified" : fileName;

        return new EmbeddedMetadata
        {
            Title = title,
            Artist = "Demo Artist",
            Album = "Challenge 044 Demo",
            DurationSeconds = 180
        };
    }
}
