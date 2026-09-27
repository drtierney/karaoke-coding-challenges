using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.SmartLists;
using KaraokeCodingChallenges.TrackSorting;

List<MediaTrackModel> tracks =
[
    new(
        title: "Bohemian Rhapsody",
        artist: "Queen",
        filePath: @"D:\Karaoke\Queen - Bohemian Rhapsody.mp3",
        durationSeconds: 354,
        isKaraoke: true),

    new(
        title: "Don't Stop Me Now",
        artist: "Queen",
        filePath: @"D:\Music\Queen - Don't Stop Me Now.mp3",
        durationSeconds: 210,
        isKaraoke: false),

    new(
        title: "Misery Business",
        artist: "Paramore",
        filePath: @"D:\Karaoke\Paramore - Misery Business.mp3",
        durationSeconds: 200,
        isKaraoke: true),

    new(
        title: "Still Into You",
        artist: "Paramore",
        filePath: @"D:\Music\Paramore - Still Into You.mp3",
        durationSeconds: 220,
        isKaraoke: false)
];

Console.WriteLine("All Tracks:");
Console.WriteLine();

foreach (MediaTrackModel track in tracks)
{
    string type = track.IsKaraoke ? "Karaoke" : "Music";

    Console.WriteLine(
        $"{track.Artist} - {track.Title} [{type}] ({track.GetFormattedDuration()})");
}

SmartList<MediaTrackModel> smartList = new("Long Karaoke");

smartList.AddRule(MediaTrackRules.KaraokeOnly());
smartList.AddRule(MediaTrackRules.MinimumDuration(180));

Console.WriteLine();
Console.WriteLine($"Smart List: {smartList.Name}");
Console.WriteLine($"Match Mode: {smartList.MatchMode}");
Console.WriteLine();
Console.WriteLine("Rules:");

foreach (SmartListRule<MediaTrackModel> rule in smartList.Rules)
{
    Console.WriteLine($"- {rule.Name}");
}

SmartListService service = new();

IReadOnlyList<MediaTrackModel> results = service.Apply(smartList, tracks);

IEnumerable<MediaTrackModel> sortedResults = MediaLibrarySortService.SortByArtistThenTitle(results);

Console.WriteLine();
Console.WriteLine("Matching Tracks (sorted by artist and title):");
Console.WriteLine();

foreach (MediaTrackModel track in sortedResults)
{
    string type = track.IsKaraoke ? "Karaoke" : "Music";

    Console.WriteLine(
        $"{track.Artist} - {track.Title} [{type}] ({track.GetFormattedDuration()})");
}

Console.WriteLine();
Console.WriteLine($"{results.Count} of {tracks.Count} tracks matched.");
