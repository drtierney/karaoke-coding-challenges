using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaMetadata;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.SearchPlaylistMilestone.Tests;

public class MediaLibraryBuilderTests
{
    [Fact]
    public void Build_WhenScansAreSuccessful_AddsTracksToLibrary()
    {
        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Queen\Bohemian Rhapsody.mp3",
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                }
            },
            new MediaScanResult
            {
                FilePath = @"D:\Music\Journey\Don't Stop Believin'.mp3",
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = @"D:\Music\Journey\Don't Stop Believin'.mp3",
                    Title = "Don't Stop Believin'",
                    Artist = "Journey",
                    DurationSeconds = 251
                }
            }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(
            scanResults,
            []
        );

        Assert.Equal(2, library.Count);
    }

    [Fact]
    public void Build_WhenScanPathIsKaraoke_SetsTrackAsKaraoke()
    {
        string karaokeFilePath = @"D:\Karaoke\Queen\Bohemian Rhapsody.mp3";

        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = karaokeFilePath,
                IsSuccess = true,
                Metadata = new MediaMetadataRecord
                {
                    FilePath = karaokeFilePath,
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    DurationSeconds = 354
                }
            }
        ];

        List<string> karaokeFilePaths =
        [
            karaokeFilePath
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(
            scanResults,
            karaokeFilePaths
        );

        Assert.Single(library.Tracks);
        Assert.True(library.Tracks[0].IsKaraoke);
    }

    [Fact]
    public void Build_WhenScanIsUnsuccessful_DoesNotAddTrackToLibrary()
    {
        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
            {
                FilePath = @"D:\Music\Invalid.mp3",
                IsSuccess = false
            }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(
            scanResults,
            []
        );

        Assert.Empty(library.Tracks);
        Assert.Equal(0, library.Count);
    }

    [Fact]
    public void Build_WhenKaraokePathHasDifferentCase_SetsTrackAsKaraoke()
    {
        string scanFilePath = @"D:\Karaoke\Queen\Bohemian Rhapsody.mp3";
        string karaokeFilePath = @"D:\KARAOKE\QUEEN\BOHEMIAN RHAPSODY.MP3";

        List<MediaScanResult> scanResults =
        [
            new MediaScanResult
        {
            FilePath = scanFilePath,
            IsSuccess = true,
            Metadata = new MediaMetadataRecord
            {
                FilePath = scanFilePath,
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        }
        ];

        List<string> karaokeFilePaths =
        [
            karaokeFilePath
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(
            scanResults,
            karaokeFilePaths
        );

        Assert.Single(library.Tracks);
        Assert.True(library.Tracks[0].IsKaraoke);
    }
}