using KaraokeCodingChallenges.IncrementalScanning.Models;

namespace KaraokeCodingChallenges.IncrementalScanning;

public class FileFingerprintProvider
{
    public FileFingerprint GetFingerprint(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }

        FileInfo fileInfo = new FileInfo(filePath);
        return new FileFingerprint
        {
            FilePath = fileInfo.FullName,
            FileSize = fileInfo.Length,
            LastModifiedUtc = fileInfo.LastWriteTimeUtc
        };
    }
}
