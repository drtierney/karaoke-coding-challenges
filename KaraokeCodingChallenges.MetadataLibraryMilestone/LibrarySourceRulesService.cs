using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.KaraokeFilePairing;

namespace KaraokeCodingChallenges.MetadataLibraryMilestone
{
    public static class LibrarySourceRulesService
    {
        public static IReadOnlyCollection<string> GetKaraokeFiles(
            LibrarySourceType sourceType,
            IEnumerable<string> sourceFiles)
        {
            List<string> karaokeFiles = [];

            switch (sourceType)
            {
                case LibrarySourceType.Music:
                    break;

                case LibrarySourceType.Karaoke:
                    karaokeFiles.AddRange(sourceFiles);
                    break;

                case LibrarySourceType.Mixed:
                    KaraokeFilePairingService mixedPairing = new();
                    mixedPairing.PairFiles(sourceFiles);

                    foreach (KaraokeFilePair pair in mixedPairing.MatchedPairs)
                    {
                        karaokeFiles.Add(pair.Mp3FilePath);
                        karaokeFiles.Add(pair.CdgFilePath);
                    }

                    karaokeFiles.AddRange(mixedPairing.UnmatchedCdgFiles);
                    break;
            }

            return karaokeFiles;
        }
    }
}