namespace KaraokeCodingChallenges.Favourites;

public class FavouriteManager
{

    private readonly HashSet<string> _favourites = [];

    public FavouriteManager()
    {
    }

    public FavouriteManager(IEnumerable<string> favourites)
    {
        ArgumentNullException.ThrowIfNull(favourites);

        _favourites = new HashSet<string>();

        foreach (string favourite in favourites)
        {
            AddFavourite(favourite);
        }
    }

    public bool AddFavourite(string trackPath)
    {
        if (string.IsNullOrWhiteSpace(trackPath))
        {
            throw new ArgumentException("Track path cannot be empty.", nameof(trackPath));
        }

        return _favourites.Add(trackPath);
    }

    public bool RemoveFavourite(string trackPath)
    {
        if (string.IsNullOrWhiteSpace(trackPath))
        {
            throw new ArgumentException("Track path cannot be empty.", nameof(trackPath));
        }

        return _favourites.Remove(trackPath);
    }

    public bool IsFavourite(string trackPath)
    {
        if (string.IsNullOrWhiteSpace(trackPath))
        {
            throw new ArgumentException("Track path cannot be empty.", nameof(trackPath));
        }

        return _favourites.Contains(trackPath);
    }

    public IReadOnlyCollection<string> GetFavourites()
    {
        return _favourites.ToList().AsReadOnly();
    }
}
