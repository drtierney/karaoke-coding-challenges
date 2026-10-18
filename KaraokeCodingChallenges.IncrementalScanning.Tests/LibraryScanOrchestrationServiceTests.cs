
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class LibraryScanOrchestrationServiceTests
{
    [Fact]
    public void Reconcile_DoesNotModifyTracks_WhenDiscoveryFails()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.Combine("Library", "Karaoke"),
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long queenId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = Path.Combine("Library", "Karaoke", "Queen.mp3"),
                DurationSeconds = 354,
                IsKaraoke = true,
                IsAvailable = true
            });

            FileFingerprintProvider fingerprintProvider = new();
            FileChangeDetector changeDetector = new();

            ScannedTrackPersistenceService scannedTrackPersistenceService = new(trackRepository, connectionString);

            IncrementalScanService incrementalScanService = new(
                trackRepository,
                fingerprintProvider,
                changeDetector,
                scannedTrackPersistenceService);

            LibraryReconciliationService reconciliationService = new();

            LibraryReconciliationPersistenceService reconciliationPersistenceService = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService);

            LibraryReconciliationResult result = service.Reconcile(
                sourceId,
                [],
                discoveryCompletedSuccessfully: false);

            Assert.False(result.ScanCompletedSuccessfully);

            Assert.Empty(result.MissingTracks);
            Assert.Empty(result.PresentTracks);
            Assert.Empty(result.MovedTracks);

            MediaTrackRecord? queenAfter = trackRepository.GetById(queenId);

            Assert.NotNull(queenAfter);

            Assert.True(queenAfter.IsAvailable);
            Assert.Equal(queenId, queenAfter.Id);
            Assert.Equal(Path.Combine("Library", "Karaoke", "Queen.mp3"), queenAfter.FilePath);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void Reconcile_MarksMissingTracksUnavailable_WhenDiscoverySucceeds()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        try
        {
            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = Path.Combine("Library", "Karaoke"),
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            string queenPath = Path.Combine("Library", "Karaoke", "Queen.mp3");
            string musePath = Path.Combine("Library", "Karaoke", "Muse.mp3");

            long queenId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = queenPath,
                DurationSeconds = 354,
                IsKaraoke = true,
                IsAvailable = true
            });

            long museId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Feeling Good",
                Artist = "Muse",
                FilePath = musePath,
                DurationSeconds = 218,
                IsKaraoke = true,
                IsAvailable = true
            });

            FileFingerprintProvider fingerprintProvider = new();
            FileChangeDetector changeDetector = new();

            ScannedTrackPersistenceService scannedTrackPersistenceService = new(trackRepository, connectionString);

            IncrementalScanService incrementalScanService = new(
                trackRepository,
                fingerprintProvider,
                changeDetector,
                scannedTrackPersistenceService);

            LibraryReconciliationService reconciliationService = new();

            LibraryReconciliationPersistenceService reconciliationPersistenceService =
                new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService);

            LibraryReconciliationResult result = service.Reconcile(
                sourceId,
                [queenPath],
                discoveryCompletedSuccessfully: true);

            // Reconciliation result
            Assert.True(result.ScanCompletedSuccessfully);
            Assert.Equal(2, result.StoredTrackCount);
            Assert.Equal(1, result.ScannedTrackCount);

            Assert.Equal(museId, Assert.Single(result.MissingTracks).Id);
            Assert.Equal(queenId, Assert.Single(result.PresentTracks).Id);
            Assert.Empty(result.MovedTracks);

            // Persisted database state
            MediaTrackRecord? queenAfter = trackRepository.GetById(queenId);
            MediaTrackRecord? museAfter = trackRepository.GetById(museId);

            Assert.NotNull(queenAfter);
            Assert.NotNull(museAfter);

            Assert.True(queenAfter.IsAvailable);
            Assert.False(museAfter.IsAvailable);

            Assert.Equal(queenId, queenAfter.Id);
            Assert.Equal(museId, museAfter.Id);
            Assert.Equal(queenPath, queenAfter.FilePath);
            Assert.Equal(musePath, museAfter.FilePath);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public void Reconcile_PreservesTrackId_WhenFileMoves()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-reconciliation-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string originalPath = Path.Combine(tempDirectory, "Queen.mp3");
        string classicsDirectory = Path.Combine(tempDirectory, "Classics");
        string movedPath = Path.Combine(classicsDirectory, "Queen.mp3");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(originalPath, "Test karaoke audio content");

            FileFingerprintProvider fingerprintProvider = new();
            FileFingerprint originalFingerprint = fingerprintProvider.GetFingerprint(originalPath);

            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = tempDirectory,
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            long trackId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                FilePath = originalPath,
                DurationSeconds = 354,
                IsKaraoke = true,
                IsAvailable = true,
                FileSize = originalFingerprint.FileSize,
                LastModifiedUtc = originalFingerprint.LastModifiedUtc
            });

            FileChangeDetector changeDetector = new();

            ScannedTrackPersistenceService scannedTrackPersistenceService = new(trackRepository, connectionString);

            IncrementalScanService incrementalScanService = new(
                trackRepository,
                fingerprintProvider,
                changeDetector,
                scannedTrackPersistenceService);

            LibraryReconciliationService reconciliationService = new();

            LibraryReconciliationPersistenceService reconciliationPersistenceService = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService);

            // Move the physical file
            Directory.CreateDirectory(classicsDirectory);
            File.Move(originalPath, movedPath);

            FileFingerprint movedFingerprint = fingerprintProvider.GetFingerprint(movedPath);

            Assert.Equal(originalFingerprint.FileSize, movedFingerprint.FileSize);
            Assert.Equal(originalFingerprint.LastModifiedUtc, movedFingerprint.LastModifiedUtc);

            CountingMetadataReader metadataReader = new();

            (LibraryReconciliationResult result, IncrementalScanSummary scanSummary) = service.ProcessLibrary(
                sourceId,
                [movedPath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Reconciliation result
            Assert.True(result.ScanCompletedSuccessfully);
            Assert.Empty(result.MissingTracks);
            Assert.Empty(result.PresentTracks);

            FileMoveMatch move = Assert.Single(result.MovedTracks);

            Assert.Equal(trackId, move.TrackId);
            Assert.Equal(originalPath, move.OldFilePath);
            Assert.Equal(movedPath, move.NewFilePath);

            // Persisted database state
            MediaTrackRecord? updatedTrack = trackRepository.GetById(trackId);

            Assert.NotNull(updatedTrack);

            Assert.Equal(trackId, updatedTrack.Id);
            Assert.Equal(sourceId, updatedTrack.SourceId);
            Assert.Equal(movedPath, updatedTrack.FilePath);
            Assert.True(updatedTrack.IsAvailable);

            Assert.Equal("Bohemian Rhapsody", updatedTrack.Title);
            Assert.Equal("Queen", updatedTrack.Artist);

            Assert.Equal(originalFingerprint.FileSize, updatedTrack.FileSize);
            Assert.Equal(originalFingerprint.LastModifiedUtc, updatedTrack.LastModifiedUtc);

            // Verify no duplicate track was created
            IReadOnlyList<MediaTrackRecord> storedTracks = trackRepository.GetBySourceId(sourceId);

            MediaTrackRecord onlyTrack = Assert.Single(storedTracks);

            Assert.Equal(trackId, onlyTrack.Id);

            // Verify incremental scanning recognises the moved track.
            Assert.Equal(1, scanSummary.Total);
            Assert.Equal(1, scanSummary.Skipped);
            Assert.Equal(0, scanSummary.Added);
            Assert.Equal(0, scanSummary.Updated);
            Assert.Equal(0, scanSummary.Failed);

            // The fingerprint matches, so no metadata extraction is needed.
            Assert.Equal(0, metadataReader.ReadCount);

            // The existing record must still be preserved.
            IReadOnlyList<MediaTrackRecord> tracksAfterScan = trackRepository.GetBySourceId(sourceId);

            MediaTrackRecord preservedTrack = Assert.Single(tracksAfterScan);

            Assert.Equal(trackId, preservedTrack.Id);
            Assert.Equal(movedPath, preservedTrack.FilePath);
            Assert.True(preservedTrack.IsAvailable);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public void Reconcile_AllowsNewTracksToBePersistedByIncrementalScanning()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-new-track-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";
        string filePath = Path.Combine(tempDirectory, "Queen.mp3");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(filePath, "Test karaoke audio content");

            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = tempDirectory,
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            FileFingerprintProvider fingerprintProvider = new();
            FileChangeDetector changeDetector = new();

            ScannedTrackPersistenceService scannedTrackPersistenceService = new(trackRepository, connectionString);

            IncrementalScanService incrementalScanService = new(
                trackRepository,
                fingerprintProvider,
                changeDetector,
                scannedTrackPersistenceService);

            LibraryReconciliationService reconciliationService = new();

            LibraryReconciliationPersistenceService reconciliationPersistenceService = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService);

            CountingMetadataReader metadataReader = new();

            // Reconcile and process genuinely new files
            (LibraryReconciliationResult result, IncrementalScanSummary scanSummary) = service.ProcessLibrary(
                sourceId,
                [filePath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Reconciliation
            Assert.True(result.ScanCompletedSuccessfully);
            Assert.Equal(0, result.StoredTrackCount);
            Assert.Equal(1, result.ScannedTrackCount);
            Assert.Empty(result.MissingTracks);
            Assert.Empty(result.PresentTracks);
            Assert.Empty(result.MovedTracks);

            // Incremental scanning
            Assert.Equal(1, scanSummary.Total);
            Assert.Equal(1, scanSummary.Added);
            Assert.Equal(0, scanSummary.Updated);
            Assert.Equal(0, scanSummary.Skipped);
            Assert.Equal(0, scanSummary.Failed);

            Assert.Equal(1, metadataReader.ReadCount);

            // Database persistence
            IReadOnlyList<MediaTrackRecord> storedTracks = trackRepository.GetBySourceId(sourceId);

            MediaTrackRecord storedTrack = Assert.Single(storedTracks);

            Assert.Equal(sourceId, storedTrack.SourceId);
            Assert.Equal(filePath, storedTrack.FilePath);
            Assert.True(storedTrack.IsAvailable);
            Assert.NotNull(storedTrack.FileSize);
            Assert.NotNull(storedTrack.LastModifiedUtc);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public void ProcessLibrary_DoesNotMatchAmbiguousMovedFiles()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-ambiguous-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string newPath = Path.Combine(tempDirectory, "New.mp3");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(newPath, "Test karaoke audio content");

            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = tempDirectory,
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            FileFingerprintProvider fingerprintProvider = new();
            FileFingerprint fingerprint = fingerprintProvider.GetFingerprint(newPath);

            string firstOldPath = Path.Combine(tempDirectory, "Old1.mp3");
            string secondOldPath = Path.Combine(tempDirectory, "Old2.mp3");

            long firstId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "First Track",
                Artist = "Queen",
                FilePath = firstOldPath,
                DurationSeconds = 180,
                IsKaraoke = true,
                FileSize = fingerprint.FileSize,
                LastModifiedUtc = fingerprint.LastModifiedUtc
            });

            long secondId = trackRepository.Add(new MediaTrackRecord
            {
                SourceId = sourceId,
                Title = "Second Track",
                Artist = "Muse",
                FilePath = secondOldPath,
                DurationSeconds = 200,
                IsKaraoke = true,
                FileSize = fingerprint.FileSize,
                LastModifiedUtc = fingerprint.LastModifiedUtc
            });

            FileChangeDetector changeDetector = new();

            ScannedTrackPersistenceService scannedTrackPersistenceService = new(trackRepository, connectionString);

            IncrementalScanService incrementalScanService = new(
                trackRepository,
                fingerprintProvider,
                changeDetector,
                scannedTrackPersistenceService);

            LibraryReconciliationService reconciliationService = new();

            LibraryReconciliationPersistenceService reconciliationPersistenceService = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService);

            CountingMetadataReader metadataReader = new();

            (LibraryReconciliationResult result, IncrementalScanSummary scanSummary) =
                service.ProcessLibrary(
                    sourceId,
                    [newPath],
                    discoveryCompletedSuccessfully: true,
                    metadataReader,
                    isKaraoke: true);

            // No ambiguous move accepted
            Assert.True(result.ScanCompletedSuccessfully);
            Assert.Empty(result.MovedTracks);
            Assert.Equal(2, result.MissingTrackCount);

            Assert.Contains(result.MissingTracks, track => track.Id == firstId);
            Assert.Contains(result.MissingTracks, track => track.Id == secondId);

            // Both original records are preserved but unavailable.
            MediaTrackRecord? firstAfter = trackRepository.GetById(firstId);
            MediaTrackRecord? secondAfter = trackRepository.GetById(secondId);

            Assert.NotNull(firstAfter);
            Assert.NotNull(secondAfter);

            Assert.False(firstAfter.IsAvailable);
            Assert.False(secondAfter.IsAvailable);

            Assert.Equal(firstOldPath, firstAfter.FilePath);
            Assert.Equal(secondOldPath, secondAfter.FilePath);

            // The unmatched new file is inserted as a separate track.
            Assert.Equal(1, scanSummary.Added);
            Assert.Equal(0, scanSummary.Failed);
            Assert.Equal(1, metadataReader.ReadCount);

            IReadOnlyList<MediaTrackRecord> storedTracks = trackRepository.GetBySourceId(sourceId);

            Assert.Equal(3, storedTracks.Count);

            MediaTrackRecord newTrack = Assert.Single(storedTracks, track => track.FilePath == newPath);

            Assert.NotEqual(firstId, newTrack.Id);
            Assert.NotEqual(secondId, newTrack.Id);
            Assert.True(newTrack.IsAvailable);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }
    }

}
