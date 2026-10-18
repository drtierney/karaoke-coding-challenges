# Challenges 040–041 - Incremental Scanning and Library Reconciliation Demo

Demonstrates incremental library scanning and reconciliation using filesystem fingerprints, SQLite persistence, selective metadata extraction and track availability updates.

The demo performs **six successive passes** through a temporary media library: initial scanning, unchanged files, modified files, a missing file, a moved file and restoration of a missing file. It also verifies that an unsuccessful filesystem discovery does not classify tracks as missing.

## Demonstration

The console application creates a temporary SQLite database and three sample media files:

- Queen - Bohemian Rhapsody.mp3
- ABBA - Dancing Queen.mp3
- Fleetwood Mac - Dreams.mp3

`DemoMetadataReader` supplies predictable metadata and counts extraction requests. The `.mp3` files contain placeholder text, not actual audio, so the scenario is reproducible without external media.

### Pass 1 - Initial Scan

All three files are new. The incremental scanner reads their fingerprints, extracts metadata and inserts three SQLite records.

**Result:** Three added tracks and three metadata reads.

### Pass 2 - Repeat Scan

The files have not changed. Their fingerprints match the stored values, so metadata extraction and persistence are skipped.

**Result:** Three skipped tracks and no additional metadata reads.

### Pass 3 - Modified File Scan

Queen's file content changes. The scanner updates its existing record and fingerprint, preserving the track ID.

**Result:** One updated track, two skipped tracks and one additional metadata read.

### Pass 4 - Missing File

ABBA's file is deleted before a completed discovery.

- Two remaining files are skipped by incremental scanning.
- ABBA is classified as missing and marked unavailable.
- Its SQLite record and original ID are preserved.

**Result:** One missing track, no additional metadata reads and no record deletion.

### Pass 5 - Moved File

Fleetwood Mac's file is moved into a `Classics` subdirectory.

- The new file matches an existing stored fingerprint unambiguously.
- Its existing SQLite row receives the new path while preserving its ID.
- The incremental scanner skips the relocated file without rereading metadata.
- ABBA remains missing and unavailable.

**Result:** One moved track and one still-missing track, without additional metadata reads.

### Pass 6 - Restored File

ABBA's file is recreated with modified content at its original path.

- Reconciliation marks its stored record available again.
- Incremental scanning detects the changed fingerprint and updates the record.
- Its original track ID is preserved.

**Result:** One updated track, two skipped tracks and one additional metadata read.

### Failed Discovery

The demo calls `Reconcile()` with an unsuccessful discovery result. No tracks are classified as missing and no availability updates are applied.

### Final Library State

Three records remain in SQLite, all marked available. The original IDs are retained, and Fleetwood Mac's stored path points to the `Classics` subdirectory.

## Example Output

Paths in the final database listing vary because the demo uses a unique temporary directory.

```text
Challenge 040/041 - Incremental Scanning and Library Reconciliation

Pass 1 - Initial Scan
Total:   3
Added:   3
Updated: 0
Skipped: 0
Failed:  0
Metadata reads: 3

Pass 2 - Repeat Scan
Total:   3
Added:   0
Updated: 0
Skipped: 3
Failed:  0
Metadata reads: 3

Pass 3 - Modified File Scan
Total:   3
Added:   0
Updated: 1
Skipped: 2
Failed:  0
Metadata reads: 4

Modified Track Verification
Track ID preserved: True
Original file size: 18 bytes
Updated file size:  29 bytes

Pass 4 - Missing File Scan
Total:   2
Added:   0
Updated: 0
Skipped: 2
Failed:  0
Metadata reads: 4

Reconciliation
Stored tracks: 3
Scanned paths: 2
Missing tracks: 1
Moved tracks: 0

ABBA marked unavailable: True
ABBA ID preserved: True

Pass 5 - Moved File Scan
Total:   2
Added:   0
Updated: 0
Skipped: 2
Failed:  0
Metadata reads: 4

Reconciliation
Stored tracks: 3
Scanned paths: 2
Missing tracks: 1
Moved tracks: 1

Fleetwood Mac ID preserved: True
Path updated: True

Pass 6 - Restored File Scan
Total:   3
Added:   0
Updated: 1
Skipped: 2
Failed:  0
Metadata reads: 5

Reconciliation
Stored tracks: 3
Scanned paths: 3
Missing tracks: 0
Moved tracks: 0

ABBA available again: True
ABBA ID preserved: True

Failed Discovery Verification
Scan completed: False
Tracks classified as missing: 0

Final Library State
ID: 1 | Available: True | <temporary library>/Queen - Bohemian Rhapsody.mp3
ID: 2 | Available: True | <temporary library>/ABBA - Dancing Queen.mp3
ID: 3 | Available: True | <temporary library>/Classics/Fleetwood Mac - Dreams.mp3

Total stored tracks: 3
```

## Running the Demo

From the repository root:

```powershell
dotnet run --project KaraokeCodingChallenges.IncrementalScanning.Demo
```

The demo targets .NET 10.

## Implementation Details

The demo integrates:

- `MediaLibraryDatabaseInitializer` - initializes temporary SQLite persistence.
- `LibrarySourceRepository` - registers the library source.
- `MediaTrackRepository` - retrieves persisted track records.
- `FileFingerprintProvider` - reads current filesystem fingerprints.
- `FileChangeDetector` - compares current and stored fingerprints.
- `ScannedTrackPersistenceService` - persists scanned metadata and fingerprints.
- `IncrementalScanService` - handles incremental file processing and statistics.
- `LibraryReconciliationService` - compares discovered paths with stored records.
- `FileMoveDetector` - finds unambiguous moved-file candidates through reconciliation.
- `LibraryReconciliationPersistenceService` - applies availability and path updates transactionally.
- `LibraryScanOrchestrationService` - performs reconciliation before incremental scanning.
- `LibraryReconciliationResult` - reports source counts, missing and moved tracks.
- `IncrementalScanSummary` - reports added, updated, skipped and failed counts.
- `DemoMetadataReader` - returns predictable metadata and counts reads.

The database and media files are created in temporary locations and cleaned up on exit.

The demo simulates completed discovery with `Directory.GetFiles()`. It does not implement a discovery service that reports traversal errors: callers must correctly report `discoveryCompletedSuccessfully`. File size and modification time provide heuristic move matching rather than content-verified identity.

## Key Outcomes

- New files are persisted; unchanged files avoid unnecessary metadata reads.
- Modified files update their existing records and fingerprints.
- Missing tracks become unavailable without being deleted.
- Moved tracks retain their IDs and update their stored paths.
- Previously unavailable tracks become available again when rediscovered.
- Failed discoveries do not trigger missing-track reconciliation.
- The final library retains three track identities without duplicates.

The demo verifies workflow behaviour rather than measuring performance.
