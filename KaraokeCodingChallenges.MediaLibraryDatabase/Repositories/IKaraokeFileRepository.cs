using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

public interface IKaraokeFileRepository
{
    string? GetByTrackId(long trackId);

    void Upsert(long trackId, string cdgFilePath);

    void Upsert(long trackId, string cdgFilePath, SqliteConnection connection, SqliteTransaction transaction);

    void DeleteByTrackId(long trackId);
}
