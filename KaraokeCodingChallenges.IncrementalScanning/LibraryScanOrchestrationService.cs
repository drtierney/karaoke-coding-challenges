using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
using KaraokeCodingChallenges.KaraokeFilePairing;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.MetadataReader;

namespace KaraokeCodingChallenges.IncrementalScanning;

public class LibraryScanOrchestrationService
{
    private readonly MediaTrackRepository _repository;
    private readonly FileFingerprintProvider _fingerprintProvider;
    private readonly LibraryReconciliationService _reconciliationService;
    private readonly LibraryReconciliationPersistenceService _reconciliationPersistenceService;
    private readonly IncrementalScanService _incrementalScanService;
    private readonly IKaraokeFileRepository _karaokeFileRepository;

    public LibraryScanOrchestrationService(
        MediaTrackRepository repository,
        FileFingerprintProvider fingerprintProvider,
        LibraryReconciliationService reconciliationService,
        LibraryReconciliationPersistenceService reconciliationPersistenceService,
        IncrementalScanService incrementalScanService,
        IKaraokeFileRepository karaokeFileRepository)
    {
        _repository = repository;
        _fingerprintProvider = fingerprintProvider;
        _reconciliationService = reconciliationService;
        _reconciliationPersistenceService = reconciliationPersistenceService;
        _incrementalScanService = incrementalScanService;
        _karaokeFileRepository = karaokeFileRepository;
    }

    public LibraryReconciliationResult Reconcile(
        long sourceId,
        IReadOnlyCollection<string> scannedPaths,
        bool discoveryCompletedSuccessfully)
    {
        IReadOnlyList<MediaTrackRecord> storedTracks = _repository.GetBySourceId(sourceId);

        if (!discoveryCompletedSuccessfully)
        {
            return _reconciliationService.Reconcile(
                sourceId,
                storedTracks,
                scannedPaths,
                scanCompletedSuccessfully: false);
        }

        HashSet<string> storedPaths = new(storedTracks.Select(track => track.FilePath), StringComparer.OrdinalIgnoreCase);

        List<FileFingerprint> newFiles = [];

        foreach (string path in scannedPaths)
        {
            if (storedPaths.Contains(path))
            {
                continue;
            }

            FileFingerprint fingerprint = _fingerprintProvider.GetFingerprint(path);

            newFiles.Add(fingerprint);
        }

        LibraryReconciliationResult result = _reconciliationService.Reconcile(
            sourceId,
            storedTracks,
            scannedPaths,
            scanCompletedSuccessfully: true,
            newFiles: newFiles);

        _reconciliationPersistenceService.Apply(result);

        return result;
    }

    public (LibraryReconciliationResult Reconciliation, IncrementalScanSummary Scan)
        ProcessLibrary(
            long sourceId,
            IReadOnlyCollection<string> scannedPaths,
            bool discoveryCompletedSuccessfully,
            IMetadataReader metadataReader,
            bool isKaraoke = false)
    {
        if (!discoveryCompletedSuccessfully)
        {
            throw new InvalidOperationException("Cannot process a library when filesystem discovery is incomplete.");
        }

        List<string> audioPaths = [];

        foreach (string path in scannedPaths)
        {
            if (!isKaraoke || string.Equals(Path.GetExtension(path), ".mp3", StringComparison.OrdinalIgnoreCase))
            {
                audioPaths.Add(path);
            }
        }

        Dictionary<string, string> cdgPathsByMp3 = new(StringComparer.OrdinalIgnoreCase);

        if (isKaraoke)
        {
            KaraokeFilePairingService pairingService = new();

            pairingService.PairFiles(scannedPaths);

            foreach (KaraokeFilePair pair in pairingService.MatchedPairs)
            {
                cdgPathsByMp3[pair.Mp3FilePath] = pair.CdgFilePath;
            }
        }

        LibraryReconciliationResult reconciliation = Reconcile(
            sourceId,
            audioPaths,
            discoveryCompletedSuccessfully);

        IncrementalScanSummary scan = _incrementalScanService.ProcessFiles(
            audioPaths,
            sourceId,
            metadataReader,
            isKaraoke,
            cdgPathsByMp3);

        if (isKaraoke)
        {
            SynchronizeCdgRelationships(sourceId, scannedPaths);
        }

        return (reconciliation, scan);
    }

    private void SynchronizeCdgRelationships(
        long sourceId,
        IReadOnlyCollection<string> scannedPaths)
    {
        KaraokeFilePairingService karaokeFilePairingService = new();

        karaokeFilePairingService.PairFiles(scannedPaths);

        foreach (KaraokeFilePair pair in karaokeFilePairingService.MatchedPairs)
        {
            MediaTrackRecord? track = _repository.GetByFilePath(pair.Mp3FilePath);

            if (track is null || track.SourceId != sourceId || !track.IsAvailable)
            {
                continue;
            }

            string? existingCdgPath = _karaokeFileRepository.GetByTrackId(track.Id);

            if (string.Equals(existingCdgPath, pair.CdgFilePath, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Upsert only when the relationship is new or different.
            _karaokeFileRepository.Upsert(track.Id, pair.CdgFilePath);
        }

        foreach (string path in karaokeFilePairingService.UnmatchedMp3Files)
        {
            MediaTrackRecord? track = _repository.GetByFilePath(path);

            if (track is null || track.SourceId != sourceId || !track.IsAvailable)
            {
                continue;
            }

            _karaokeFileRepository.DeleteByTrackId(track.Id);
        }
    }
}
