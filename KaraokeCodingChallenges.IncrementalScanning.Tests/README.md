# Challenges 040–042 - Incremental Scanning, Reconciliation and Watcher Tests

Contains xUnit tests for incremental scanning and SQLite-backed library reconciliation. The suite verifies selective metadata extraction, filesystem fingerprints, track availability updates, moved-file identification, transactional persistence and scan orchestration.

## Challenge 040 - Incremental Scanning Tests

Challenge 040 introduced **24 tests** covering the following areas.

### File Fingerprint Generation

Tests for `FileFingerprintProvider` cover:

- Retrieving a file's full path.
- Reading file size and last-modified UTC timestamp.
- Handling invalid or missing file paths.
- Using temporary files for filesystem verification.

### File Change Detection

Tests for `FileChangeDetector` cover:

- Identifying new files when no previous fingerprint exists.
- Identifying modified files when their sizes differ.
- Identifying modified files when their modification timestamps differ.
- Identifying unchanged files when fingerprints match.

### Incremental File Classification

Tests for `IncrementalScanService.CheckFile()` cover:

- Classifying files without existing SQLite records as new.
- Classifying files with missing stored fingerprint values as modified.
- Detecting modifications to previously persisted files.
- Recognising unchanged files using stored fingerprints.

### Single-File Processing

Tests for `IncrementalScanService.ProcessFile()` cover:

- Scanning and persisting new files.
- Rescanning modified files.
- Preserving existing track identifiers during updates.
- Skipping unchanged files without invoking the metadata reader.
- Avoiding fingerprint updates when metadata extraction fails.

### Batch Processing

Tests for `IncrementalScanService.ProcessFiles()` cover:

- Adding multiple new files.
- Skipping a batch of unchanged files.
- Handling a mixture of new, modified, and unchanged files.
- Continuing after an earlier file is missing.
- Reporting added, updated, skipped, and failed counts correctly.

## Challenge 041 - Library Reconciliation Tests

Challenge 041 extends the project with tests covering:

- Identifying missing and present tracks from discovered paths.
- Handling unsuccessful and successfully empty filesystem discoveries.
- Comparing paths case-insensitively.
- Detecting unambiguous moved files using file size and modification timestamps.
- Rejecting ambiguous moved-file matches.
- Marking missing tracks unavailable without deleting their records.
- Restoring availability when a previously missing track is rediscovered.
- Preserving original track IDs, paths and metadata where applicable.
- Rolling back invalid SQLite reconciliation updates.
- Preventing updates across different library sources.
- Coordinating reconciliation and incremental scanning through `ProcessLibrary()`.
- Skipping recognised moves without inserting duplicates or rereading metadata.
- Persisting genuinely new files and leaving ambiguous move candidates unmatched.

### Challenge 041 Verification

- IncrementalScanning tests: **44 passed**
- Full solution tests: **366 passed**
- Failed: **0**
- Skipped: **0**

These results were recorded on .NET 10 after completing the Challenge 041 implementation.


## Challenge 042 - File System Watcher Tests

Challenge 042 adds tests for filesystem notifications, event debouncing, SQLite-backed library updates and end-to-end event processing.

### LibraryFileWatcher Tests

Verify watcher lifecycle behaviour, including:

- Starting and stopping filesystem monitoring.
- Cancelling pending changed-file notifications when stopped.
- Preventing pending notifications after disposal.
- Rejecting attempts to restart a disposed watcher.

### FileEventDebouncer Tests

Verify:

- A single notification executes its callback.
- Repeated notifications for the same path execute only the latest pending callback.
- Different paths are debounced independently.
- Pending callbacks can be cancelled individually or together.
- Cancellation does not interrupt a callback that has already started.
- Invalid debounce intervals are rejected.

### WatchedLibraryUpdateService Tests

Seven SQLite integration tests verify:

- Deleted files are marked unavailable while preserving their records.
- Unknown deletion paths do not modify existing tracks.
- Renamed files update their stored paths and preserve track IDs.
- Conflicting rename destinations are rejected.
- Created files persist metadata and filesystem fingerprints.
- Unchanged files skip unnecessary metadata reads.
- Modified files update fingerprints while preserving track identity.

These tests use temporary SQLite databases, temporary filesystem files and a controlled `IMetadataReader`.

### WatchedLibraryEventCoordinator Tests

Eight end-to-end tests exercise actual filesystem events through the watcher, coordinator and SQLite persistence services.

Coverage includes:

- File deletion updating track availability.
- File renaming preserving track IDs.
- New supported files being persisted.
- Modified files updating their fingerprints.
- Unsupported files being ignored.
- Renaming from supported to unsupported extensions.
- Renaming from unsupported to supported extensions.
- Rapid deletion cancelling pending file processing.

The tests use bounded asynchronous waits to observe database changes.

The rapid-deletion test covers cancellation before a pending scan executes; it does not exhaustively prove every possible concurrent filesystem event ordering.

### Challenge 042 Verification

- IncrementalScanning tests: **71 passed**
- Full solution tests: **393 passed**
- Failed: **0**
- Skipped: **0**

These results were recorded on .NET 10 following the Challenge 042 implementation and reliability updates.

## Test Infrastructure

The integration tests use:

- **xUnit** for assertions and test execution.
- **Temporary SQLite databases** for persistence verification.
- **Temporary filesystem files and directories** for fingerprinting, modifications and real file moves.
- **Controlled `IMetadataReader` implementations** for successful and failed metadata extraction.

Tests exercise the real repository and persistence implementations rather than using SQLite mocks. Temporary resources are cleaned up after execution.

## SQLite Verification

Tests involving persisted tracks confirm that:

- New tracks receive stored fingerprints.
- Modified and moved tracks retain their SQLite identifiers.
- Fingerprint updates reflect filesystem changes.
- Failed metadata extraction does not overwrite the existing fingerprint.
- Unchanged files do not require metadata reads.
- Missing and present tracks receive the correct `IsAvailable` state.
- Failed reconciliation transactions leave the database unchanged.

Database schema migration and repository-specific fingerprint and availability tests are maintained in the MediaLibraryDatabase test project.

## Running the Tests

From the repository root:

```powershell
dotnet test KaraokeCodingChallenges.IncrementalScanning.Tests
```

To run the complete solution suite:

```powershell
dotnet test
```

## Challenge 040 Verification

At the completion of Challenge 040's implementation:

- IncrementalScanning tests: **24 passed**
- Full solution tests: **337 passed**
- Failed: **0**
- Skipped: **0**

All tests passed on .NET 10.
