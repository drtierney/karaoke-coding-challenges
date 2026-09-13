namespace KaraokeCodingChallenges.KaraokeFilePairing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testDirectory = @"D:\KaraokeTest";
            string[] files = Directory.GetFiles(testDirectory);

            Console.WriteLine("Files:");

            foreach (string file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            Console.WriteLine();

            KaraokeFilePairingService filePairing = new KaraokeFilePairingService();
            filePairing.PairFiles(files);

            Console.WriteLine("Matched Karaoke Pairs:");

            foreach (KaraokeFilePair pair in filePairing.MatchedPairs)
            {
                Console.WriteLine(Path.GetFileNameWithoutExtension(pair.Mp3FilePath));
            }

            Console.WriteLine();

            Console.WriteLine("MP3 Without CDG:");

            foreach (string file in filePairing.UnmatchedMp3Files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            Console.WriteLine();

            Console.WriteLine("CDG Without MP3:");

            foreach (string file in filePairing.UnmatchedCdgFiles)
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            Console.WriteLine();

            Console.WriteLine("Summary:");
            Console.WriteLine($"Matched pairs: {filePairing.MatchedPairs.Count}");
            Console.WriteLine($"MP3 without CDG: {filePairing.UnmatchedMp3Files.Count}");
            Console.WriteLine($"CDG without MP3: {filePairing.UnmatchedCdgFiles.Count}");
        }
    }
}
