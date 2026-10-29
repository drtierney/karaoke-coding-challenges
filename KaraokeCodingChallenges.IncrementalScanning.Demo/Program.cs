
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Demo;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.IncrementalScanning.Demo.Demos;

if (args.Contains("--watcher", StringComparer.OrdinalIgnoreCase))
{
    bool showRawEvents = args.Contains("--raw", StringComparer.OrdinalIgnoreCase);

    FileWatcherDemo.Run(showRawEvents);
    return;
}

if (args.Contains("--library-watch", StringComparer.OrdinalIgnoreCase))
{
    WatchedLibraryDemo.Run();
    return;
}

string databasePath = Path.Combine(
    Path.GetTempPath(),
    $"incremental-demo-{Guid.NewGuid():N}.db");

string connectionString = $"Data Source={databasePath};Pooling=False";

string directoryPath = Path.Combine(
    Path.GetTempPath(),
    $"incremental-demo-files-{Guid.NewGuid():N}");

try
{
    Directory.CreateDirectory(directoryPath);

    string[] filePaths =
    [
        Path.Combine(directoryPath, "Queen - Bohemian Rhapsody.mp3"),
        Path.Combine(directoryPath, "ABBA - Dancing Queen.mp3"),
        Path.Combine(directoryPath, "Fleetwood Mac - Dreams.mp3")
    ];

    foreach (string filePath in filePaths)
    {
        File.WriteAllText(filePath, "Demo media content");
    }

    MediaLibraryDatabaseInitializer initializer = new(connectionString);
    initializer.Initialize();

    LibrarySourceRepository sourceRepository = new(connectionString);

    long sourceId = sourceRepository.Add(new LibrarySourceRecord
    {
        Path = directoryPath,
        Type = LibrarySourceType.Music,
        Enabled = true
    });

    MediaTrackRepository repository = new(connectionString);
    FileFingerprintProvider fingerprintProvider = new();
    FileChangeDetector changeDetector = new();

    ScannedTrackPersistenceService persistenceService =
        new(repository, connectionString);

    IncrementalScanService incrementalScanService = new(
        repository,
        fingerprintProvider,
        changeDetector,
        persistenceService);

    LibraryReconciliationService reconciliationService = new();

    LibraryReconciliationPersistenceService reconciliationPersistenceService =
        new(connectionString);

    LibraryScanOrchestrationService orchestrationService = new(
        repository,
        fingerprintProvider,
        reconciliationService,
        reconciliationPersistenceService,
        incrementalScanService);

    DemoMetadataReader metadataReader = new();

    Console.WriteLine("Challenge 040/041 - Incremental Scanning and Library Reconciliation");
    Console.WriteLine();

    // Pass 1 - Initial scan
    IncrementalScanSummary firstPass = incrementalScanService.ProcessFiles(
        filePaths,
        sourceId,
        metadataReader);

    PrintSummary("Pass 1 - Initial Scan", firstPass, metadataReader.ReadCount);

    // Pass 2 - Unchanged files
    IncrementalScanSummary secondPass = incrementalScanService.ProcessFiles(
        filePaths,
        sourceId,
        metadataReader);

    PrintSummary("Pass 2 - Repeat Scan", secondPass, metadataReader.ReadCount);

    // Pass 3 - Modified file
    string modifiedFilePath = filePaths[0];

    MediaTrackRecord originalTrack = repository.GetByFilePath(modifiedFilePath)
        ?? throw new InvalidOperationException("Original track not found.");

    long originalTrackId = originalTrack.Id;
    long originalFileSize = originalTrack.FileSize ?? 0;

    File.WriteAllText(modifiedFilePath, "Demo media content - Modified");

    IncrementalScanSummary thirdPass = incrementalScanService.ProcessFiles(
        filePaths,
        sourceId,
        metadataReader);

    PrintSummary("Pass 3 - Modified File Scan", thirdPass, metadataReader.ReadCount);

    MediaTrackRecord updatedTrack = repository.GetByFilePath(modifiedFilePath)
        ?? throw new InvalidOperationException("Modified track not found.");

    Console.WriteLine("Modified Track Verification");
    Console.WriteLine($"Track ID preserved: {originalTrackId == updatedTrack.Id}");
    Console.WriteLine($"Original file size: {originalFileSize} bytes");
    Console.WriteLine($"Updated file size:  {updatedTrack.FileSize} bytes");
    Console.WriteLine();

    // Pass 4 - Missing file
    string missingPath = filePaths[1];

    MediaTrackRecord abbaBefore = repository.GetByFilePath(missingPath)
        ?? throw new InvalidOperationException("ABBA track not found.");

    File.Delete(missingPath);

    string[] remainingPaths = Directory.GetFiles(
        directoryPath,
        "*.mp3",
        SearchOption.AllDirectories);

    (LibraryReconciliationResult fourthReconciliation, IncrementalScanSummary fourthScan) =
        orchestrationService.ProcessLibrary(
            sourceId,
            remainingPaths,
            discoveryCompletedSuccessfully: true,
            metadataReader);

    PrintSummary("Pass 4 - Missing File Scan", fourthScan, metadataReader.ReadCount);
    PrintReconciliation(fourthReconciliation);

    MediaTrackRecord abbaMissing = repository.GetById(abbaBefore.Id)
        ?? throw new InvalidOperationException("Missing track record was deleted.");

    Console.WriteLine($"ABBA marked unavailable: {!abbaMissing.IsAvailable}");
    Console.WriteLine($"ABBA ID preserved: {abbaMissing.Id == abbaBefore.Id}");
    Console.WriteLine();

    // Pass 5 - Moved file
    string fleetwoodOriginalPath = filePaths[2];
    string classicsDirectory = Path.Combine(directoryPath, "Classics");
    string fleetwoodMovedPath = Path.Combine(
        classicsDirectory,
        Path.GetFileName(fleetwoodOriginalPath));

    MediaTrackRecord fleetwoodBefore = repository.GetByFilePath(fleetwoodOriginalPath)
        ?? throw new InvalidOperationException("Fleetwood Mac track not found.");

    Directory.CreateDirectory(classicsDirectory);
    File.Move(fleetwoodOriginalPath, fleetwoodMovedPath);

    string[] movedPaths = Directory.GetFiles(
        directoryPath,
        "*.mp3",
        SearchOption.AllDirectories);

    (LibraryReconciliationResult fifthReconciliation, IncrementalScanSummary fifthScan) =
        orchestrationService.ProcessLibrary(
            sourceId,
            movedPaths,
            discoveryCompletedSuccessfully: true,
            metadataReader);

    PrintSummary("Pass 5 - Moved File Scan", fifthScan, metadataReader.ReadCount);
    PrintReconciliation(fifthReconciliation);

    MediaTrackRecord fleetwoodAfter = repository.GetById(fleetwoodBefore.Id)
        ?? throw new InvalidOperationException("Moved track record not found.");

    Console.WriteLine($"Fleetwood Mac ID preserved: {fleetwoodAfter.Id == fleetwoodBefore.Id}");
    Console.WriteLine($"Path updated: {fleetwoodAfter.FilePath == fleetwoodMovedPath}");
    Console.WriteLine();

    // Pass 6 - Restore previously missing file
    File.WriteAllText(missingPath, "Restored demo media content");

    string[] restoredPaths = Directory.GetFiles(
        directoryPath,
        "*.mp3",
        SearchOption.AllDirectories);

    (LibraryReconciliationResult sixthReconciliation, IncrementalScanSummary sixthScan) =
        orchestrationService.ProcessLibrary(
            sourceId,
            restoredPaths,
            discoveryCompletedSuccessfully: true,
            metadataReader);

    PrintSummary("Pass 6 - Restored File Scan", sixthScan, metadataReader.ReadCount);
    PrintReconciliation(sixthReconciliation);

    MediaTrackRecord abbaRestored = repository.GetById(abbaBefore.Id)
        ?? throw new InvalidOperationException("Restored track record not found.");

    Console.WriteLine($"ABBA available again: {abbaRestored.IsAvailable}");
    Console.WriteLine($"ABBA ID preserved: {abbaRestored.Id == abbaBefore.Id}");
    Console.WriteLine();

    // Demonstrate safe handling of failed discovery
    LibraryReconciliationResult failedDiscovery = orchestrationService.Reconcile(
        sourceId,
        [],
        discoveryCompletedSuccessfully: false);

    Console.WriteLine("Failed Discovery Verification");
    Console.WriteLine($"Scan completed: {failedDiscovery.ScanCompletedSuccessfully}");
    Console.WriteLine($"Tracks classified as missing: {failedDiscovery.MissingTrackCount}");
    Console.WriteLine();

    // Final database state
    IReadOnlyList<MediaTrackRecord> finalTracks = repository.GetBySourceId(sourceId);

    Console.WriteLine("Final Library State");

    foreach (MediaTrackRecord track in finalTracks)
    {
        Console.WriteLine(
            $"ID: {track.Id} | Available: {track.IsAvailable} | {track.FilePath}");
    }

    Console.WriteLine();
    Console.WriteLine($"Total stored tracks: {finalTracks.Count}");
}
finally
{
    if (Directory.Exists(directoryPath))
    {
        Directory.Delete(directoryPath, recursive: true);
    }

    if (File.Exists(databasePath))
    {
        File.Delete(databasePath);
    }
}

static void PrintSummary(
    string title,
    IncrementalScanSummary summary,
    int metadataReads)
{
    Console.WriteLine(title);
    Console.WriteLine($"Total:   {summary.Total}");
    Console.WriteLine($"Added:   {summary.Added}");
    Console.WriteLine($"Updated: {summary.Updated}");
    Console.WriteLine($"Skipped: {summary.Skipped}");
    Console.WriteLine($"Failed:  {summary.Failed}");
    Console.WriteLine($"Metadata reads: {metadataReads}");
    Console.WriteLine();
}

static void PrintReconciliation(LibraryReconciliationResult result)
{
    Console.WriteLine("Reconciliation");
    Console.WriteLine($"Stored tracks: {result.StoredTrackCount}");
    Console.WriteLine($"Scanned paths: {result.ScannedTrackCount}");
    Console.WriteLine($"Missing tracks: {result.MissingTrackCount}");
    Console.WriteLine($"Moved tracks: {result.MovedTracks.Count}");
    Console.WriteLine();
}
