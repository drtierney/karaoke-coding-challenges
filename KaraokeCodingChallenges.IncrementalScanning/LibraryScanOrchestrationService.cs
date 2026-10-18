using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.IncrementalScanning.Services;
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

    public LibraryScanOrchestrationService(
        MediaTrackRepository repository,
        FileFingerprintProvider fingerprintProvider,
        LibraryReconciliationService reconciliationService,
        LibraryReconciliationPersistenceService reconciliationPersistenceService,
        IncrementalScanService incrementalScanService)
    {
        _repository = repository;
        _fingerprintProvider = fingerprintProvider;
        _reconciliationService = reconciliationService;
        _reconciliationPersistenceService = reconciliationPersistenceService;
        _incrementalScanService = incrementalScanService;
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

    public (
        LibraryReconciliationResult Reconciliation,
        IncrementalScanSummary Scan)
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

        LibraryReconciliationResult reconciliation = Reconcile(
            sourceId,
            scannedPaths,
            discoveryCompletedSuccessfully);

        IncrementalScanSummary scan = _incrementalScanService.ProcessFiles(
            scannedPaths,
            sourceId,
            metadataReader,
            isKaraoke);

        return (reconciliation, scan);
    }

}
