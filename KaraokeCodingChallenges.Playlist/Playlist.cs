using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.Playlist
{
    public class Playlist
    {
        public Guid Id { get; } = Guid.NewGuid();

        public required string Name { get; init; }

        private readonly List<MediaTrackModel> _tracks = [];

        public IReadOnlyList<MediaTrackModel> Tracks => _tracks;

        public int Count => _tracks.Count;

        public void AddTrack(MediaTrackModel track)
        {
            _tracks.Add(track);
        }

        public bool RemoveTrack(MediaTrackModel track)
        {
            return _tracks.Remove(track);
        }

        public void Clear()
        {
            _tracks.Clear();
        }
    }
}
