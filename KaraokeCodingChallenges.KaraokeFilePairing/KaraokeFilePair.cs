namespace KaraokeCodingChallenges.KaraokeFilePairing;

public class KaraokeFilePair
{
    public string Mp3FilePath { get; set; }
    public string CdgFilePath { get; set; }

    public KaraokeFilePair(string mp3FilePath, string cdgFilePath)
    {
        Mp3FilePath = mp3FilePath;
        CdgFilePath = cdgFilePath;
    }
}