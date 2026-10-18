using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class LibraryReconciliationServiceTests
{
    [Fact]
    public void Reconcile_ReturnsMissingTracks_WhenStoredTracksAreNotScanned()
    {
        int sourceId = 1;

        List<MediaTrackRecord> storedTracks =
        [
            new MediaTrackRecord
            {
                Id = 1,
                FilePath = Path.Combine("Library", "Karaoke", "Queen.mp3"),
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            },
            new MediaTrackRecord
            {
                Id = 2,
                FilePath = Path.Combine("Library", "Karaoke", "Muse.mp3"),
                Artist = "Muse",
                Title = "Feeling Good"
            },
            new MediaTrackRecord
            {
                Id = 3,
                FilePath = Path.Combine("Library", "Karaoke", "Paramore.mp3"),
                Artist = "Paramore",
                Title = "Ain't it Fun"
            }
        ];

        List<string> scannedPaths = new()
        {
            Path.Combine("Library", "Karaoke", "Queen.mp3"),
            Path.Combine("Library", "Karaoke", "Paramore.mp3")
        };

        LibraryReconciliationService service = new();

        bool scanCompletedSuccessfully = true;

        LibraryReconciliationResult result = service.Reconcile(
            sourceId,
            storedTracks,
            scannedPaths,
            scanCompletedSuccessfully);

        Assert.Equal(3, result.StoredTrackCount);
        Assert.Equal(2, result.ScannedTrackCount);

        Assert.Equal(1, result.MissingTrackCount);
        Assert.Equal(2L, Assert.Single(result.MissingTracks).Id);

        Assert.Equal(2, result.PresentTracks.Count);
        Assert.Equal([1L, 3L], result.PresentTracks.Select(track => track.Id).ToArray());

        Assert.True(result.ScanCompletedSuccessfully);
    }

    [Fact]
    public void Reconcile_ReturnsNoMissingTracks_WhenScanFails()
    {
        int sourceId = 1;

        List<MediaTrackRecord> storedTracks =
        [
            new MediaTrackRecord
            {
                Id = 1,
                FilePath = Path.Combine("Library", "Karaoke", "Queen.mp3"),
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            }
        ];

        List<string> scannedPaths = [];

        LibraryReconciliationService service = new();

        bool scanCompletedSuccessfully = false;

        LibraryReconciliationResult result = service.Reconcile(
            sourceId,
            storedTracks,
            scannedPaths,
            scanCompletedSuccessfully);

        Assert.Equal(0, result.MissingTrackCount);
        Assert.Empty(result.MissingTracks);
        Assert.Empty(result.PresentTracks);
        Assert.False(result.ScanCompletedSuccessfully);
    }

    [Fact]
    public void Reconcile_MatchesFilePaths_CaseInsensitively()
    {
        int sourceId = 1;

        List<MediaTrackRecord> storedTracks =
        [
            new MediaTrackRecord
            {
                Id = 1,
                FilePath = Path.Combine("LIBRARY", "KARAOKE", "QUEEN.MP3"),
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            },
            new MediaTrackRecord
            {
                Id = 2,
                FilePath = Path.Combine("library", "karaoke", "muse.mp3"),
                Artist = "Muse",
                Title = "Feeling Good"
            }
        ];

        List<string> scannedPaths = new()
        {
            Path.Combine("Library", "Karaoke", "Queen.mp3"),
            Path.Combine("Library", "Karaoke", "muse.mp3")
        };

        LibraryReconciliationService service = new();

        bool scanCompletedSuccessfully = true;

        LibraryReconciliationResult result = service.Reconcile(
            sourceId,
            storedTracks,
            scannedPaths,
            scanCompletedSuccessfully);

        Assert.Equal(2, result.StoredTrackCount);
        Assert.Equal(2, result.ScannedTrackCount);
        Assert.Empty(result.MissingTracks);
        Assert.Equal(0, result.MissingTrackCount);
        Assert.True(result.ScanCompletedSuccessfully);
    }

    [Fact]
    public void Reconcile_ReturnsAllStoredTracks_WhenSuccessfulScanIsEmpty()
    {
        int sourceId = 1;

        List<MediaTrackRecord> storedTracks =
        [
            new MediaTrackRecord
            {
                Id = 1,
                FilePath = Path.Combine("Library", "Karaoke", "Queen.mp3"),
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            },
            new MediaTrackRecord
            {
                Id = 3,
                FilePath = Path.Combine("Library", "Karaoke", "Paramore.mp3"),
                Artist = "Paramore",
                Title = "Ain't it Fun"
            }
        ];

        List<string> scannedPaths = [];

        LibraryReconciliationService service = new();

        bool scanCompletedSuccessfully = true;

        LibraryReconciliationResult result = service.Reconcile(
            sourceId,
            storedTracks,
            scannedPaths,
            scanCompletedSuccessfully);

        Assert.Equal(2, result.StoredTrackCount);
        Assert.Equal(0, result.ScannedTrackCount);
        Assert.Equal([1, 3], result.MissingTracks.Select(track => track.Id).ToArray());
        Assert.True(result.ScanCompletedSuccessfully);
    }

    [Fact]
    public void Reconcile_IdentifiesMovedTracks_WithoutMarkingThemMissing()
    {
        DateTime lastModified = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

        MediaTrackRecord storedTrack = new()
        {
            Id = 42,
            SourceId = 1,
            Title = "Bohemian Rhapsody",
            Artist = "Queen",
            FilePath = Path.Combine("Karaoke", "Queen.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        FileFingerprint newFile = new()
        {
            FilePath = Path.Combine("Karaoke", "Classics", "Queen.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        LibraryReconciliationService service = new();

        LibraryReconciliationResult result = service.Reconcile(
            sourceId: 1,
            storedTracks: [storedTrack],
            scannedPaths: [newFile.FilePath],
            scanCompletedSuccessfully: true,
            newFiles: [newFile]);

        Assert.Equal(1, result.StoredTrackCount);
        Assert.Equal(1, result.ScannedTrackCount);

        Assert.Empty(result.MissingTracks);
        Assert.Empty(result.PresentTracks);

        FileMoveMatch move = Assert.Single(result.MovedTracks);

        Assert.Equal(42L, move.TrackId);
        Assert.Equal(storedTrack.FilePath, move.OldFilePath);
        Assert.Equal(newFile.FilePath, move.NewFilePath);
    }
}
