namespace KaraokeCodingChallenges.LibraryScanner
{
    public class LibraryScannerService
    {
        private static readonly HashSet<string> SupportedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".mp3",
                ".opus",
                ".flac",
                ".cdg"
            };

        public List<string> Scan(string? folderPath)
        {
            List<string> supportedFiles = [];

            if (!Directory.Exists(folderPath))
            {
                return supportedFiles;
            }

            string[] files = Directory.GetFiles(
                folderPath,
                "*",
                SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);

                if (SupportedExtensions.Contains(extension))
                {
                    supportedFiles.Add(file);
                }
            }

            return supportedFiles;
        }

        internal Dictionary<string, int> CountByExtension(List<string> files)
        {
            Dictionary<string, int> extensionCounts =
                new(StringComparer.OrdinalIgnoreCase);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file).ToLowerInvariant();

                extensionCounts.TryGetValue(extension, out int count);
                extensionCounts[extension] = count + 1;
            }

            return extensionCounts;
        }
    }
}