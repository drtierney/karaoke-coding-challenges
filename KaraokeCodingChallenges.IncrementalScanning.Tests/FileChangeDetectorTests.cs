using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class FileChangeDetectorTests
{
    [Fact]
    public void DetectChange_ReturnsNew_WhenPreviousFingerprintIsNull()
    {
        FileChangeDetector fileChangeDetector = new FileChangeDetector();

        FileFingerprint currentFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = DateTime.UtcNow
        };

        FileChangeStatus result = fileChangeDetector.DetectChange(currentFileFingerprint, null);

        Assert.Equal(FileChangeStatus.New, result);
    }

    [Fact]
    public void DetectChange_ReturnsModified_WhenFileSizeDiffers()
    {
        FileChangeDetector fileChangeDetector = new FileChangeDetector();

        DateTime now = DateTime.UtcNow;

        FileFingerprint currentFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 2048,
            LastModifiedUtc = now
        };

        FileFingerprint previousFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = now
        };

        FileChangeStatus result = fileChangeDetector.DetectChange(currentFileFingerprint, previousFileFingerprint);

        Assert.Equal(FileChangeStatus.Modified, result);
    }
    
    [Fact]
    public void DetectChange_ReturnsModified_WhenLastModifiedUtcDiffers()
    {
        FileChangeDetector fileChangeDetector = new FileChangeDetector();

        DateTime now = DateTime.UtcNow;
        DateTime previousDay = now.AddDays(-1);

        FileFingerprint currentFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = DateTime.UtcNow
        };

        FileFingerprint previousFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = DateTime.UtcNow.AddDays(-1)
        };

        FileChangeStatus result = fileChangeDetector.DetectChange(currentFileFingerprint, previousFileFingerprint);

        Assert.Equal(FileChangeStatus.Modified, result);
    }

    [Fact]
    public void DetectChange_ReturnsUnchanged_WhenFingerprintsMatch()
    {
        FileChangeDetector fileChangeDetector = new FileChangeDetector();

        DateTime now = DateTime.UtcNow;

        FileFingerprint currentFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = now
        };

        FileFingerprint previousFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = now
        };

        FileChangeStatus result = fileChangeDetector.DetectChange(currentFileFingerprint, previousFileFingerprint);

        Assert.Equal(FileChangeStatus.Unchanged, result);
    }

    [Fact]
    public void DetectChange_ReturnsModified_WhenFileSizeAndLastModifiedUtcDiffer()
    {
        FileChangeDetector fileChangeDetector = new FileChangeDetector();

        DateTime now = DateTime.UtcNow;
        DateTime previousDay = now.AddDays(-1);

        FileFingerprint currentFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 2048,
            LastModifiedUtc = now
        };

        FileFingerprint previousFileFingerprint = new FileFingerprint
        {
            FilePath = "test.mp3",
            FileSize = 1024,
            LastModifiedUtc = previousDay
        };

        FileChangeStatus result = fileChangeDetector.DetectChange(currentFileFingerprint, previousFileFingerprint);

        Assert.Equal(FileChangeStatus.Modified, result);
    }
}
