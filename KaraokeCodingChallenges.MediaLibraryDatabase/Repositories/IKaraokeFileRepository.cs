namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

public interface IKaraokeFileRepository
{
    string? GetByTrackId(long trackId);

    void Upsert(long trackId, string cdgFilePath);

    void DeleteByTrackId(long trackId);
}
