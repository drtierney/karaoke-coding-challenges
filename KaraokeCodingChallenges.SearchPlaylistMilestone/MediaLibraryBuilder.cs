using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.SearchPlaylistMilestone;

public class MediaLibraryBuilder
{
    private readonly MediaTrackMapper _mapper;

    public MediaLibraryBuilder(MediaTrackMapper mapper)
    {
        _mapper = mapper;
    }

    public MediaLibraryCollection Build(
        IEnumerable<MediaScanResult> scanResults,
        IEnumerable<string> karaokeFilePaths)
    {
        MediaLibraryCollection library = new();

        foreach (MediaScanResult scanResult in scanResults)
        {
            bool isKaraoke = karaokeFilePaths.Contains(
                scanResult.FilePath,
                StringComparer.OrdinalIgnoreCase
            );

            MediaTrackModel? track =
                _mapper.Map(scanResult, isKaraoke);

            if (track is not null)
            {
                library.AddTrack(track);
            }
        }

        return library;
    }
}
