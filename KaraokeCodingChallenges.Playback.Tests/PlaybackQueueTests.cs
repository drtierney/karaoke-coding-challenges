using KaraokeCodingChallenges.MediaTrack;
using KaraokeCodingChallenges.Playback;

namespace KaraokeCodingChallenges.Playback.Tests;

public class PlaybackQueueTests
{
    [Fact]
    public void PlaybackQueue_WhenCreated_StartsEmpty()
    {
        PlaybackQueue queue = new();

        Assert.Empty(queue.Tracks);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Enqueue_AddsTrackToQueue()
    {
        PlaybackQueue queue = new();

        MediaTrackModel track = new(
            title: "Bohemian Rhapsody", 
            artist: "Queen", 
            filePath: @"D:\Music\Queen\Bohemian Rhapsody.mp3"
        );

        queue.Enqueue(track);

        Assert.Single(queue.Tracks);
        Assert.Equal(track, queue.Tracks.First());
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Enqueue_PreservesInsertionOrder()
    {
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

        queue.Enqueue(firstTrack);
        queue.Enqueue(secondTrack);

        Assert.Equal(firstTrack, queue.Tracks.ElementAt(0));
        Assert.Equal(secondTrack, queue.Tracks.ElementAt(1));
    }

    [Fact]
    public void Peek_ReturnsFirstTrackWithoutRemovingIt()
    {
        PlaybackQueue queue = new();

        MediaTrackModel track = new(
            title: "Bohemian Rhapsody",
            artist: "Queen",
            filePath: @"D:\Music\Queen\Bohemian Rhapsody.mp3"
        );

        queue.Enqueue(track);

        MediaTrackModel? result = queue.Peek();

        Assert.Equal(track, result);
        Assert.Single(queue.Tracks);
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Peek_WhenQueueIsEmpty_ReturnsNull()
    {
        PlaybackQueue queue = new();

        MediaTrackModel? result = queue.Peek();

        Assert.Null(result);
    }

    [Fact]
    public void Dequeue_ReturnsAndRemovesFirstTrack()
    {
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

        queue.Enqueue(firstTrack);
        queue.Enqueue(secondTrack);

        MediaTrackModel? result = queue.Dequeue();

        Assert.Equal(firstTrack, result);
        Assert.Single(queue.Tracks);
        Assert.Equal(secondTrack, queue.Tracks.First());
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Dequeue_WhenQueueIsEmpty_ReturnsNull()
    {
        PlaybackQueue queue = new();

        MediaTrackModel? result = queue.Dequeue();

        Assert.Null(result);
    }

    [Fact]
    public void QueueNext_InsertsTrackAtStartOfQueue()
    {
        PlaybackQueue queue = new();

        MediaTrackModel firstTrack = new(
            title: "Bohemian Rhapsody",
            artist: "Queen",
            filePath: @"D:\Music\Queen\Bohemian Rhapsody.mp3"
        );

        MediaTrackModel nextTrack = new(
            title: "Don't Stop Me Now",
            artist: "Queen",
            filePath: @"D:\Music\Queen\Don't Stop Me Now.mp3"
        );

        queue.Enqueue(firstTrack);

        queue.QueueNext(nextTrack);

        Assert.Equal(nextTrack, queue.Tracks.First());
        Assert.Equal(firstTrack, queue.Tracks.ElementAt(1));
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void QueueNext_PreservesExistingQueueOrder()
    {
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

        queue.QueueNext(nextTrack);

        Assert.Equal(nextTrack, queue.Tracks.ElementAt(0));
        Assert.Equal(firstTrack, queue.Tracks.ElementAt(1));
        Assert.Equal(secondTrack, queue.Tracks.ElementAt(2));
    }

    [Fact]
    public void Remove_WhenTrackExists_RemovesTrack()
    {
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

        queue.Enqueue(firstTrack);
        queue.Enqueue(secondTrack);

        bool result = queue.Remove(firstTrack);

        Assert.True(result);
        Assert.Single(queue.Tracks);
        Assert.Equal(secondTrack, queue.Tracks.First());
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Remove_WhenTrackDoesNotExist_ReturnsFalse()
    {
        PlaybackQueue queue = new();

        MediaTrackModel queuedTrack = new(
            title: "Bohemian Rhapsody",
            artist: "Queen",
            filePath: @"D:\Music\Queen\Bohemian Rhapsody.mp3"
        );

        MediaTrackModel missingTrack = new(
            title: "Don't Stop Me Now",
            artist: "Queen",
            filePath: @"D:\Music\Queen\Don't Stop Me Now.mp3"
        );

        queue.Enqueue(queuedTrack);

        bool result = queue.Remove(missingTrack);

        Assert.False(result);
        Assert.Single(queue.Tracks);
        Assert.Equal(queuedTrack, queue.Tracks.First());
    }

    [Fact]
    public void Clear_RemovesAllTracks()
    {
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

        queue.Enqueue(firstTrack);
        queue.Enqueue(secondTrack);

        queue.Clear();

        Assert.Empty(queue.Tracks);
        Assert.Equal(0, queue.Count);
    }
}

