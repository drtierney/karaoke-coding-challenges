namespace KaraokeCodingChallenges.KaraokeFilePairing.Tests;

public class KaraokeFilePairingServiceTests
{
    [Fact]
    public void PairFiles_WhenCalledMultipleTimes_ClearsPreviousResults()
    {
        KaraokeFilePairingService service = new();

        string[] firstScan =
        [
            Path.Combine("Karaoke", "Queen.mp3"),
            Path.Combine("Karaoke", "Queen.cdg"),
            Path.Combine("Karaoke", "ABBA.mp3")
        ];

        string[] secondScan =
        [
            Path.Combine("Karaoke", "Muse.cdg")
        ];

        service.PairFiles(firstScan);
        service.PairFiles(secondScan);

        Assert.Empty(service.MatchedPairs);
        Assert.Empty(service.UnmatchedMp3Files);
        Assert.Single(service.UnmatchedCdgFiles);
        Assert.Equal(secondScan[0], service.UnmatchedCdgFiles[0]);
    }

    [Fact]
    public void PairFiles_WhenNamesMatch_ReturnsMatchedPair()
    {
        KaraokeFilePairingService service = new();

        string mp3Path = Path.Combine("Karaoke", "Queen", "Bohemian Rhapsody.mp3");
        string cdgPath = Path.Combine("Karaoke", "Queen", "Bohemian Rhapsody.cdg");

        service.PairFiles([mp3Path, cdgPath]);

        KaraokeFilePair pair = Assert.Single(service.MatchedPairs);

        Assert.Equal(mp3Path, pair.Mp3FilePath);
        Assert.Equal(cdgPath, pair.CdgFilePath);
        Assert.Empty(service.UnmatchedMp3Files);
        Assert.Empty(service.UnmatchedCdgFiles);
    }

    [Fact]
    public void PairFiles_WhenDirectoriesDiffer_DoesNotMatch()
    {
        KaraokeFilePairingService service = new();

        string mp3Path = Path.Combine("Karaoke", "Queen", "Track01.mp3");
        string cdgPath = Path.Combine("Karaoke", "Muse", "Track01.cdg");

        service.PairFiles([mp3Path, cdgPath]);

        Assert.Empty(service.MatchedPairs);
        Assert.Equal(mp3Path, Assert.Single(service.UnmatchedMp3Files));
        Assert.Equal(cdgPath, Assert.Single(service.UnmatchedCdgFiles));
    }

    [Fact]
    public void PairFiles_WhenExtensionsHaveDifferentCasing_ReturnsMatchedPair()
    {
        KaraokeFilePairingService service = new();

        string mp3Path = Path.Combine("Karaoke", "Queen", "Bohemian Rhapsody.MP3");
        string cdgPath = Path.Combine("Karaoke", "Queen", "Bohemian Rhapsody.cdg");

        service.PairFiles([mp3Path, cdgPath]);

        KaraokeFilePair pair = Assert.Single(service.MatchedPairs);

        Assert.Equal(mp3Path, pair.Mp3FilePath);
        Assert.Equal(cdgPath, pair.CdgFilePath);
        Assert.Empty(service.UnmatchedMp3Files);
        Assert.Empty(service.UnmatchedCdgFiles);
    }
}
