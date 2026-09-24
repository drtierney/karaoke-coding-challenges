namespace KaraokeCodingChallenges.Favourites.Demo;

internal class Program
{
    static void Main()
    {
        string filePath = "favourites.json";

        FavouriteStore store = new();

        IReadOnlyCollection<string> savedFavourites = store.Load(filePath);

        FavouriteManager manager = new(savedFavourites);

        Console.WriteLine("Loaded Favourites");
        Console.WriteLine();

        foreach (string favourite in manager.GetFavourites())
        {
            Console.WriteLine(favourite);
        }

        Console.WriteLine();

        string queenTrack = "Queen - Bohemian Rhapsody.mp3";
        string journeyTrack = "Journey - Don't Stop Believin'.mp3";

        bool queenAdded = manager.AddFavourite(queenTrack);
        bool journeyAdded = manager.AddFavourite(journeyTrack);

        Console.WriteLine($"Added Queen: {queenAdded}");
        Console.WriteLine($"Added Journey: {journeyAdded}");

        Console.WriteLine();

        bool queenRemoved = manager.RemoveFavourite(queenTrack);

        Console.WriteLine($"Removed Queen: {queenRemoved}");

        Console.WriteLine();

        store.Save(filePath, manager.GetFavourites());

        Console.WriteLine("Current Favourites");
        Console.WriteLine();

        foreach (string favourite in manager.GetFavourites())
        {
            Console.WriteLine(favourite);
        }
    }
}
