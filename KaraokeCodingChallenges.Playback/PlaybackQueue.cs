using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.Playback;

public class PlaybackQueue
{
    private readonly List<MediaTrackModel> _tracks = [];

    public IReadOnlyCollection<MediaTrackModel> Tracks => _tracks.AsReadOnly();

    public int Count => _tracks.Count;

    public void Enqueue(MediaTrackModel track)
    {
        _tracks.Add(track);
    }

    public void QueueNext(MediaTrackModel track)
    {
        _tracks.Insert(0, track);
    }

    public MediaTrackModel? Peek()
    {
        return _tracks.Count > 0 ? _tracks[0] : null;
    }

    public MediaTrackModel? Dequeue()
    {
        if (_tracks.Count == 0)
        {
            return null;
        }

        MediaTrackModel track = _tracks[0];
        _tracks.RemoveAt(0);

        return track;
    }

    public bool Remove(MediaTrackModel track)
    {
        return _tracks.Remove(track);
    }

    public void Clear()
    {
        _tracks.Clear();
    }
}