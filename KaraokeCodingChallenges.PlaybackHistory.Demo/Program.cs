using KaraokeCodingChallenges.PlaybackHistory;

PlaybackHistoryManager manager = new();

DateTimeOffset firstQueenPlay = new(2026, 9, 24, 18, 30, 0, TimeSpan.Zero);

DateTimeOffset journeyPlay = new(2026, 9, 24, 18, 45, 0, TimeSpan.Zero);

DateTimeOffset secondQueenPlay = new(2026, 9, 24, 19, 0, 0, TimeSpan.Zero);

manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", firstQueenPlay);

manager.RecordPlay("Journey - Don't Stop Believin'.mp3", journeyPlay);

manager.RecordPlay("Queen - Bohemian Rhapsody.mp3", secondQueenPlay);

Console.WriteLine("Playback History");
Console.WriteLine();

foreach (PlaybackHistoryEntry entry in manager.GetRecentHistory())
{
    Console.WriteLine($"Track: {entry.TrackPath}");
    Console.WriteLine($"Played at: {entry.PlayedAt}");
    Console.WriteLine($"Play count: {manager.GetPlayCount(entry.TrackPath)}");
    Console.WriteLine();
}

Console.WriteLine("Track Summary");
Console.WriteLine();

string queenTrack = "Queen - Bohemian Rhapsody.mp3";

Console.WriteLine($"Track: {queenTrack}");
Console.WriteLine($"Play count: {manager.GetPlayCount(queenTrack)}");
Console.WriteLine($"Last played: {manager.GetLastPlayed(queenTrack)}");