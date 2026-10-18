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
                FilePath = @"D:\Karaoke\Queen.mp3",
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            },
            new MediaTrackRecord
            {
                Id = 2,
                FilePath = @"D:\Karaoke\Muse.mp3",
                Artist = "Muse",
                Title = "Feeling Good"
            },
            new MediaTrackRecord
            {
                Id = 3,
                FilePath = @"D:\Karaoke\Paramore.mp3",
                Artist = "Paramore",
                Title = "Ain't it Fun"
            }
        ];

        List<string> scannedPaths = new()
        {
            @"D:\Karaoke\Queen.mp3",
            @"D:\Karaoke\Paramore.mp3"
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
                FilePath = @"D:\Karaoke\Queen.mp3",
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
                FilePath = @"D:\KARAOKE\QUEEN.MP3",
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            },
            new MediaTrackRecord
            {
                Id = 2,
                FilePath = @"d:\karaoke\muse.mp3",
                Artist = "Muse",
                Title = "Feeling Good"
            }
        ];

        List<string> scannedPaths = new()
        {
            @"D:\Karaoke\Queen.mp3",
            @"D:\Karaoke\muse.mp3"
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
                FilePath = @"D:\Karaoke\Queen.mp3",
                Artist = "Queen",
                Title = "Bohemian Rhapsody"
            },
            new MediaTrackRecord
            {
                Id = 3,
                FilePath = @"D:\Karaoke\Paramore.mp3",
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
}
