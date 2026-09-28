using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.SearchPlaylistMilestone;

public class MediaTrackMapper
{
    public MediaTrackModel? Map(MediaScanResult scanResult, bool isKaraoke = false)
    {
        if (!scanResult.IsSuccess || scanResult.Metadata is null)
        {
            return null;
        }

        string title = string.IsNullOrWhiteSpace(scanResult.Metadata.Title)
            ? Path.GetFileNameWithoutExtension(scanResult.FilePath)
            : scanResult.Metadata.Title;

        return new MediaTrackModel(
            title,
            scanResult.Metadata.Artist ?? string.Empty,
            scanResult.FilePath,
            scanResult.Metadata.DurationSeconds ?? 0,
            isKaraoke
        );
    }
}
