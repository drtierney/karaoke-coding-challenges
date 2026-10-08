using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Mapping;

public static class ScannedTrackMapper
{
    public static MediaTrackRecord? Map(MediaScanResult scanResult, long sourceId, bool isKaraoke = false)
    {
        if (!scanResult.IsSuccess || scanResult.Metadata is null)
        {
            return null;
        }

        string fileName = scanResult.FilePath.Split('\\', '/').Last();

        string title = string.IsNullOrWhiteSpace(scanResult.Metadata.Title)
            ? Path.GetFileNameWithoutExtension(fileName)
            : scanResult.Metadata.Title;

        return new MediaTrackRecord
        {
            SourceId = sourceId,
            Title = title,
            Artist = scanResult.Metadata.Artist ?? string.Empty,
            FilePath = scanResult.FilePath,
            DurationSeconds = scanResult.Metadata.DurationSeconds ?? 0,
            IsKaraoke = isKaraoke
        };
    }
}
