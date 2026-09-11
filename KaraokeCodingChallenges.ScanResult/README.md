# Challenge 018 - Scan Result Model

Create a structured model for representing the outcome of scanning a media file.

## Concepts Practised

- Records
- Required properties
- Nullable reference types
- Read-only collections
- Collection expressions
- Structured result models
- Representing success, warnings, and errors
- Separating data from presentation
- Formatting console output

## Implementation

The `ScanResult` record represents the result of scanning a single media file.

Each result contains:

- The path of the scanned file.
- Resolved metadata when the scan succeeds.
- An `IsSuccess` flag indicating whether the scan completed successfully.
- A collection of warnings for non-fatal issues.
- A collection of errors for failed scans.

Metadata is nullable so failed scans can return a result without creating a partially populated metadata object.

Warnings and errors use `IReadOnlyCollection<string>` and default to empty collections so callers can safely inspect or iterate over them without null checks.

## Sample Scenarios

The application demonstrates three scan outcomes:

- A successful scan with complete metadata.
- A successful scan with missing metadata and warnings.
- A failed scan with no metadata and an error message.

A helper method formats each result for console output while keeping presentation logic separate from the `ScanResult` record.

```text
File: D:\Music\Queen - Bohemian Rhapsody.mp3
Success: True
Title: Bohemian Rhapsody
Artist: Queen
Album: A Night at the Opera

File: D:\Music\Unknown Artist - Mystery Song.mp3
Success: True
Title: Mystery Song
Artist: Unknown Artist
Album:
Warnings:
- Album metadata was not available.
- Track number could not be determined.

File: D:\Music\BrokenFile.mp3
Success: False
Errors:
- The file could not be read.
```