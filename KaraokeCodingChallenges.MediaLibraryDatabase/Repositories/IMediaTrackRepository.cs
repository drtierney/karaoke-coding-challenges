using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories
{
    public interface IMediaTrackRepository
    {
        long Add(MediaTrackRecord track);

        long Add(MediaTrackRecord track, SqliteConnection connection, SqliteTransaction transaction);

        MediaTrackRecord? GetById(long id);

        MediaTrackRecord? GetByFilePath(string filePath);

        MediaTrackRecord? GetByFilePath(string filePath, SqliteConnection connection, SqliteTransaction transaction);

        IReadOnlyList<MediaTrackRecord> GetAll();

        bool Update(MediaTrackRecord track);

        bool Update(MediaTrackRecord track, SqliteConnection connection, SqliteTransaction transaction);

        bool Delete(long id);
    }
}
