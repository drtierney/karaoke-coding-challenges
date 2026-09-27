using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.SmartLists;

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

Console.WriteLine("All Tracks");
Console.WriteLine();

foreach (MediaTrackModel track in tracks)
{
    Console.WriteLine(track);
    Console.WriteLine();
}

SmartList smartList = new("Queen Karaoke");

smartList.AddRule(
    new SmartListRule(
        "Karaoke tracks",
        track => track.IsKaraoke));

smartList.AddRule(
    new SmartListRule(
        "Queen tracks",
        track => track.Artist == "Queen"));

Console.WriteLine($"Smart List: {smartList.Name}");
Console.WriteLine();

foreach (SmartListRule rule in smartList.Rules)
{
    Console.WriteLine($"Rule: {rule.Name}");
}

SmartListService service = new();

IReadOnlyList<MediaTrackModel> results =
    service.Apply(smartList, tracks);

Console.WriteLine();
Console.WriteLine("Matching Tracks");
Console.WriteLine();

foreach (MediaTrackModel track in results)
{
    Console.WriteLine(track);
    Console.WriteLine();
}