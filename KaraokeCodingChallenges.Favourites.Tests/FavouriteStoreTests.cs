using System.Text.Json;

namespace KaraokeCodingChallenges.Favourites.Tests;

public class FavouriteStoreTests
{
    [Fact]
    public void Save_WhenFavouritesExist_CreatesFile()
    {
        FavouriteStore store = new();

        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        IReadOnlyCollection<string> favourites =
        [
            "Queen - Bohemian Rhapsody.mp3",
            "The Beatles - Hey Jude.mp3"
        ];

        try
        {
            Assert.False(File.Exists(filePath));

            store.Save(filePath, favourites);

            Assert.True(File.Exists(filePath));
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void Save_WhenFavouritesIsNull_ThrowsArgumentNullException()
    {
        FavouriteStore store = new();

        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        Assert.Throws<ArgumentNullException>(() => store.Save(filePath, null!));
    }

    [Fact]
    public void Load_WhenFileExists_ReturnsSavedFavourites()
    {
        string filePath = Path.GetTempFileName();

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

            Assert.Equal(2, loaded.Count);
            Assert.Contains("Queen - Bohemian Rhapsody.mp3", loaded);
            Assert.Contains("Journey - Don't Stop Believin'.mp3", loaded);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsEmptyCollection()
    {
        FavouriteStore store = new();

        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        IReadOnlyCollection<string> favourites = store.Load(filePath);

        Assert.Empty(favourites);
    }

    [Fact]
    public void Save_WhenFilePathIsEmpty_ThrowsArgumentException()
    {
        FavouriteStore store = new();

        IReadOnlyCollection<string> favourites = [];

        Assert.Throws<ArgumentException>(() => store.Save(string.Empty, favourites));
    }

    [Fact]
    public void Load_WhenFilePathIsEmpty_ThrowsArgumentException()
    {
        FavouriteStore store = new();

        Assert.Throws<ArgumentException>(() => store.Load(string.Empty));
    }

    [Fact]
    public void Load_WhenFileContainsInvalidJson_ThrowsJsonException()
    {
        FavouriteStore store = new();

        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            File.WriteAllText(filePath, "{ invalid json }");

            Assert.Throws<JsonException>(() => store.Load(filePath));
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
