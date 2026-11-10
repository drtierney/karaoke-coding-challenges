using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibraryDatabase.Services;
using KaraokeCodingChallenges.MediaMetadata;
using KaraokeCodingChallenges.ScanResult;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests;

public class KaraokeLibraryPersistenceServiceTests
{
    private sealed class FailingKaraokeFileRepository : IKaraokeFileRepository
    {
        public string? GetByTrackId(long trackId)
        {
            throw new NotImplementedException();
        }

        public void Upsert(long trackId, string cdgFilePath)
        {
            throw new NotImplementedException();
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
    public void PersistPair_WhenFilesMatch_PersistsTrackAndCdgRelationship()
    {
        using MediaLibraryDatabaseTestContext context = new ();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        string mp3Path = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.cdg");

        MediaTrackRepository mediaTrackRepository = new (context.ConnectionString);
        ScannedTrackPersistenceService scannedTrackPersistenceService = new (mediaTrackRepository, context.ConnectionString);
        KaraokeFileRepository karaokeFileRepository = new (context.ConnectionString);
        KaraokeFilePairingService karaokeFilePairingService = new();

        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = mp3Path,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = mp3Path,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        KaraokeLibraryPersistenceService karaokeLibraryPersistenceService = new(scannedTrackPersistenceService, karaokeFileRepository, karaokeFilePairingService, context.ConnectionString);

        long? trackId = karaokeLibraryPersistenceService.PersistPair(scanResult, cdgPath, sourceId);

        Assert.NotNull(trackId);

        MediaTrackRecord? track = mediaTrackRepository.GetById(trackId.Value);

        Assert.NotNull(track);
        Assert.True(track.IsKaraoke);

        Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(trackId.Value));
    }

    [Fact]
    public void PersistPair_WhenFileNamesDiffer_DoesNotPersistTrackOrCdgRelationship()
    {
        using MediaLibraryDatabaseTestContext context = new();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        string mp3Path = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen - We Will Rock You.cdg");

        MediaTrackRepository mediaTrackRepository = new(context.ConnectionString);
        ScannedTrackPersistenceService scannedTrackPersistenceService = new(mediaTrackRepository, context.ConnectionString);
        KaraokeFileRepository karaokeFileRepository = new(context.ConnectionString);
        KaraokeFilePairingService karaokeFilePairingService = new();

        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = mp3Path,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = mp3Path,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        KaraokeLibraryPersistenceService karaokeLibraryPersistenceService = new(scannedTrackPersistenceService, karaokeFileRepository, karaokeFilePairingService, context.ConnectionString);

        long? trackId = karaokeLibraryPersistenceService.PersistPair(scanResult, cdgPath, sourceId);

        Assert.Null(trackId);

        Assert.Null(mediaTrackRepository.GetByFilePath(mp3Path));
    }

    [Fact]
    public void PersistPair_WhenScanFails_DoesNotPersistTrack()
    {
        using MediaLibraryDatabaseTestContext context = new();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        string mp3Path = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.cdg");

        MediaTrackRepository mediaTrackRepository = new(context.ConnectionString);
        ScannedTrackPersistenceService scannedTrackPersistenceService = new(mediaTrackRepository, context.ConnectionString);
        KaraokeFileRepository karaokeFileRepository = new(context.ConnectionString);
        KaraokeFilePairingService karaokeFilePairingService = new();

        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = mp3Path,
            IsSuccess = false,
            Metadata = null
        };

        KaraokeLibraryPersistenceService karaokeLibraryPersistenceService = new(scannedTrackPersistenceService, karaokeFileRepository, karaokeFilePairingService, context.ConnectionString);
        long? trackId = karaokeLibraryPersistenceService.PersistPair(scanResult, cdgPath, sourceId);

        Assert.Null(trackId);

        Assert.Null(mediaTrackRepository.GetByFilePath(mp3Path));
    }

    [Fact]
    public void PersistPair_WhenTrackAlreadyExists_UpdatesWithoutDuplication()
    {
        using MediaLibraryDatabaseTestContext context = new();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        string mp3Path = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.cdg");

        MediaTrackRepository mediaTrackRepository = new(context.ConnectionString);
        ScannedTrackPersistenceService scannedTrackPersistenceService = new(mediaTrackRepository, context.ConnectionString);
        KaraokeFileRepository karaokeFileRepository = new(context.ConnectionString);
        KaraokeFilePairingService karaokeFilePairingService = new();

        MediaScanResult scanResult = new MediaScanResult
        {
            FilePath = mp3Path,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = mp3Path,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        KaraokeLibraryPersistenceService karaokeLibraryPersistenceService = new(scannedTrackPersistenceService, karaokeFileRepository, karaokeFilePairingService, context.ConnectionString);

        long? originalTrackId = karaokeLibraryPersistenceService.PersistPair(scanResult, cdgPath, sourceId);

        MediaScanResult updatedScanResult = scanResult with
        {
            Metadata = scanResult.Metadata! with
            {
                Title = "Bohemian Rhapsody - Remastered"
            }
        };

        long? updatedTrackId = karaokeLibraryPersistenceService.PersistPair(updatedScanResult, cdgPath, sourceId);

        Assert.NotNull(originalTrackId);
        Assert.NotNull(updatedTrackId);

        Assert.Equal(originalTrackId, updatedTrackId);

        MediaTrackRecord? updatedTrack = mediaTrackRepository.GetById(updatedTrackId.Value);

        Assert.NotNull(updatedTrack);
        Assert.Equal("Bohemian Rhapsody - Remastered", updatedTrack.Title);
        Assert.Equal("Bohemian Rhapsody", scanResult.Metadata!.Title);
        Assert.True(updatedTrack.IsKaraoke);

        Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(updatedTrackId.Value));
    }

    [Fact]
    public void PersistPair_WhenCdgPersistenceFails_RollsBackTrack()
    {
        using MediaLibraryDatabaseTestContext context = new();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        string mp3Path = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.cdg");

        MediaTrackRepository mediaTrackRepository = new(context.ConnectionString);

        ScannedTrackPersistenceService scannedTrackPersistenceService = new(mediaTrackRepository, context.ConnectionString);

        FailingKaraokeFileRepository failingKaraokeFileRepository = new();

        KaraokeFilePairingService karaokeFilePairingService = new();

        MediaScanResult scanResult = new()
        {
            FilePath = mp3Path,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = mp3Path,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        KaraokeLibraryPersistenceService karaokeLibraryPersistenceService = new(
            scannedTrackPersistenceService,
            failingKaraokeFileRepository,
            karaokeFilePairingService,
            context.ConnectionString);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            karaokeLibraryPersistenceService.PersistPair(
                scanResult,
                cdgPath,
                sourceId));

        Assert.Equal("Simulated CDG persistence failure.", exception.Message);

        Assert.Null(mediaTrackRepository.GetByFilePath(mp3Path));
    }

    [Fact]
    public void PersistPair_WhenFingerprintProvided_PersistsFingerprint()
    {
        using MediaLibraryDatabaseTestContext context = new();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        string mp3Path = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen - Bohemian Rhapsody.cdg");

        long fileSize = 1024;
        DateTime lastModifiedUtc = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

        MediaTrackRepository mediaTrackRepository = new(context.ConnectionString);

        ScannedTrackPersistenceService scannedTrackPersistenceService = new(mediaTrackRepository, context.ConnectionString);

        KaraokeFileRepository karaokeFileRepository = new(context.ConnectionString);

        KaraokeFilePairingService karaokeFilePairingService = new();

        KaraokeLibraryPersistenceService karaokeLibraryPersistenceService = new(
            scannedTrackPersistenceService,
            karaokeFileRepository,
            karaokeFilePairingService,
            context.ConnectionString);

        MediaScanResult scanResult = new()
        {
            FilePath = mp3Path,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = mp3Path,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        };

        long? trackId = karaokeLibraryPersistenceService.PersistPair(
            scanResult,
            cdgPath,
            sourceId,
            fileSize: fileSize,
            lastModifiedUtc: lastModifiedUtc);

        Assert.NotNull(trackId);

        MediaTrackRecord? track = mediaTrackRepository.GetById(trackId.Value);

        Assert.NotNull(track);
        Assert.Equal(mp3Path, track.FilePath);
        Assert.True(track.IsKaraoke);

        Assert.Equal(fileSize, track.FileSize);
        Assert.Equal(lastModifiedUtc, track.LastModifiedUtc);

        Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(trackId.Value));
    }
}
