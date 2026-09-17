using System.Text.Json;

namespace KaraokeCodingChallenges.Configuration
{
    public static class ConfigurationLoader
    {
        public static AppConfiguration Load(string filePath)
        {
            string json = File.ReadAllText(filePath);

            AppConfiguration? config = JsonSerializer.Deserialize<AppConfiguration>(json);

            if (config == null)
            {
                throw new InvalidOperationException("Failed to load configuration.");
            }

            if (string.IsNullOrWhiteSpace(config.LibraryPath))
            {
                throw new InvalidOperationException("LibraryPath is missing from configuration.");
            }

            return config;
        }
    }
}