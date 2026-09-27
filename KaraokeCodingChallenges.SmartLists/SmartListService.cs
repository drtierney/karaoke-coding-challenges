using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists;

public class SmartListService
{
    public IReadOnlyList<MediaTrackModel> Apply(SmartList smartList, IEnumerable<MediaTrackModel> tracks)
    {
        return tracks.Where(track => smartList.Rules.All(rule => rule.Matches(track))).ToList();
    }
}