namespace KaraokeCodingChallenges.Configuration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AppConfiguration config = ConfigurationLoader.Load("appsettings.json");

                Console.WriteLine($"Library Path: {config.LibraryPath}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Configuration file not found.");
            }
            catch (System.Text.Json.JsonException)
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