namespace KaraokeCodingChallenges.LibraryScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LibraryScanner scanner = new LibraryScanner();
            Console.WriteLine("Enter folder to scan:");
            string? folderPath = Console.ReadLine();

            Console.WriteLine();
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Folder '{folderPath}' does not exist.");
                return;
            }

            Console.WriteLine($"Scanning folder: {folderPath}");

            List<string> files = scanner.Scan(folderPath);

            if (files.Count == 0)
            {
                Console.WriteLine("No supported files found.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Supported files found: {files.Count}");

            Console.WriteLine();
            Console.WriteLine("Files:");

            foreach (string file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            Dictionary<string, int> counts = scanner.CountByExtension(files);

            Console.WriteLine();
            Console.WriteLine("Files by extension:");

            foreach (KeyValuePair<string, int> count in counts)
            {
                Console.WriteLine($"{count.Key}: {count.Value}");
            }
        }
    }
}