namespace KaraokeCodingChallenges.Configuration
{
    public static class ConfigurationValidator
    {
        public static IReadOnlyCollection<string> Validate(AppConfiguration config)
        {
            List<string> errors = [];

            if (string.IsNullOrWhiteSpace(config.LibraryPath))
            {
                errors.Add("LibraryPath is missing from configuration.");
            }

            return errors;
        }
    }
}