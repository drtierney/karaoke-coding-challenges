using KaraokeCodingChallenges.IncrementalScanning;
using KaraokeCodingChallenges.IncrementalScanning.Models;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class FileFingerprintProviderTests
{

    [Fact]
    public void GetFingerprint_ReturnsFileSizeAndLastModifiedUtc()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"fingerprint-test-{Guid.NewGuid():N}.mp3");

        File.WriteAllText(filePath, "Test");

        try
        {
            FileInfo fileInfo = new FileInfo(filePath);

            FileFingerprintProvider provider = new FileFingerprintProvider();

            FileFingerprint fingerprint = provider.GetFingerprint(filePath);

            Assert.Equal(fileInfo.FullName, fingerprint.FilePath);
            Assert.Equal(fileInfo.Length, fingerprint.FileSize);
            Assert.Equal(fileInfo.LastWriteTimeUtc, fingerprint.LastModifiedUtc);
            Assert.Equal(DateTimeKind.Utc, fingerprint.LastModifiedUtc.Kind);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void GetFingerprint_ThrowsArgumentException_WhenPathIsEmpty()
    {
        FileFingerprintProvider provider = new FileFingerprintProvider();

        Assert.Throws<ArgumentException>(() => provider.GetFingerprint(string.Empty));
    }

    [Fact]
    public void GetFingerprint_ThrowsArgumentException_WhenPathIsWhitespace()
    {
        FileFingerprintProvider provider = new FileFingerprintProvider();

        Assert.Throws<ArgumentException>(() => provider.GetFingerprint("   "));
    }

    [Fact]
    public void GetFingerprint_ThrowsFileNotFoundException_WhenFileDoesNotExist()
    {
        FileFingerprintProvider provider = new FileFingerprintProvider();

        string nonExistentFilePath = Path.Combine(Path.GetTempPath(), $"nonexistent-{Guid.NewGuid():N}.mp3");

        Assert.Throws<FileNotFoundException>(() => provider.GetFingerprint(nonExistentFilePath));
    }

    [Fact]
    public void GetFingerprint_ReturnsZeroFileSize_WhenFileIsEmpty()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"empty-file-test-{Guid.NewGuid():N}.mp3");

        File.WriteAllText(filePath, string.Empty);

        try
        {
            FileFingerprintProvider provider = new FileFingerprintProvider();
            FileFingerprint fingerprint = provider.GetFingerprint(filePath);

            Assert.Equal(0, fingerprint.FileSize);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }


    [Fact]
    public void GetFingerprint_ReturnsAbsoluteFilePath_WhenGivenRelativePath()
    {
        string relativeFilePath = $"relative-file-test-{Guid.NewGuid():N}.mp3";

        string absoluteFilePath = Path.GetFullPath(relativeFilePath);

        File.WriteAllText(relativeFilePath, "Test");

        try
        {
            FileFingerprintProvider provider = new FileFingerprintProvider();

            FileFingerprint fingerprint = provider.GetFingerprint(relativeFilePath);

            Assert.Equal(absoluteFilePath, fingerprint.FilePath);
        }
        finally
        {
            if (File.Exists(absoluteFilePath))
            {
                File.Delete(absoluteFilePath);
            }
        }
    }

}
