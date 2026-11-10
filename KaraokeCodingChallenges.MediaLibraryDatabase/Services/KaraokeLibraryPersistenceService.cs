using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.ScanResult;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Services;

public class KaraokeLibraryPersistenceService
{
    private readonly ScannedTrackPersistenceService _trackPersistenceService;
    private readonly IKaraokeFileRepository _karaokeFileRepository;
    private readonly KaraokeFilePairingService _pairingService;
    private readonly string _connectionString;

    public KaraokeLibraryPersistenceService(
        ScannedTrackPersistenceService trackPersistenceService,
        IKaraokeFileRepository karaokeFileRepository,
        KaraokeFilePairingService pairingService,
        string connectionString)
    {
        _trackPersistenceService = trackPersistenceService;
        _karaokeFileRepository = karaokeFileRepository;
        _pairingService = pairingService;
        _connectionString = connectionString;
    }

    public long? PersistPair(
        MediaScanResult mp3ScanResult,
        string cdgFilePath,
        long sourceId)
    {
        if (!mp3ScanResult.IsSuccess || mp3ScanResult.Metadata is null)
        {
            return null;
        }

        string mp3FilePath = mp3ScanResult.FilePath;

        _pairingService.PairFiles([mp3FilePath, cdgFilePath]);

        if (_pairingService.MatchedPairs.Count != 1)
        {
            return null;
        }

        using SqliteConnection connection = new(_connectionString);

        connection.Open();

        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            long? trackId = _trackPersistenceService.Persist(
                mp3ScanResult,
                sourceId,
                connection,
                transaction,
                isKaraoke: true);

            if (trackId is null)
            {
                return null;
            }

            _karaokeFileRepository.Upsert(
                trackId.Value,
                cdgFilePath,
                connection,
                transaction);

            transaction.Commit();

            return trackId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
