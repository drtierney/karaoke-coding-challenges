using System.Text.Json;

namespace KaraokeCodingChallenges.Configuration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AppConfiguration config = ConfigurationLoader.Load("appsettings.json");

                IReadOnlyCollection<string> errors = ConfigurationValidator.Validate(config);

                if (errors.Count > 0)
                {
                    Console.WriteLine("Configuration errors:");

                    foreach (string error in errors)
                    {
                        Console.WriteLine($"- {error}");
                    }

                    return;
                }

                foreach (LibrarySource source in config.LibrarySources)
                {
                    Console.WriteLine($"Path: {source.Path}");
                    Console.WriteLine($"Type: {source.Type}");
                    Console.WriteLine($"Enabled: {source.Enabled}");
                    Console.WriteLine();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Configuration file not found.");
            }
            catch (JsonException)
            {
                Console.WriteLine("Configuration file contains invalid JSON.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}