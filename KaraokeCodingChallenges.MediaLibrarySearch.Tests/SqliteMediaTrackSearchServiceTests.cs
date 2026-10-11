
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibrarySearch.Models;

namespace KaraokeCodingChallenges.MediaLibrarySearch.Tests;

public class SqliteMediaTrackSearchServiceTests
{
    [Fact]
    public void Search_WhenNoCriteriaProvided_ReturnsAllTracks()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);
        context.AddTrack(sourceId, "Don't Stop Believin'", "Journey", 251);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria();

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Equal(3, results.Count);
        Assert.Contains(results, track => track.Title == "Bohemian Rhapsody" && track.Artist == "Queen");
        Assert.Contains(results, track => track.Title == "Misery Business" && track.Artist == "Paramore");
        Assert.Contains(results, track => track.Title == "Don't Stop Believin'" && track.Artist == "Journey");
    }

    [Fact]
    public void Search_WhenTitleProvided_ReturnsMatchingTracks()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Title = "Bohemian"
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Single(results);
        Assert.Equal("Bohemian Rhapsody", results[0].Title);
        Assert.Equal("Queen", results[0].Artist);
    }

    [Theory]
    [InlineData("bohemian")]
    [InlineData("BOHEMIAN")]
    [InlineData("BoHeMiAn")]
    public void Search_WhenTitleHasDifferentCasing_ReturnsMatchingTracks(string searchTitle)
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Title = searchTitle
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Single(results);
        Assert.Equal("Bohemian Rhapsody", results[0].Title);
        Assert.Equal("Queen", results[0].Artist);
    }

    [Fact]
    public void Search_WhenTitleDoesNotMatch_ReturnsEmptyCollection()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Title = "Unknown Song"
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Search_WhenTitleIsBlank_ReturnsAllTracks(string? searchTitle)
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Title = searchTitle
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, track => track.Title == "Bohemian Rhapsody" && track.Artist == "Queen");
        Assert.Contains(results, track => track.Title == "Misery Business" && track.Artist == "Paramore");
    }

    [Fact]
    public void Search_WhenArtistProvided_ReturnsMatchingTracks()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);
        context.AddTrack(sourceId, "Don't Stop Me Now", "Queen", 284);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Artist = "Queen"
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Equal(2, results.Count);
        Assert.All(results, track => Assert.Equal("Queen", track.Artist));
    }

    [Theory]
    [InlineData("queen")]
    [InlineData("QUEEN")]
    public void Search_WhenArtistHasDifferentCasing_ReturnsMatchingTracks(string searchArtist)
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);
        context.AddTrack(sourceId, "Don't Stop Me Now", "Queen", 284);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Artist = searchArtist
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Equal(2, results.Count);
        Assert.All(results, track => Assert.Equal("Queen", track.Artist));
    }

    [Fact]
    public void Search_WhenMatchModeIsAll_ReturnsTracksMatchingBothCriteria()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);
        context.AddTrack(sourceId, "Don't Stop Me Now", "Queen", 284);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Artist = "Queen",
            Title = "Bohemian",
            MatchMode = MediaTrackSearchMatchMode.All
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Single(results);
        Assert.Equal("Bohemian Rhapsody", results[0].Title);
        Assert.Equal("Queen", results[0].Artist);
    }

    [Fact]
    public void Search_WhenMatchModeIsAny_ReturnsTracksMatchingEitherCriteria()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);
        context.AddTrack(sourceId, "Don't Stop Me Now", "Queen", 284);
        context.AddTrack(sourceId, "Bohemian Like You", "The Dandy Warhols", 211);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Artist = "Queen",
            Title = "Bohemian",
            MatchMode = MediaTrackSearchMatchMode.Any
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        Assert.Equal(3, results.Count);
        Assert.Contains(results, track => track.Title == "Bohemian Rhapsody" && track.Artist == "Queen");
        Assert.Contains(results, track => track.Title == "Don't Stop Me Now" && track.Artist == "Queen");
        Assert.Contains(results, track => track.Title == "Bohemian Like You" && track.Artist == "The Dandy Warhols");
    }

    [Fact]
    public void Search_WhenMatchModeIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            MatchMode = (MediaTrackSearchMatchMode)999
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => searchService.Search(criteria));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenConnectionStringIsInvalid_ThrowsArgumentException(
    string? connectionString)
    {
        Assert.Throws<ArgumentException>(() => new SqliteMediaTrackSearchService(connectionString!));
    }

    [Fact]
    public void Search_WhenCriteriaIsNull_ThrowsArgumentNullException()
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        Assert.Throws<ArgumentNullException>(() => searchService.Search(null!));
    }


    [Theory]
    [InlineData(MediaTrackSearchMatchMode.All, "Bohemian", null)]
    [InlineData(MediaTrackSearchMatchMode.Any, "Bohemian", null)]
    [InlineData(MediaTrackSearchMatchMode.All, null, "Queen")]
    [InlineData(MediaTrackSearchMatchMode.Any, null, "Queen")]
    public void Search_WhenOnlyOneCriterionProvided_ReturnsMatchingTracks(MediaTrackSearchMatchMode matchMode, string? title, string? artist)
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        context.AddTrack(sourceId, "Bohemian Rhapsody", "Queen", 354);
        context.AddTrack(sourceId, "Misery Business", "Paramore", 212);
        context.AddTrack(sourceId, "Don't Stop Me Now", "Queen", 284);

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Title = title,
            Artist = artist,
            MatchMode = matchMode
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        int expectedCount = title is not null ? 1 : 2;

        Assert.Equal(expectedCount, results.Count);
        Assert.All(results, track => Assert.True((title is not null && track.Title.Contains(title)) || (artist is not null && track.Artist.Contains(artist))));
    }

    [Theory]
    [InlineData(true, "100%", "100% Pure Love", "1000 Pure Love")]
    [InlineData(true, "Song_1", "Song_1", "SongA1")]
    [InlineData(true, @"Part\Two", @"Part\Two", "PartTwo")]
    [InlineData(false, "100%", "100% Band", "1000 Band")]
    [InlineData(false, "Band_1", "Band_1", "BandA1")]
    [InlineData(false, @"Band\Two", @"Band\Two", "BandTwo")]
    public void Search_WhenCriterionContainsSpecialCharacters_TreatsCharactersLiterally(bool searchByTitle, string searchValue, string matchingValue, string nonMatchingValue)
    {
        using MediaLibrarySearchTestContext context = new MediaLibrarySearchTestContext();

        long sourceId = context.AddSource();

        if (searchByTitle)
        {
            context.AddTrack(sourceId, matchingValue, "Artist One", 200);
            context.AddTrack(sourceId, nonMatchingValue, "Artist Two", 210);
        }
        else
        {
            context.AddTrack(sourceId, "Track One", matchingValue, 200);
            context.AddTrack(sourceId, "Track Two", nonMatchingValue, 210);
        }

        SqliteMediaTrackSearchService searchService = new SqliteMediaTrackSearchService(context.ConnectionString);

        MediaTrackSearchCriteria criteria = new MediaTrackSearchCriteria
        {
            Title = searchByTitle ? searchValue : null,
            Artist = searchByTitle ? null : searchValue
        };

        IReadOnlyList<MediaTrackRecord> results = searchService.Search(criteria);

        MediaTrackRecord result = Assert.Single(results);

        if (searchByTitle)
        {
            Assert.Equal(matchingValue, result.Title);
        }
        else
        {
            Assert.Equal(matchingValue, result.Artist);
        }
    }

}
