namespace KaraokeCodingChallenges.KaraokeFilePairing;

public class KaraokeFilePairingService
{
    public List<KaraokeFilePair> MatchedPairs { get; } = new();
    public List<string> UnmatchedMp3Files { get; } = new();
    public List<string> UnmatchedCdgFiles { get; } = new();

    public void PairFiles(IEnumerable<string> filePaths)
    {
        HashSet<string> supportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3",
            ".cdg"
        };

        Dictionary<string, List<string>> filesByName =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (string file in filePaths)
        {
            string extension = Path.GetExtension(file);

            if (!supportedExtensions.Contains(extension))
            {
                continue;
            }

            string directory = Path.GetDirectoryName(file) ?? string.Empty;
            string fileName = Path.GetFileNameWithoutExtension(file);
            string fileKey = Path.Combine(directory, fileName);

            if (!filesByName.ContainsKey(fileKey))
            {
                filesByName[fileKey] = new List<string>();
            }

            filesByName[fileKey].Add(file);
        }

        foreach (KeyValuePair<string, List<string>> kvp in filesByName)
        {
            string? mp3File = null;
            string? cdgFile = null;

            foreach (string file in kvp.Value)
            {
                string extension = Path.GetExtension(file);

                if (extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    mp3File = file;
                }
                else if (extension.Equals(".cdg", StringComparison.OrdinalIgnoreCase))
                {
                    cdgFile = file;
                }
            }

            if (mp3File != null && cdgFile != null)
            {
                MatchedPairs.Add(new KaraokeFilePair(mp3File, cdgFile));
            }
            else if (mp3File != null)
            {
                UnmatchedMp3Files.Add(mp3File);
            }
            else if (cdgFile != null)
            {
                UnmatchedCdgFiles.Add(cdgFile);
            }
        }
    }
}