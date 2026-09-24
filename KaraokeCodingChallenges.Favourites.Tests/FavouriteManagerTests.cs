namespace KaraokeCodingChallenges.Favourites.Tests;

public class FavouriteManagerTests
{
    [Fact]
    public void FavouriteManager_WhenCreated_HasNoFavourites()
    {
        FavouriteManager manager = new();

        IReadOnlyCollection<string> favourites = manager.GetFavourites();

        Assert.Empty(favourites);
    }

    [Fact]
    public void FavouriteManager_WhenFavouritesIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FavouriteManager(null!));
    }

    [Fact]
    public void AddFavourite_WhenTrackIsAdded_ReturnsTrue()
    {
        FavouriteManager manager = new();

        bool added = manager.AddFavourite("Queen - Bohemian Rhapsody.mp3");

        Assert.True(added);
    }

    [Fact]
    public void AddFavourite_WhenTrackAlreadyExists_ReturnsFalse()
    {
        FavouriteManager manager = new();

        manager.AddFavourite("Queen - Bohemian Rhapsody.mp3");

        bool added = manager.AddFavourite("Queen - Bohemian Rhapsody.mp3");

        Assert.False(added);
    }

    [Fact]
    public void IsFavourite_WhenTrackExists_ReturnsTrue()
    {
        FavouriteManager manager = new();

        manager.AddFavourite("Queen - Bohemian Rhapsody.mp3");

        bool isFavourite = manager.IsFavourite("Queen - Bohemian Rhapsody.mp3");

        Assert.True(isFavourite);
    }

    [Fact]
    public void RemoveFavourite_WhenTrackExists_ReturnsTrue()
    {
        FavouriteManager manager = new();

        manager.AddFavourite("Queen - Bohemian Rhapsody.mp3");

        bool removed = manager.RemoveFavourite("Queen - Bohemian Rhapsody.mp3");

        Assert.True(removed);
    }

    [Fact]
    public void RemoveFavourite_WhenTrackDoesNotExist_ReturnsFalse()
    {
        FavouriteManager manager = new();

        bool removed = manager.RemoveFavourite("Queen - Bohemian Rhapsody.mp3");

        Assert.False(removed);
    }

    [Fact]
    public void IsFavourite_WhenTrackDoesNotExist_ReturnsFalse()
    {
        FavouriteManager manager = new();

        bool isFavourite = manager.IsFavourite("Queen - Bohemian Rhapsody.mp3");

        Assert.False(isFavourite);
    }

    [Fact]
    public void AddFavourite_WhenTrackPathIsEmpty_ThrowsArgumentException()
    {
        FavouriteManager manager = new();

        Assert.Throws<ArgumentException>(() => manager.AddFavourite(string.Empty));
    }

    [Fact]
    public void RemoveFavourite_WhenTrackPathIsEmpty_ThrowsArgumentException()
    {
        FavouriteManager manager = new();

        Assert.Throws<ArgumentException>(() => manager.RemoveFavourite(string.Empty));
    }

    [Fact]
    public void IsFavourite_WhenTrackPathIsEmpty_ThrowsArgumentException()
    {
        FavouriteManager manager = new();

        Assert.Throws<ArgumentException>(() => manager.IsFavourite(string.Empty));
    }

    [Fact]
    public void FavouriteManager_WhenCreatedWithFavourites_LoadsFavourites()
    {
        FavouriteManager manager = new(
        [
            "Queen - Bohemian Rhapsody.mp3",
            "Journey - Don't Stop Believin'.mp3"
        ]);

        IReadOnlyCollection<string> favourites = manager.GetFavourites();

        Assert.Equal(2, favourites.Count);
    }

    [Fact]
    public void FavouriteManager_WhenCreatedWithDuplicateFavourites_RemovesDuplicates()
    {
        FavouriteManager manager = new(
        [
            "Queen - Bohemian Rhapsody.mp3",
            "Queen - Bohemian Rhapsody.mp3"
        ]);

        IReadOnlyCollection<string> favourites = manager.GetFavourites();

        Assert.Single(favourites);
    }

    [Fact]
    public void FavouriteManager_WhenLoadedFromStore_RestoresFavourites()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            FavouriteStore store = new();

            IReadOnlyCollection<string> favourites =
            [
                "Queen - Bohemian Rhapsody.mp3",
                "Journey - Don't Stop Believin'.mp3"
            ];

            store.Save(filePath, favourites);

            IReadOnlyCollection<string> loaded = store.Load(filePath);

            FavouriteManager manager = new(loaded);

            Assert.True(manager.IsFavourite("Queen - Bohemian Rhapsody.mp3"));
            Assert.True(manager.IsFavourite("Journey - Don't Stop Believin'.mp3"));
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}