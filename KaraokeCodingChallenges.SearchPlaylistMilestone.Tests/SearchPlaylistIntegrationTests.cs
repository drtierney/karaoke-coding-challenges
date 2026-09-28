using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.LibrarySourceRules;
using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.ScanResult;
using KaraokeCodingChallenges.SmartLists;
using KaraokeCodingChallenges.TrackSorting;
using KaraokeCodingChallenges.Playlist;
using KaraokeCodingChallenges.PlaylistFile;
using KaraokeCodingChallenges.Favourites;
using KaraokeCodingChallenges.Playback;
using KaraokeCodingChallenges.PlaybackHistory;

namespace KaraokeCodingChallenges.SearchPlaylistMilestone.Tests;

public class SearchPlaylistIntegrationTests
{
    [Fact]
    public void Build_FromMixedSource_ClassifiesKaraokeAndMusicTracks()
    {
        string karaokeMp3Path = @"D:\Music\Karaoke Song.mp3";
        string karaokeCdgPath = @"D:\Music\Karaoke Song.cdg";
        string musicMp3Path = @"D:\Music\Music Only.mp3";

        string[] sourceFiles =
        [
            karaokeMp3Path,
            karaokeCdgPath,
            musicMp3Path
        ];

        IReadOnlyCollection<string> karaokeFiles =
            LibrarySourceRulesService.GetKaraokeFiles(
                LibrarySourceType.Mixed,
                sourceFiles);

        MediaScanResult[] scanResults =
        [
            new MediaScanResult
            {
                FilePath = karaokeMp3Path,
                IsSuccess = true,
                Metadata = new()
                {
                    FilePath = karaokeMp3Path,
                    Title = "Karaoke Song",
                    Artist = "Test Artist"
                }
            },
            new MediaScanResult
            {
                FilePath = musicMp3Path,
                IsSuccess = true,
                Metadata = new()
                {
                    FilePath = musicMp3Path,
                    Title = "Music Only",
                    Artist = "Test Artist"
                }
            }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        var library = builder.Build(scanResults, karaokeFiles);

        Assert.Equal(2, library.Count);

        var karaokeTrack = library.FindTrackByFilePath(karaokeMp3Path);
        var musicTrack = library.FindTrackByFilePath(musicMp3Path);

        Assert.NotNull(karaokeTrack);
        Assert.NotNull(musicTrack);

        Assert.True(karaokeTrack.IsKaraoke);
        Assert.False(musicTrack.IsKaraoke);
    }

    [Fact]
    public void BuiltLibrary_CanBeSearchedFilteredAndSorted()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        },
        new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Don't Stop Me Now.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Don't Stop Me Now.mp3",
                Title = "Don't Stop Me Now",
                Artist = "Queen",
                DurationSeconds = 209
            }
        },
        new MediaScanResult
        {
            FilePath = @"D:\Music\Paramore - Misery Business.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Paramore - Misery Business.mp3",
                Title = "Misery Business",
                Artist = "Paramore",
                DurationSeconds = 212
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(scanResults, []);

        SmartList<MediaTrackModel> queenSmartList = new("Queen Tracks");
        queenSmartList.AddRule(MediaTrackRules.ArtistContains("Queen"));

        SmartListService smartListService = new();

        IReadOnlyList<MediaTrackModel> queenTracks = smartListService.Apply(queenSmartList, library.Tracks);

        List<MediaTrackModel> sortedTracks = MediaLibrarySortService.SortByDurationAscending(queenTracks).ToList();

        Assert.Equal(2, sortedTracks.Count);

        Assert.Equal("Don't Stop Me Now", sortedTracks[0].Title);
        Assert.Equal("Bohemian Rhapsody", sortedTracks[1].Title);
    }

    [Fact]
    public void FilteredLibraryTracks_CanBeAddedToPlaylist()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen"
            }
        },
        new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Don't Stop Me Now.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Don't Stop Me Now.mp3",
                Title = "Don't Stop Me Now",
                Artist = "Queen"
            }
        },
        new MediaScanResult
        {
            FilePath = @"D:\Music\Paramore - Misery Business.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Paramore - Misery Business.mp3",
                Title = "Misery Business",
                Artist = "Paramore"
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);
        MediaLibraryCollection library = builder.Build(scanResults, []);

        SmartList<MediaTrackModel> queenSmartList = new("Queen Tracks");
        queenSmartList.AddRule(MediaTrackRules.ArtistContains("Queen"));

        SmartListService smartListService = new();

        IReadOnlyList<MediaTrackModel> queenTracks = smartListService.Apply(queenSmartList, library.Tracks);

        MediaPlaylist playlist = new()
        {
            Name = "Queen Playlist"
        };

        foreach (MediaTrackModel track in queenTracks)
        {
            playlist.AddTrack(track);
        }

        Assert.Equal(2, playlist.Count);

        Assert.Same(queenTracks[0], playlist.Tracks[0]);
        Assert.Same(queenTracks[1], playlist.Tracks[1]);

        Assert.Equal("Bohemian Rhapsody", playlist.Tracks[0].Title);
        Assert.Equal("Don't Stop Me Now", playlist.Tracks[1].Title);
    }

    [Fact]
    public void ImportedPlaylist_WhenTracksExistInLibrary_UsesLibraryTracks()
    {
        MediaTrackModel libraryTrack = new(
            "Bohemian Rhapsody",
            "Queen",
            @"D:\Music\Queen\Bohemian Rhapsody.mp3",
            354
        );

        MediaLibraryCollection library = new();
        library.AddTrack(libraryTrack);

        MediaPlaylist importedPlaylist = new()
        {
            Name = "Queen Playlist"
        };

        importedPlaylist.AddTrack(
            new MediaTrackModel(
                "Bohemian Rhapsody",
                string.Empty,
                @"D:\Music\Queen\Bohemian Rhapsody.mp3"
            )
        );

        PlaylistLibraryResolver resolver = new();

        MediaPlaylist resolvedPlaylist =
            resolver.Resolve(importedPlaylist, library);

        Assert.Single(resolvedPlaylist.Tracks);

        Assert.Same(
            libraryTrack,
            resolvedPlaylist.Tracks[0]
        );

        Assert.Equal("Queen", resolvedPlaylist.Tracks[0].Artist);
        Assert.Equal(354, resolvedPlaylist.Tracks[0].DurationSeconds);
    }

    [Fact]
    public void ImportedPlaylist_WhenTrackIsNotInLibrary_PreservesImportedTrack()
    {
        MediaLibraryCollection library = new();

        MediaTrackModel importedTrack = new(
            "Missing Track",
            string.Empty,
            @"D:\Music\Missing Track.mp3"
        );

        MediaPlaylist importedPlaylist = new()
        {
            Name = "Imported Playlist"
        };

        importedPlaylist.AddTrack(importedTrack);

        PlaylistLibraryResolver resolver = new();

        MediaPlaylist resolvedPlaylist =
            resolver.Resolve(importedPlaylist, library);

        Assert.Single(resolvedPlaylist.Tracks);

        Assert.Same(importedTrack, resolvedPlaylist.Tracks[0]);
    }

    [Fact]
    public void M3uPlaylist_CanBeResolvedAgainstMediaLibrary()
    {
        string tempDirectory =Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        string playlistPath = Path.Combine(tempDirectory, "Mixed Playlist.m3u");

        string libraryTrackPath = Path.Combine(tempDirectory, "Queen - Bohemian Rhapsody.mp3");

        string missingTrackPath = Path.Combine(tempDirectory, "Missing Track.mp3");

        File.WriteAllLines(
            playlistPath,
            [
                libraryTrackPath,
                missingTrackPath
            ]
        );

        MediaTrackModel libraryTrack = new(
            "Bohemian Rhapsody",
            "Queen",
            libraryTrackPath,
            354
        );

        MediaLibraryCollection library = new();
        library.AddTrack(libraryTrack);

        try
        {
            M3uPlaylistReader reader = new();

            MediaPlaylist importedPlaylist = reader.Read(playlistPath);

            PlaylistLibraryResolver resolver = new();

            MediaPlaylist resolvedPlaylist = resolver.Resolve(importedPlaylist, library);

            Assert.Equal("Mixed Playlist", resolvedPlaylist.Name);
            Assert.Equal(2, resolvedPlaylist.Count);

            Assert.Same(libraryTrack, resolvedPlaylist.Tracks[0]);

            Assert.Equal("Queen", resolvedPlaylist.Tracks[0].Artist);
            Assert.Equal(354, resolvedPlaylist.Tracks[0].DurationSeconds);

            Assert.Equal(missingTrackPath, resolvedPlaylist.Tracks[1].FilePath);

            Assert.Equal("Missing Track", resolvedPlaylist.Tracks[1].Title);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    [Fact]
    public void BuiltLibrary_CanIdentifyFavouriteTracks()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen"
            }
        },
        new MediaScanResult
        {
            FilePath = @"D:\Music\Paramore - Misery Business.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Paramore - Misery Business.mp3",
                Title = "Misery Business",
                Artist = "Paramore"
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(scanResults, []);

        FavouriteManager favouriteManager = new();

        favouriteManager.AddFavourite(@"D:\Music\Queen - Bohemian Rhapsody.mp3");

        List<MediaTrackModel> favouriteTracks = library.Tracks.Where(track => favouriteManager.IsFavourite(track.FilePath)).ToList();

        Assert.Single(favouriteTracks);

        Assert.Equal("Bohemian Rhapsody", favouriteTracks[0].Title);
    }

    [Fact]
    public void BuiltLibrary_CanIdentifyFavouriteTracksIgnoringPathCase()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen"
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(scanResults, []);

        FavouriteManager favouriteManager = new();

        favouriteManager.AddFavourite(@"D:\MUSIC\QUEEN - BOHEMIAN RHAPSODY.MP3");

        List<MediaTrackModel> favouriteTracks = library.Tracks.Where(track => favouriteManager.IsFavourite(track.FilePath)).ToList();

        Assert.Single(favouriteTracks);

        Assert.Equal("Bohemian Rhapsody",favouriteTracks[0].Title);
    }

    [Fact]
    public void FilteredLibraryTrack_CanBeQueuedForPlayback()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        },
        new MediaScanResult
        {
            FilePath = @"D:\Music\Paramore - Misery Business.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Paramore - Misery Business.mp3",
                Title = "Misery Business",
                Artist = "Paramore",
                DurationSeconds = 212
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(scanResults, []);

        SmartList<MediaTrackModel> queenSmartList = new("Queen Tracks");

        queenSmartList.AddRule(MediaTrackRules.ArtistContains("Queen"));

        SmartListService smartListService = new();

        IReadOnlyList<MediaTrackModel> queenTracks = smartListService.Apply(queenSmartList, library.Tracks);

        MediaTrackModel selectedTrack = Assert.Single(queenTracks);

        PlaybackQueue queue = new();

        queue.Enqueue(selectedTrack);

        MediaTrackModel? queuedTrack = queue.Peek();

        Assert.Same(selectedTrack, queuedTrack);
        Assert.Same(library.Tracks[0], queuedTrack);

        Assert.Equal("Bohemian Rhapsody", queuedTrack!.Title);
        Assert.Equal("Queen", queuedTrack.Artist);
        Assert.Equal(354, queuedTrack.DurationSeconds);
    }

    [Fact]
    public void QueuedLibraryTrack_CanBePlayedAndRecordedInHistory()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen",
                DurationSeconds = 354
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(scanResults, []);

        MediaTrackModel libraryTrack = Assert.Single(library.Tracks);

        PlaybackQueue queue = new();
        queue.Enqueue(libraryTrack);

        MediaTrackModel? playedTrack = queue.Dequeue();

        Assert.NotNull(playedTrack);
        Assert.Same(libraryTrack, playedTrack);

        PlaybackHistoryManager historyManager = new();

        DateTimeOffset playedAt = new(2026, 9, 28, 18, 0, 0, TimeSpan.Zero);

        historyManager.RecordPlay(playedTrack.FilePath, playedAt);

        Assert.Equal(1, historyManager.GetPlayCount(libraryTrack.FilePath));

        Assert.Equal(playedAt, historyManager.GetLastPlayed(libraryTrack.FilePath));

        PlaybackHistoryEntry historyEntry = Assert.Single(historyManager.GetRecentHistory());

        Assert.Equal(libraryTrack.FilePath, historyEntry.TrackPath);

        Assert.Equal(playedAt, historyEntry.PlayedAt);
    }

    [Fact]
    public void PlaybackHistory_CanFindLibraryTrackIgnoringPathCase()
    {
        MediaScanResult[] scanResults =
        [
            new MediaScanResult
        {
            FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
            IsSuccess = true,
            Metadata = new()
            {
                FilePath = @"D:\Music\Queen - Bohemian Rhapsody.mp3",
                Title = "Bohemian Rhapsody",
                Artist = "Queen"
            }
        }
        ];

        MediaTrackMapper mapper = new();
        MediaLibraryBuilder builder = new(mapper);

        MediaLibraryCollection library = builder.Build(scanResults, []);

        MediaTrackModel libraryTrack = Assert.Single(library.Tracks);

        PlaybackHistoryManager historyManager = new();

        DateTimeOffset playedAt = new(2026, 9, 28, 18, 0, 0, TimeSpan.Zero);

        historyManager.RecordPlay(libraryTrack.FilePath, playedAt);

        string differentCasePath = @"D:\MUSIC\QUEEN - BOHEMIAN RHAPSODY.MP3";

        Assert.Equal(1, historyManager.GetPlayCount(differentCasePath));

        Assert.Equal(playedAt, historyManager.GetLastPlayed(differentCasePath));
    }
}