using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MediaLibrarySearch.Models;

namespace KaraokeCodingChallenges.MediaLibrarySearch.Tests;

public class MediaTrackSearchCriteriaTests
{
    [Fact]
    public void Constructor_InitializesPropertiesToNull()
    {
        var criteria = new MediaTrackSearchCriteria();

        Assert.Null(criteria.Title);
        Assert.Null(criteria.Artist);
    }

    [Fact]
    public void Constructor_DefaultsMatchModeToAll()
    {
        var criteria = new MediaTrackSearchCriteria();

        Assert.Equal(MediaTrackSearchMatchMode.All, criteria.MatchMode);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var criteria = new MediaTrackSearchCriteria
        {
            Title = "Song Title",
            Artist = "Artist Name",
            MatchMode = MediaTrackSearchMatchMode.Any
        };

        Assert.Equal("Song Title", criteria.Title);
        Assert.Equal("Artist Name", criteria.Artist);
        Assert.Equal(MediaTrackSearchMatchMode.Any, criteria.MatchMode);
    }
}
