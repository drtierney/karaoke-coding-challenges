
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class LibraryScanOrchestrationServiceTests
{
    private sealed class FailingKaraokeFileRepository : IKaraokeFileRepository
    {
        public string? GetByTrackId(long trackId)
        {
            return null;
        }

        public void Upsert(long trackId, string cdgFilePath)
        {
            throw new InvalidOperationException("Non-transactional CDG persistence was attempted.");
        }

        public void Upsert(
            long trackId,
            string cdgFilePath,
            SqliteConnection connection,
            SqliteTransaction transaction)
        {
            throw new InvalidOperationException("Simulated CDG persistence failure.");
        }

        public void DeleteByTrackId(long trackId)
        {
            throw new NotImplementedException();
        }
    }

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

            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

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

            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

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

            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

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

            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

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

            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

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


    [Fact]
    public void ProcessLibrary_WhenKaraokeFilesContainCdg_OnlyProcessesMp3Tracks()
    {
        // Arrange
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-cdg-filter-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string mp3Path = Path.Combine(tempDirectory, "Queen.mp3");
        string cdgPath = Path.Combine(tempDirectory, "Queen.cdg");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(mp3Path, "Simulated MP3 content");
            File.WriteAllText(cdgPath, "Simulated CDG content");

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

            ScannedTrackPersistenceService persistenceService =
                new(trackRepository, connectionString);

            KaraokeFileRepository karaokeFileRepository =
                new(connectionString);

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

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

            CountingMetadataReader metadataReader = new();

            // Act
            (LibraryReconciliationResult reconciliation,
             IncrementalScanSummary scan) = service.ProcessLibrary(
                sourceId,
                [mp3Path, cdgPath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Assert - Reconciliation
            Assert.True(reconciliation.ScanCompletedSuccessfully);
            Assert.Equal(1, reconciliation.ScannedTrackCount);
            Assert.Empty(reconciliation.MissingTracks);
            Assert.Empty(reconciliation.MovedTracks);

            // Assert - Incremental scanning
            Assert.Equal(1, scan.Total);
            Assert.Equal(1, scan.Added);
            Assert.Equal(0, scan.Updated);
            Assert.Equal(0, scan.Skipped);
            Assert.Equal(0, scan.Failed);

            Assert.Equal(1, metadataReader.ReadCount);

            // Assert - Database persistence
            MediaTrackRecord? mp3Track = trackRepository.GetByFilePath(mp3Path);
            MediaTrackRecord? cdgTrack = trackRepository.GetByFilePath(cdgPath);

            Assert.NotNull(mp3Track);
            Assert.True(mp3Track.IsKaraoke);
            Assert.Null(cdgTrack);

            Assert.Single(trackRepository.GetBySourceId(sourceId));
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
    public void ProcessLibrary_WhenCdgIsAddedToUnchangedMp3_CreatesRelationshipWithoutRescanning()
    {
        // Arrange
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-cdg-added-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string mp3Path = Path.Combine(tempDirectory, "Queen.mp3");
        string cdgPath = Path.Combine(tempDirectory, "Queen.cdg");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(mp3Path, "Simulated MP3 content");

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

            ScannedTrackPersistenceService persistenceService =
                new(trackRepository, connectionString);

            KaraokeFileRepository karaokeFileRepository =
                new(connectionString);

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

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

            CountingMetadataReader metadataReader = new();

            // Act - First scan: MP3 only
            var initialResult = service.ProcessLibrary(
                sourceId,
                [mp3Path],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            MediaTrackRecord? originalTrack = trackRepository.GetByFilePath(mp3Path);

            Assert.NotNull(originalTrack);

            long originalTrackId = originalTrack.Id;

            Assert.Equal(1, initialResult.Scan.Added);
            Assert.Equal(1, metadataReader.ReadCount);
            Assert.Null(karaokeFileRepository.GetByTrackId(originalTrackId));

            // Act - Add the CDG companion without modifying the MP3
            File.WriteAllText(cdgPath, "Simulated CDG content");

            var secondResult = service.ProcessLibrary(
                sourceId,
                [mp3Path, cdgPath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Assert - MP3 was not rescanned
            Assert.Equal(1, secondResult.Scan.Total);
            Assert.Equal(1, secondResult.Scan.Skipped);
            Assert.Equal(0, secondResult.Scan.Added);
            Assert.Equal(0, secondResult.Scan.Updated);
            Assert.Equal(0, secondResult.Scan.Failed);

            Assert.Equal(1, metadataReader.ReadCount);

            // Assert - Existing track ID was preserved
            MediaTrackRecord? updatedTrack = trackRepository.GetByFilePath(mp3Path);

            Assert.NotNull(updatedTrack);
            Assert.Equal(originalTrackId, updatedTrack.Id);
            Assert.True(updatedTrack.IsAvailable);

            // Assert - CDG relationship was created
            Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(originalTrackId));

            // Assert - CDG was not created as a separate track
            Assert.Null(trackRepository.GetByFilePath(cdgPath));
            Assert.Single(trackRepository.GetBySourceId(sourceId));
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
    public void ProcessLibrary_WhenCdgIsRemovedFromUnchangedMp3_RemovesRelationshipWithoutRescanning()
    {
        // Arrange
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-cdg-removed-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string mp3Path = Path.Combine(tempDirectory, "Queen.mp3");
        string cdgPath = Path.Combine(tempDirectory, "Queen.cdg");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(mp3Path, "Simulated MP3 content");
            File.WriteAllText(cdgPath, "Simulated CDG content");

            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);
            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = tempDirectory,
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            FileFingerprintProvider fingerprintProvider = new();
            FileChangeDetector changeDetector = new();

            ScannedTrackPersistenceService persistenceService =
                new(trackRepository, connectionString);

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

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

            CountingMetadataReader metadataReader = new();

            // Act - First scan: MP3 + CDG
            var initialResult = service.ProcessLibrary(
                sourceId,
                [mp3Path, cdgPath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            MediaTrackRecord? originalTrack = trackRepository.GetByFilePath(mp3Path);

            Assert.NotNull(originalTrack);

            long originalTrackId = originalTrack.Id;
            long? originalFileSize = originalTrack.FileSize;
            DateTime? originalLastModifiedUtc = originalTrack.LastModifiedUtc;

            // Assert - Initial relationship exists
            Assert.Equal(1, initialResult.Scan.Added);
            Assert.Equal(1, metadataReader.ReadCount);

            Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(originalTrackId));

            // Act - Remove CDG, leaving the MP3 unchanged
            File.Delete(cdgPath);

            var secondResult = service.ProcessLibrary(
                sourceId,
                [mp3Path],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Assert - MP3 was not rescanned
            Assert.Equal(1, secondResult.Scan.Total);
            Assert.Equal(1, secondResult.Scan.Skipped);
            Assert.Equal(0, secondResult.Scan.Added);
            Assert.Equal(0, secondResult.Scan.Updated);
            Assert.Equal(0, secondResult.Scan.Failed);

            Assert.Equal(1, metadataReader.ReadCount);

            // Assert - Original MP3 track was preserved
            MediaTrackRecord? updatedTrack = trackRepository.GetByFilePath(mp3Path);

            Assert.NotNull(updatedTrack);
            Assert.Equal(originalTrackId, updatedTrack.Id);
            Assert.True(updatedTrack.IsAvailable);
            Assert.True(updatedTrack.IsKaraoke);

            // Assert - Fingerprint remains unchanged
            Assert.Equal(originalFileSize, updatedTrack.FileSize);
            Assert.Equal(originalLastModifiedUtc, updatedTrack.LastModifiedUtc);

            // Assert - CDG relationship was removed
            Assert.Null(karaokeFileRepository.GetByTrackId(originalTrackId));

            // Assert - MP3 remains the only track
            Assert.Single(trackRepository.GetBySourceId(sourceId));
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
    public void ProcessLibrary_WhenNewMp3CdgPairIsDiscovered_PersistsBothWithFingerprint()
    {
        // Arrange
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-transaction-success-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string mp3Path = Path.Combine(tempDirectory, "Queen.mp3");
        string cdgPath = Path.Combine(tempDirectory, "Queen.cdg");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(mp3Path, "Simulated MP3 content");
            File.WriteAllText(cdgPath, "Simulated CDG content");

            MediaLibraryDatabaseInitializer initializer = new(connectionString);
            initializer.Initialize();

            LibrarySourceRepository sourceRepository = new(connectionString);
            MediaTrackRepository trackRepository = new(connectionString);
            KaraokeFileRepository karaokeFileRepository = new(connectionString);

            long sourceId = sourceRepository.Add(new LibrarySourceRecord
            {
                Path = tempDirectory,
                Type = LibrarySourceType.Karaoke,
                Enabled = true
            });

            FileFingerprintProvider fingerprintProvider = new();
            FileFingerprint expectedFingerprint = fingerprintProvider.GetFingerprint(mp3Path);

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

            LibraryReconciliationPersistenceService reconciliationPersistenceService =
                new(connectionString);

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                karaokeFileRepository);

            CountingMetadataReader metadataReader = new();

            // Act - Initial scan
            var result = service.ProcessLibrary(
                sourceId,
                [mp3Path, cdgPath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Assert - Initial scan
            Assert.True(result.Reconciliation.ScanCompletedSuccessfully);

            Assert.Equal(1, result.Scan.Total);
            Assert.Equal(1, result.Scan.Added);
            Assert.Equal(0, result.Scan.Updated);
            Assert.Equal(0, result.Scan.Skipped);
            Assert.Equal(0, result.Scan.Failed);

            Assert.Equal(1, metadataReader.ReadCount);

            // Assert - MP3 persisted
            MediaTrackRecord? track = trackRepository.GetByFilePath(mp3Path);

            Assert.NotNull(track);
            Assert.Equal(sourceId, track.SourceId);
            Assert.True(track.IsKaraoke);
            Assert.True(track.IsAvailable);

            // Assert - Fingerprint persisted
            Assert.Equal(expectedFingerprint.FileSize, track.FileSize);
            Assert.Equal(expectedFingerprint.LastModifiedUtc, track.LastModifiedUtc);

            // Assert - CDG relationship persisted
            Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(track.Id));

            // Assert - CDG is not a separate track
            Assert.Null(trackRepository.GetByFilePath(cdgPath));
            Assert.Single(trackRepository.GetBySourceId(sourceId));

            // Act - Repeat scan without changing either file
            var secondResult = service.ProcessLibrary(
                sourceId,
                [mp3Path, cdgPath],
                discoveryCompletedSuccessfully: true,
                metadataReader,
                isKaraoke: true);

            // Assert - No unnecessary metadata scan
            Assert.Equal(1, secondResult.Scan.Total);
            Assert.Equal(1, secondResult.Scan.Skipped);
            Assert.Equal(0, secondResult.Scan.Added);
            Assert.Equal(0, secondResult.Scan.Updated);
            Assert.Equal(0, secondResult.Scan.Failed);

            Assert.Equal(1, metadataReader.ReadCount);

            // Assert - Relationship and track ID preserved
            MediaTrackRecord? trackAfter = trackRepository.GetByFilePath(mp3Path);

            Assert.NotNull(trackAfter);
            Assert.Equal(track.Id, trackAfter.Id);

            Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(trackAfter.Id));
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
    public void ProcessLibrary_WhenCdgPersistenceFails_RollsBackMp3()
    {
        // Arrange
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"karaoke-transaction-failure-{Guid.NewGuid():N}");

        string databasePath = Path.Combine(tempDirectory, "library.db");
        string connectionString = $"Data Source={databasePath};Pooling=False";

        string mp3Path = Path.Combine(tempDirectory, "Queen.mp3");
        string cdgPath = Path.Combine(tempDirectory, "Queen.cdg");

        Directory.CreateDirectory(tempDirectory);

        try
        {
            File.WriteAllText(mp3Path, "Simulated MP3 content");
            File.WriteAllText(cdgPath, "Simulated CDG content");

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

            ScannedTrackPersistenceService persistenceService = new(trackRepository, connectionString);

            // Simulate failure during transactional CDG persistence.
            FailingKaraokeFileRepository failingRepository = new();

            KaraokeFilePairingService pairingService = new();

            KaraokeLibraryPersistenceService karaokePersistenceService = new(
                persistenceService,
                failingRepository,
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

            LibraryScanOrchestrationService service = new(
                trackRepository,
                fingerprintProvider,
                reconciliationService,
                reconciliationPersistenceService,
                incrementalScanService,
                failingRepository);

            CountingMetadataReader metadataReader = new();

            // Act - CDG persistence throws during the transaction.
            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(() =>
                    service.ProcessLibrary(
                        sourceId,
                        [mp3Path, cdgPath],
                        discoveryCompletedSuccessfully: true,
                        metadataReader,
                        isKaraoke: true));

            // Assert - Expected failure
            Assert.Equal("Simulated CDG persistence failure.", exception.Message);

            Assert.Equal(1, metadataReader.ReadCount);

            // Assert - MP3 insertion was rolled back
            Assert.Null(trackRepository.GetByFilePath(mp3Path));

            // Assert - No CDG relationship was persisted
            Assert.Empty(trackRepository.GetBySourceId(sourceId));

            // Verify the database directly using the real repository.
            Assert.Null(trackRepository.GetByFilePath(cdgPath));
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
