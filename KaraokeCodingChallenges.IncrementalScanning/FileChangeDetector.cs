using KaraokeCodingChallenges.IncrementalScanning.Enums;
using KaraokeCodingChallenges.IncrementalScanning.Models;

namespace KaraokeCodingChallenges.IncrementalScanning;

public class FileChangeDetector
{
    public FileChangeStatus DetectChange(
        FileFingerprint current,
        FileFingerprint? previous)
    {
        ArgumentNullException.ThrowIfNull(current);

        if (previous == null)
        {
            return FileChangeStatus.New;
        }

        if (current.FileSize != previous.FileSize || 
            current.LastModifiedUtc != previous.LastModifiedUtc)
        {
            return FileChangeStatus.Modified;
        }

        return FileChangeStatus.Unchanged;
    }
}
