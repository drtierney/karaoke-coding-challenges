# Challenge 040 - Incremental Library Scanning Tests

Contains xUnit tests for Challenge 040, covering filesystem fingerprints, change detection, SQLite integration, incremental scan processing, and batch statistics.

The tests verify that new and modified files are processed while unchanged files are skipped without unnecessary metadata extraction.

## Test Coverage

The project contains **24 tests** covering the following areas.

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
- Continuing to process subsequent files when an earlier file is missing.
- Reporting added, updated, skipped, and failed counts correctly.

## Test Infrastructure

The integration tests use:

- **xUnit** for test execution and assertions.
- **Temporary SQLite databases** for persistence verification.
- **Temporary filesystem files and directories** for fingerprint generation and modification.
- **Controlled `IMetadataReader` implementations** to simulate successful and unsuccessful metadata extraction.

The tests exercise the real repository and persistence implementations rather than relying on database mocks.

Temporary resources are cleaned up after execution.

## SQLite Verification

Tests involving persisted tracks confirm that:

- New tracks receive stored fingerprints.
- Modified tracks retain their existing SQLite identifiers.
- Updated fingerprints reflect filesystem changes.
- Failed metadata extraction does not overwrite existing fingerprint values.
- Unchanged files do not require additional metadata reads.

Database schema migration and repository-specific fingerprint persistence tests are maintained in their respective project test suites.

## Running the Tests

From the repository root:

```powershell
dotnet test .\KaraokeCodingChallenges.IncrementalScanning.Tests\
```

To run the complete solution test suite:

```powershell
dotnet test
```

## Verification

At the completion of Challenge 040's implementation:

- IncrementalScanning tests: **24 passed**
- Full solution tests: **337 passed**
- Failed: **0**
- Skipped: **0**

All tests passed on .NET 10.