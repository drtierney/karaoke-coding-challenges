
namespace KaraokeCodingChallenges.MediaLibrarySearch.Models;

public class MediaTrackSearchCriteria
{
    public string? Title { get; set; }
    public string? Artist { get; set; }

    public MediaTrackSearchMatchMode MatchMode { get; set; } = MediaTrackSearchMatchMode.All;
}
