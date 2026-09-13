# Challenge 019 - Exception Handling & Error Reporting

Handle recoverable media scan failures without allowing a single problem file to stop the wider scan.

## Concepts Practised

- Exception handling with `try` and `catch`
- Specific exception types
- Recoverable failures
- Structured error reporting
- Warning generation
- Batch processing
- Iterators with `yield return`
- Helper methods
- Separation of scanning and error-handling responsibilities

## Implementation

The `MediaScanService` provides a higher-level scanning layer around `MediaMetadataScannerService`.

`ScanFile` attempts to resolve metadata for a single media file and returns a `MediaMediaScanResult` describing the outcome.

Successful scans return:

- The original file path.
- Resolved metadata.
- `IsSuccess` set to `true`.
- Warnings for non-fatal metadata issues.

Failed scans return:

- The original file path.
- No metadata.
- `IsSuccess` set to `false`.
- An error message describing the failure.

The service handles expected recoverable exceptions including:

- `FileNotFoundException` when a file no longer exists.
- `InvalidDataException` when metadata cannot be read.
- `UnauthorizedAccessException` when access to a file is denied.

A final `Exception` handler converts unexpected file-level failures into failed scan results so a single file does not terminate a wider batch scan.

Repeated failed-result construction is handled through a private `CreateFailedResult` helper method.

## Warnings

A scan can still succeed while containing incomplete metadata.

Warnings are currently generated when:

- Artist metadata cannot be resolved.
- Title metadata cannot be resolved.

Warnings do not change `IsSuccess` to `false` because the file was still successfully scanned.

## Batch Scanning

`ScanFiles` accepts an `IEnumerable<string>` of file paths and uses `yield return` to return each `MediaMediaScanResult` individually.

Each file is processed through `ScanFile`, allowing failed files to produce structured error results while later files continue to be scanned.

This means a sequence containing valid and invalid files can complete without one failed file terminating the whole operation.

## Sample Output

```text
File: D:\KaraokeTest\01 - UB40 - Can't Help Falling In Love.mp3
Success: True

File: D:\KaraokeTest\1975 - The Sound.mp3
Success: True

File: D:\KaraokeTest\MissingFile.mp3
Success: False
Error: File not found

File: D:\KaraokeTest\Foxes Body Talk.mp3
Success: True
Warning: Artist metadata is missing.

File: D:\KaraokeTest\Logic ft Alessia Cara & Khalid - 1-800-273-8255.mp3
Success: True
```

The sample demonstrates successful scans, a recoverable file error, a non-fatal metadata warning, and continued processing after a failed file.
