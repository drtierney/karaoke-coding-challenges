using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.ScanResult;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Services;

public class KaraokeLibraryPersistenceService
{
    private readonly ScannedTrackPersistenceService _trackPersistenceService;
    private readonly IKaraokeFileRepository _karaokeFileRepository;
    private readonly KaraokeFilePairingService _pairingService;

    public KaraokeLibraryPersistenceService(
        ScannedTrackPersistenceService trackPersistenceService,
        IKaraokeFileRepository karaokeFileRepository,
        KaraokeFilePairingService pairingService)
    {
        _trackPersistenceService = trackPersistenceService;
        _karaokeFileRepository = karaokeFileRepository;
        _pairingService = pairingService;
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

        long? trackId = _trackPersistenceService.Persist(mp3ScanResult, sourceId, isKaraoke: true);

        if (trackId is null)
        {
            return null;
        }

        _karaokeFileRepository.Upsert(trackId.Value, cdgFilePath);

        return trackId;
    }
}
