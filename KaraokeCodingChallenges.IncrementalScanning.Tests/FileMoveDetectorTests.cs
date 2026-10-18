
using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class FileMoveDetectorTests
{
    [Fact]
    public void DetectMoves_ReturnsMatch_WhenFingerprintMatchesAtDifferentPath()
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

        FileMoveDetector detector = new();

        IReadOnlyList<FileMoveMatch> matches = detector.DetectMoves(
            [storedTrack],
            [newFile]);

        FileMoveMatch match = Assert.Single(matches);

        Assert.Equal(42L, match.TrackId);
        Assert.Equal(storedTrack.FilePath, match.OldFilePath);
        Assert.Equal(newFile.FilePath, match.NewFilePath);
    }

    [Fact]
    public void DetectMoves_ReturnsNoMatches_WhenMultipleTracksShareFingerprint()
    {
        DateTime lastModified = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

        MediaTrackRecord queen = new()
        {
            Id = 42,
            SourceId = 1,
            Title = "Bohemian Rhapsody",
            Artist = "Queen",
            FilePath = Path.Combine("Karaoke", "Queen.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        MediaTrackRecord muse = new()
        {
            Id = 43,
            SourceId = 1,
            Title = "Feeling Good",
            Artist = "Muse",
            FilePath = Path.Combine("Karaoke", "Muse.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        FileFingerprint newFile = new()
        {
            FilePath = Path.Combine("Karaoke", "Classics", "Unknown.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        FileMoveDetector detector = new();

        IReadOnlyList<FileMoveMatch> matches = detector.DetectMoves(
            [queen, muse],
            [newFile]);

        Assert.Empty(matches);
    }

    [Fact]
    public void DetectMoves_ReturnsNoMatches_WhenOneTrackMatchesMultipleNewFiles()
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

        FileFingerprint newFileA = new()
        {
            FilePath = Path.Combine("Karaoke", "Classics", "Queen.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        FileFingerprint newFileB = new()
        {
            FilePath = Path.Combine("Karaoke", "Rock", "Queen.mp3"),
            FileSize = 1024,
            LastModifiedUtc = lastModified
        };

        FileMoveDetector detector = new();

        IReadOnlyList<FileMoveMatch> matches = detector.DetectMoves(
            [storedTrack],
            [newFileA, newFileB]);

        Assert.Empty(matches);
    }
}
