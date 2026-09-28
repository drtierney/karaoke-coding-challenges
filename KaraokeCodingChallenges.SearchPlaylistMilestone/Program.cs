using KaraokeCodingChallenges.Favourites;
using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playback;
using KaraokeCodingChallenges.PlaybackHistory;
using KaraokeCodingChallenges.Playlist;
using KaraokeCodingChallenges.PlaylistFile;
using KaraokeCodingChallenges.ScanResult;
using KaraokeCodingChallenges.SearchPlaylistMilestone;
using KaraokeCodingChallenges.SmartLists;
using KaraokeCodingChallenges.TrackSorting;
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.LibraryScanner;
using KaraokeCodingChallenges.LibrarySourceRules;
using KaraokeCodingChallenges.MediaScanService;
using KaraokeCodingChallenges.MetadataReader;

Console.WriteLine("Search & Playlist Milestone");
Console.WriteLine("===========================\n");

// Composition root
MediaTrackMapper mapper = new();
MediaLibraryBuilder libraryBuilder = new(mapper);
SmartListService smartListService = new();
FavouriteManager favouriteManager = new();
M3uPlaylistReader playlistReader = new();
M3uPlaylistWriter playlistWriter = new();
PlaylistLibraryResolver playlistResolver = new();
PlaybackQueue playbackQueue = new();
PlaybackHistoryManager playbackHistory = new();
LibraryScannerService scanner = new();
TagLibMetadataReader metadataReader = new();

// Load and validate configuration
AppConfiguration config;

try
{
    config = ConfigurationLoader.Load("appsettings.json");
}
catch (FileNotFoundException)
{
    Console.WriteLine("Configuration file not found.");
    return;
}
catch (System.Text.Json.JsonException)
{
    Console.WriteLine("Configuration file contains invalid JSON.");
    return;
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
    return;
}

IReadOnlyCollection<string> configurationErrors =
    ConfigurationValidator.Validate(config);

if (configurationErrors.Count > 0)
{
    Console.WriteLine("Configuration errors:");

    foreach (string error in configurationErrors)
    {
        Console.WriteLine($"- {error}");
    }

    return;
}

if (!config.LibrarySources.Any(source => source.Enabled))
{
    Console.WriteLine("No library sources are enabled.");
    return;
}

Console.WriteLine("Configuration loaded.\n");

// Scan enabled library sources
List<string> files = [];
List<string> karaokeFilePaths = [];

foreach (LibrarySource source in config.LibrarySources)
{
    if (!source.Enabled)
    {
        Console.WriteLine(
            $"Skipping disabled {source.Type} source: {source.Path}"
        );

        continue;
    }

    if (!Directory.Exists(source.Path))
    {
        Console.WriteLine(
            $"Library source unavailable: {source.Type} - {source.Path}"
        );

        continue;
    }

    Console.WriteLine(
        $"Scanning {source.Type} source: {source.Path}"
    );

    List<string> sourceFiles = scanner.Scan(source.Path);

    karaokeFilePaths.AddRange(
        LibrarySourceRulesService.GetKaraokeFiles(
            source.Type,
            sourceFiles
        )
    );

    files.AddRange(sourceFiles);

    Console.WriteLine(
        $"Total files found in {source.Type} source: {sourceFiles.Count}"
    );

    Console.WriteLine();
}

if (files.Count == 0)
{
    Console.WriteLine("No supported media files found.");
    return;
}

HashSet<string> metadataExtensions =
    new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3",
        ".flac",
        ".opus"
    };

List<string> metadataFiles = files
    .Where(file =>
        metadataExtensions.Contains(Path.GetExtension(file)))
    .ToList();

Console.WriteLine($"Files found: {files.Count}");
Console.WriteLine($"Metadata files: {metadataFiles.Count}");
Console.WriteLine($"Karaoke files: {karaokeFilePaths.Count}");
Console.WriteLine();

List<MediaScanResult> scanResults =
    MediaScannerService
        .ScanFiles(metadataFiles, metadataReader)
        .ToList();

// Build the in-memory media library
MediaLibraryCollection library = libraryBuilder.Build(scanResults, karaokeFilePaths);

Console.WriteLine("Media Library");
Console.WriteLine("-------------");

foreach (MediaTrackModel track in library.Tracks)
{
    string mediaType = track.IsKaraoke ? "Karaoke" : "Music";

    Console.WriteLine(
        $"{track.DisplayName} [{mediaType}] ({track.GetFormattedDuration()})"
    );
}

Console.WriteLine($"\nTotal tracks: {library.Count}");

// Filter and sort the library
SmartList<MediaTrackModel> karaokeSmartList = new(
    "Karaoke Tracks",
    SmartListMatchMode.All
);

karaokeSmartList.AddRule(
    MediaTrackRules.KaraokeOnly()
);

IReadOnlyList<MediaTrackModel> filteredTracks =
    smartListService.Apply(karaokeSmartList, library.Tracks);

List<MediaTrackModel> sortedTracks =
    MediaLibrarySortService
        .SortByDurationAscending(filteredTracks)
        .ToList();

Console.WriteLine("\nKaraoke Tracks - Shortest First");
Console.WriteLine("-------------------------------");

foreach (MediaTrackModel track in sortedTracks)
{
    Console.WriteLine(
        $"{track.DisplayName} [Karaoke] ({track.GetFormattedDuration()})"
    );
}

if (sortedTracks.Count == 0)
{
    Console.WriteLine("\nNo karaoke tracks found.");
    return;
}

// Add a favourite
MediaTrackModel favouriteTrack =
    sortedTracks.Count > 1
        ? sortedTracks[1]
        : sortedTracks[0];

favouriteManager.AddFavourite(favouriteTrack.FilePath);

Console.WriteLine("\nFavourites");
Console.WriteLine("----------");

foreach (MediaTrackModel track in library.Tracks)
{
    if (favouriteManager.IsFavourite(track.FilePath))
    {
        Console.WriteLine(track.DisplayName);
    }
}

// Create a playlist from the filtered tracks
MediaPlaylist playlist = new()
{
    Name = "Karaoke Playlist"
};

foreach (MediaTrackModel track in sortedTracks)
{
    playlist.AddTrack(track);
}

Console.WriteLine($"\nPlaylist: {playlist.Name}");
Console.WriteLine("-----------------------------");

foreach (MediaTrackModel track in playlist.Tracks)
{
    string favouriteMarker =
        favouriteManager.IsFavourite(track.FilePath)
            ? " [Favourite]"
            : string.Empty;

    Console.WriteLine($"{track.DisplayName}{favouriteMarker}");
}

Console.WriteLine($"\nPlaylist tracks: {playlist.Count}");

// Export the playlist to M3U
string playlistFilePath =
    Path.Combine(Path.GetTempPath(), "Karaoke Playlist.m3u");

playlistWriter.Write(playlistFilePath, playlist);

Console.WriteLine($"\nPlaylist exported: {playlistFilePath}");

// Import the playlist from M3U
MediaPlaylist importedPlaylist =
    playlistReader.Read(playlistFilePath);

Console.WriteLine("\nImported Playlist");
Console.WriteLine("-----------------");

foreach (MediaTrackModel track in importedPlaylist.Tracks)
{
    Console.WriteLine(
        $"{track.FilePath} ({track.GetFormattedDuration()})"
    );
}

// Resolve imported tracks against the media library
MediaPlaylist resolvedPlaylist =
    playlistResolver.Resolve(importedPlaylist, library);

Console.WriteLine("\nResolved Playlist");
Console.WriteLine("-----------------");

foreach (MediaTrackModel track in resolvedPlaylist.Tracks)
{
    string mediaType = track.IsKaraoke ? "Karaoke" : "Music";

    string favouriteMarker =
        favouriteManager.IsFavourite(track.FilePath)
            ? " [Favourite]"
            : string.Empty;

    Console.WriteLine(
        $"{track.DisplayName} [{mediaType}] " +
        $"({track.GetFormattedDuration()}){favouriteMarker}"
    );
}

// Queue the resolved playlist for playback
foreach (MediaTrackModel track in resolvedPlaylist.Tracks)
{
    playbackQueue.Enqueue(track);
}

Console.WriteLine("\nPlayback Queue");
Console.WriteLine("--------------");

foreach (MediaTrackModel track in playbackQueue.Tracks)
{
    Console.WriteLine(track.DisplayName);
}

// Simulate playing the next track
MediaTrackModel? playingTrack = playbackQueue.Dequeue();

if (playingTrack is not null)
{
    DateTimeOffset playedAt = DateTimeOffset.Now;

    playbackHistory.RecordPlay(playingTrack.FilePath, playedAt);

    Console.WriteLine("\nNow Playing");
    Console.WriteLine("-----------");
    Console.WriteLine(
        $"{playingTrack.DisplayName} ({playingTrack.GetFormattedDuration()})"
    );

    Console.WriteLine("\nPlayback History");
    Console.WriteLine("----------------");
    Console.WriteLine(
        $"{playingTrack.DisplayName} - " +
        $"Play count: {playbackHistory.GetPlayCount(playingTrack.FilePath)}"
    );
}

// Clean up the temporary playlist file
if (File.Exists(playlistFilePath))
{
    File.Delete(playlistFilePath);
}
