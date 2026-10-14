# Challenge 040 - Incremental Library Scanning Demo

Demonstrates incremental media library scanning using filesystem fingerprints, SQLite persistence, and selective metadata extraction.

The demo performs three consecutive scans of the same media library to demonstrate how new, unchanged, and modified files are handled without unnecessarily reprocessing existing tracks.

## Demonstration

The console application creates a temporary SQLite database and three example media files:

- Queen - Bohemian Rhapsody.mp3
- ABBA - Dancing Queen.mp3
- Fleetwood Mac - Dreams.mp3

A `DemoMetadataReader` implementation provides predictable metadata and records how many times metadata extraction is requested.

The example files contain placeholder content rather than real audio. This keeps the demonstration reproducible without requiring external media files.

### Pass 1 - Initial Scan

All three files are new and have no existing SQLite records.

The incremental scanner:

- Retrieves their filesystem fingerprints.
- Extracts metadata for each file.
- Persists the tracks and their fingerprints.
- Reports three added tracks.

**Result:** Three metadata reads and three new database records.

### Pass 2 - Repeat Scan

The same files are scanned again without any modifications.

The incremental scanner:

- Compares current fingerprints with the stored SQLite values.
- Identifies all three files as unchanged.
- Skips metadata extraction and persistence.
- Reports three skipped tracks.

**Result:** No additional metadata reads.

### Pass 3 - Modified File Scan

The contents of `Queen - Bohemian Rhapsody.mp3` are modified before scanning again.

The incremental scanner:

- Identifies the modified file using its updated fingerprint.
- Extracts metadata for the modified file.
- Updates the existing SQLite record.
- Skips the remaining two unchanged files.
- Preserves the modified track's original database identifier.

**Result:** One additional metadata read, one updated track, and two skipped tracks.

## Example Output

```text
Challenge 040 - Incremental Library Scanning Demo

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
```

## Running the Demo

From the repository root:

```powershell
dotnet run --project .\KaraokeCodingChallenges.IncrementalScanning.Demo\
```

The demo targets .NET 10.

## Implementation Details

The demo integrates the following components:

- `MediaLibraryDatabaseInitializer` - initializes the temporary SQLite database.
- `LibrarySourceRepository` - registers the temporary library source.
- `MediaTrackRepository` - retrieves and persists track records.
- `FileFingerprintProvider` - reads filesystem fingerprints.
- `FileChangeDetector` - compares current and stored fingerprints.
- `ScannedTrackPersistenceService` - persists scanned metadata and fingerprints.
- `IncrementalScanService` - coordinates individual file classification and batch scanning.
- `IncrementalScanSummary` - provides per-pass statistics.
- `DemoMetadataReader` - supplies predictable metadata and counts extraction calls.

The database and media files are created in temporary locations and deleted when execution completes, including when the demo exits through an exception.

## Key Outcomes

The demonstration confirms that:

- New files are added to the persistent media library.
- Unchanged files are skipped without repeated metadata extraction.
- Modified files are rescanned and updated.
- Existing track identifiers are preserved.
- Scan statistics accurately reflect each processing pass.

The demo validates incremental scanning behaviour rather than benchmarking performance.