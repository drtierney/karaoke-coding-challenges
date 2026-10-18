using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public class FileMoveDetector
{

    public IReadOnlyList<FileMoveMatch> DetectMoves(
        IReadOnlyList<MediaTrackRecord> missingTracks,
        IReadOnlyList<FileFingerprint> newFiles)
    {
        List<FileMoveMatch> matches = [];

        foreach (MediaTrackRecord track in missingTracks)
        {
            if (track.FileSize is null || track.LastModifiedUtc is null)
            {
                continue;
            }

            foreach (FileFingerprint file in newFiles)
            {
                if (!string.Equals(
                    track.FilePath,
                    file.FilePath,
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (track.FileSize == file.FileSize && track.LastModifiedUtc == file.LastModifiedUtc)
                    {
                        matches.Add(new FileMoveMatch
                        {
                            TrackId = track.Id,
                            OldFilePath = track.FilePath,
                            NewFilePath = file.FilePath
                        });
                    }
                }
            }
        }

        List<FileMoveMatch> unambiguousMatches = [];

        foreach (FileMoveMatch match in matches)
        {
            int trackMatches = matches.Count(candidate => candidate.TrackId == match.TrackId);

            int fileMatches = matches.Count(candidate =>
                string.Equals(
                    candidate.NewFilePath,
                    match.NewFilePath,
                    StringComparison.OrdinalIgnoreCase));

            if (trackMatches == 1 && fileMatches == 1)
            {
                unambiguousMatches.Add(match);
            }
        }

        return unambiguousMatches;
    }
}
