using System.Text.Json;

namespace KaraokeCodingChallenges.Favourites;

public class FavouriteStore
{
    public void Save(string filePath, IReadOnlyCollection<string> favourites)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        ArgumentNullException.ThrowIfNull(favourites);

        string json = JsonSerializer.Serialize(favourites);

        File.WriteAllText(filePath, json);
    }

    public IReadOnlyCollection<string> Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            return [];
        }

        string json = File.ReadAllText(filePath);

        List<string>? favourites =
            JsonSerializer.Deserialize<List<string>>(json);

        return favourites ?? [];
    }
}