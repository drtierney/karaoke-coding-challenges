using System.Collections.Generic;
using MediaTrackModel = KaraokeCodingChallenges.MediaTrack.MediaTrack;

namespace KaraokeCodingChallenges.MediaLibrary
{
    public class MediaLibrary
    {
        private List<MediaTrackModel> library = [];

        public IReadOnlyList<MediaTrackModel> Tracks
        {
            get { return library.AsReadOnly(); }
        }

        public int Count
        {
            get { return library.Count; }
        }

        public void AddTrack(MediaTrackModel track)
        {
            library.Add(track);
        }

        public void AddTracks(IEnumerable<MediaTrackModel> tracks)
        {
            library.AddRange(tracks);
        }

        public bool RemoveTrack(MediaTrackModel track)
        {
            return library.Remove(track);
        }

        public bool RemoveTrack(string filePath)
        {
            MediaTrackModel? trackToRemove = FindTrackByFilePath(filePath);

            if (trackToRemove != null)
            {
                return library.Remove(trackToRemove);
            }

            return false;
        }

        public MediaTrackModel? FindTrackByFilePath(string filePath)
        {
            return library.Find(track =>
                string.Equals(
                    track.FilePath,
                    filePath,
                    StringComparison.OrdinalIgnoreCase));
        }

        public void Clear()
        {
            library.Clear();
        }
    }
}