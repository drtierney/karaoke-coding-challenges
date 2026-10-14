
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Demo;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;

string databasePath = Path.Combine(Path.GetTempPath(), $"incremental-demo-{Guid.NewGuid():N}.db");

string connectionString = $"Data Source={databasePath};Pooling=False";

string directoryPath = Path.Combine(Path.GetTempPath(), $"incremental-demo-files-{Guid.NewGuid():N}");

try
{
    // Create a temporary media library.
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

    // Initialize SQLite.
    MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(connectionString);

    initializer.Initialize();

    LibrarySourceRepository sourceRepository = new LibrarySourceRepository(connectionString);

    long sourceId = sourceRepository.Add(new LibrarySourceRecord
    {
        Path = directoryPath,
        Type = LibrarySourceType.Music,
        Enabled = true
    });

    // Configure incremental scanning.
    MediaTrackRepository repository = new MediaTrackRepository(connectionString);

    FileFingerprintProvider fingerprintProvider = new FileFingerprintProvider();

    FileChangeDetector changeDetector = new FileChangeDetector();

    ScannedTrackPersistenceService persistenceService = new ScannedTrackPersistenceService(repository, connectionString);

    IncrementalScanService service = new IncrementalScanService(
        repository,
        fingerprintProvider,
        changeDetector,
        persistenceService);

    DemoMetadataReader metadataReader = new DemoMetadataReader();

    Console.WriteLine("Challenge 040 - Incremental Library Scanning Demo");
    Console.WriteLine();

    // Pass 1 - Initial scan
    IncrementalScanSummary firstPass = service.ProcessFiles(
        filePaths,
        sourceId,
        metadataReader);

    PrintSummary("Pass 1 - Initial Scan", firstPass, metadataReader.ReadCount);

    // Pass 2 - Repeat scan without changes
    IncrementalScanSummary secondPass = service.ProcessFiles(
        filePaths,
        sourceId,
        metadataReader);

    PrintSummary("Pass 2 - Repeat Scan", secondPass, metadataReader.ReadCount);


    // Pass 3 - Modify one file and scan again
    string modifiedFilePath = filePaths[0];

    MediaTrackRecord? originalTrack = repository.GetByFilePath(modifiedFilePath);

    if (originalTrack is null)
    {
        throw new InvalidOperationException("Expected track was not found after the initial scan.");
    }

    long originalTrackId = originalTrack.Id;
    long originalFileSize = originalTrack.FileSize ?? 0;

    // Change the contents to trigger a new fingerprint.
    File.WriteAllText(modifiedFilePath, "Demo media content - Modified");

    IncrementalScanSummary thirdPass = service.ProcessFiles(
        filePaths,
        sourceId,
        metadataReader);

    PrintSummary("Pass 3 - Modified File Scan", thirdPass, metadataReader.ReadCount);

    MediaTrackRecord? updatedTrack = repository.GetByFilePath(modifiedFilePath);

    if (updatedTrack is null)
    {
        throw new InvalidOperationException("Modified track was not found after rescanning.");
    }

    Console.WriteLine("Modified Track Verification");
    Console.WriteLine($"Track ID preserved: {originalTrackId == updatedTrack.Id}");
    Console.WriteLine($"Original file size: {originalFileSize} bytes");
    Console.WriteLine($"Updated file size:  {updatedTrack.FileSize} bytes");
    Console.WriteLine();

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
