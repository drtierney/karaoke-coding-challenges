namespace KaraokeCodingChallenges.FileValidation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string?[] testFiles =
            {
                @"D:\KaraokeTest\Queen - Bohemian Rhapsody.mp3",
                @"D:\KaraokeTest\Queen - Bohemian Rhapsody.cdg",
                @"D:\KaraokeTest\Oasis - Wonderwall.FLAC",
                @"D:\KaraokeTest\cover.jpg",
                @"D:\KaraokeTest\missing.mp3",
                "",
                null
            };

            foreach (string? testFile in testFiles)
            {
                FileValidationStatus validationStatus = MediaFileValidator.Validate(testFile);

                string displayFile = testFile switch
                {
                    null => "<null>",
                    "" => "<empty>",
                    _ => testFile
                };

                Console.WriteLine($"File: {displayFile} -> Validation Status: {validationStatus}");
            }
        }
    }
}
