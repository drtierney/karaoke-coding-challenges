using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playback;

PlaybackQueue queue = new();

MediaTrackModel firstTrack = new(
    title: "Bohemian Rhapsody",
    artist: "Queen",
    filePath: @"D:\Music\Queen\Bohemian Rhapsody.mp3"
);

MediaTrackModel secondTrack = new(
    title: "Don't Stop Me Now",
    artist: "Queen",
    filePath: @"D:\Music\Queen\Don't Stop Me Now.mp3"
);

MediaTrackModel nextTrack = new(
    title: "Somebody To Love",
    artist: "Queen",
    filePath: @"D:\Music\Queen\Somebody To Love.mp3"
);

queue.Enqueue(firstTrack);
queue.Enqueue(secondTrack);

Console.WriteLine("Initial queue:");
DisplayQueue(queue);

queue.QueueNext(nextTrack);

Console.WriteLine();
Console.WriteLine("After QueueNext:");
DisplayQueue(queue);

Console.WriteLine();
Console.WriteLine($"Peek: {queue.Peek()?.Title}");

MediaTrackModel? dequeuedTrack = queue.Dequeue();

Console.WriteLine();
Console.WriteLine($"Dequeued: {dequeuedTrack?.Title}");
DisplayQueue(queue);

queue.Remove(secondTrack);

Console.WriteLine();
Console.WriteLine("After Remove:");
DisplayQueue(queue);

queue.Clear();

Console.WriteLine();
Console.WriteLine("After Clear:");
DisplayQueue(queue);

static void DisplayQueue(PlaybackQueue queue)
{
    Console.WriteLine($"Count: {queue.Count}");

    foreach (MediaTrackModel track in queue.Tracks)
    {
        Console.WriteLine($"- {track.Artist} - {track.Title}");
    }
}