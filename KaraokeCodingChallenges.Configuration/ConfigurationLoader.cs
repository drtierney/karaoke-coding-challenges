using System.Text.Json;
using System.Text.Json.Serialization;

namespace KaraokeCodingChallenges.Configuration
{
    public static class ConfigurationLoader
    {
        public static AppConfiguration Load(string filePath)
        {
            string json = File.ReadAllText(filePath);

            JsonSerializerOptions options = new();

            options.Converters.Add(new JsonStringEnumConverter());

            AppConfiguration? config = JsonSerializer.Deserialize<AppConfiguration>(json, options);

            if (config == null)
            {
                throw new InvalidOperationException("Failed to load configuration.");
            }

            return config;
        }
    }
}