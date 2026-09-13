# Challenge 020 - Scan Statistics

Produce useful summary statistics from media scan results, metadata quality, and karaoke file pairing results.

## Concepts Practised

- LINQ
- `Count`
- `Sum`
- `Where`
- `Select`
- `GroupBy`
- `ToDictionary`
- Aggregates
- Percentage calculations
- Collection materialisation with `ToList`
- Read-only dictionaries
- Summary models
- Reusing results from existing services

## Implementation

The `ScanStatisticsService` generates a `ScanStatistics` summary from a collection of `ScanResult` objects and existing karaoke file pairing results.

The scan results are materialised into a list before statistics are calculated so the input does not need to be repeatedly enumerated.

## Scan Summary

The service calculates:

- Total number of scans.
- Successful scans.
- Failed scans.
- Total warnings.
- Success percentage.
- Failure percentage.

Percentages safely return zero when there are no scan results.

## Files By Extension

Scan results are grouped by their file extension using `GroupBy`.

Extensions are normalised to lowercase using `ToLowerInvariant` before each group is converted into a dictionary containing the extension and number of matching files.

This provides a summary such as:

```text
.flac: 1
.mp3: 2
.opus: 1
```

## Metadata Quality

Metadata quality statistics are calculated from successful scans containing metadata.

The service counts records missing:

- Artist.
- Title.
- Album.
- Genre.
- Track number.

A metadata record is considered complete when all five fields are populated.

The service also calculates the percentage of successful metadata records that contain all five fields.

## Karaoke Pairing Statistics

Challenge 020 reuses the karaoke file pairing results introduced in Challenge 003 rather than implementing pairing logic again.

The statistics include:

- Matched karaoke pairs.
- Unmatched MP3 files.
- Unmatched CDG files.

This keeps file pairing and statistical reporting as separate responsibilities.

## Sample Output

```text
Scan Summary
Total Scans: 4
Successful Scans: 3
Failed Scans: 1
Success Percentage: 75.00%
Failure Percentage: 25.00%
Total Warnings: 4

Files By Extension
.flac: 1
.mp3: 2
.opus: 1

Metadata Quality
Missing Artist: 1
Missing Title: 0
Missing Album: 1
Missing Genre: 1
Missing Track Number: 1
Complete Metadata Percentage: 33.33%

Karaoke Pairing
Matched Pairs: 2
Unmatched MP3 Files: 1
Unmatched CDG Files: 1
```

The sample demonstrates aggregate scan statistics, grouped file-extension counts, metadata completeness reporting, and karaoke pairing statistics.
