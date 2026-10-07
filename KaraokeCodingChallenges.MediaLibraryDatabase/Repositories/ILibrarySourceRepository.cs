using KaraokeCodingChallenges.MediaLibraryDatabase.Models;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Repositories
{
    public interface ILibrarySourceRepository
    {
        long Add(LibrarySourceRecord source);

        LibrarySourceRecord? GetById(long id);

        IReadOnlyList<LibrarySourceRecord> GetAll();

        bool Update(LibrarySourceRecord source);

        bool Delete(long id);
    }
}
