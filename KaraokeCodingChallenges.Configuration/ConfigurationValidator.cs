namespace KaraokeCodingChallenges.Configuration
{
    public static class ConfigurationValidator
    {
        public static IReadOnlyCollection<string> Validate(AppConfiguration config)
        {
            List<string> errors = [];
            HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase);

            if (config.LibrarySources == null || config.LibrarySources.Count == 0)
            {
                errors.Add("At least one library source must be configured.");
                return errors;
            }
            
            foreach (LibrarySource source in config.LibrarySources)
            {
                if (string.IsNullOrWhiteSpace(source.Path))
                {
                    errors.Add("Library source path cannot be empty.");
                }
                else if (!paths.Add(source.Path))
                {
                    errors.Add($"Duplicate library source path found: {source.Path}");
                }
            }

            return errors;
        }
    }
}