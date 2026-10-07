using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories
{
    public interface IMediaTrackRepository
    {
        long Add(MediaTrackRecord track);

        MediaTrackRecord? GetById(long id);

        MediaTrackRecord? GetByFilePath(string filePath);

        IReadOnlyList<MediaTrackRecord> GetAll();

        bool Update(MediaTrackRecord track);

        bool Delete(long id);
    }
}
