using KaraokeCodingChallenges.Configuration;

namespace KaraokeCodingChallenges.LibrarySourceRules.Tests;

public class LibrarySourceRulesServiceTests
{
    [Fact]
    public void GetKaraokeFiles_MusicSource_ReturnsNoFiles()
    {
        List<string> sourceFiles =
        [
        @"D:\Music\Artist - Song.mp3",
        @"D:\Music\Artist - Another Song.flac"
        ];

        IReadOnlyCollection<string> result = LibrarySourceRulesService.GetKaraokeFiles(LibrarySourceType.Music, sourceFiles);

        Assert.Empty(result);
    }

    [Fact]
    public void GetKaraokeFiles_KaraokeSource_ReturnsAllFiles()
    {
        List<string> sourceFiles =
        [
        @"D:\Karaoke\Song.mp3",
        @"D:\Karaoke\Song.cdg"
        ];

        IReadOnlyCollection<string> result = LibrarySourceRulesService.GetKaraokeFiles(LibrarySourceType.Karaoke, sourceFiles);

        Assert.Equal(sourceFiles.Count, result.Count);
        Assert.Contains(sourceFiles[0], result);
        Assert.Contains(sourceFiles[1], result);
    }

    [Fact]
    public void GetKaraokeFiles_MixedSource_ExcludesStandaloneMp3()
    {
        List<string> sourceFiles =
        [
        @"D:\Mixed\Karaoke Song.mp3",
        @"D:\Mixed\Karaoke Song.cdg",
        @"D:\Mixed\Music Only.mp3"
        ];

        IReadOnlyCollection<string> result = LibrarySourceRulesService.GetKaraokeFiles(LibrarySourceType.Mixed, sourceFiles);

        Assert.Contains(@"D:\Mixed\Karaoke Song.mp3", result);
        Assert.Contains(@"D:\Mixed\Karaoke Song.cdg", result);
        Assert.DoesNotContain(@"D:\Mixed\Music Only.mp3", result);
    }

    [Fact]
    public void GetKaraokeFiles_MixedSource_IncludesUnmatchedCdg()
    {
        List<string> sourceFiles =
        [
        @"D:\Mixed\Karaoke Song.mp3",
        @"D:\Mixed\Karaoke Song.cdg",
        @"D:\Mixed\Orphan Song.cdg"
        ];

        IReadOnlyCollection<string> result = LibrarySourceRulesService.GetKaraokeFiles(LibrarySourceType.Mixed, sourceFiles);

        Assert.Contains(@"D:\Mixed\Orphan Song.cdg", result);
    }

    [Fact]
    public void GetKaraokeFiles_MixedSource_DoesNotPairSameNameFromDifferentDirectories()
    {
        List<string> sourceFiles =
        [
        @"D:\Mixed\FolderA\Song.mp3",
        @"D:\Mixed\FolderB\Song.cdg"
        ];

        IReadOnlyCollection<string> result = LibrarySourceRulesService.GetKaraokeFiles(LibrarySourceType.Mixed, sourceFiles);

        Assert.DoesNotContain(@"D:\Mixed\FolderA\Song.mp3", result);
        Assert.Contains(@"D:\Mixed\FolderB\Song.cdg", result);
    }

    [Fact]
    public void GetKaraokeFiles_MixedSource_MatchesExtensionsCaseInsensitively()
    {
        List<string> sourceFiles =
        [
        @"D:\Mixed\Song.MP3",
        @"D:\Mixed\Song.CDG"
        ];

        IReadOnlyCollection<string> result = LibrarySourceRulesService.GetKaraokeFiles(LibrarySourceType.Mixed, sourceFiles);

        Assert.Contains(@"D:\Mixed\Song.MP3", result);
        Assert.Contains(@"D:\Mixed\Song.CDG", result);
    }
}
