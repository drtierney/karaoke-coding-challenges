namespace KaraokeCodingChallenges.MediaTypeDetection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testDirectory = @"D:\KaraokeTest";
            string[] files = Directory.GetFiles(testDirectory);

            Console.WriteLine("Files:");

            MediaTypeDetector mediaTypeDetector = new MediaTypeDetector();
            foreach (string file in files)
            {
                Console.Write(Path.GetFileName(file) + " -> ");
                Console.WriteLine(mediaTypeDetector.GetMediaType(file));
            }

        }
    }
}
