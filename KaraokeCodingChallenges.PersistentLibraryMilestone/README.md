# Challenge 044 – Persistent Library Milestone

## Overview

The Persistent Library Milestone brings together the scanning, reconciliation, and persistence functionality developed throughout the previous challenges.

The milestone demonstrates a complete workflow for discovering media files, extracting metadata, persisting tracks to SQLite, detecting file changes, and maintaining MP3+CDG karaoke relationships.

It also verifies that existing database records are preserved when files are modified, removed, or restored.

## Features Demonstrated

- **Library discovery:** Identify MP3 and CDG files within a simulated karaoke library.
- **SQLite persistence:** Store discovered tracks and their metadata in a relational database.
- **Incremental scanning:** Use file fingerprints to identify new, modified, and unchanged files.
- **Metadata extraction:** Process new or modified MP3 files without repeatedly extracting metadata from unchanged files.
- **MP3+CDG pairing:** Associate matching CDG companion files with their MP3 tracks.
- **Transactional persistence:** Persist new or modified MP3 tracks and their CDG relationships within a single transaction.
- **Companion synchronisation:** Add and remove CDG relationships independently of MP3 metadata extraction.
- **Library reconciliation:** Identify missing tracks and update their availability without deleting their database records.
- **Track restoration:** Restore previously unavailable tracks while preserving their existing database IDs.
- **Database verification:** Reopen the SQLite database and verify the persisted library state.

## Projects Integrated

| Project | Responsibility |
|---|---|
| `KaraokeCodingChallenges.IncrementalScanning` | File fingerprinting, incremental scanning, reconciliation, and scan orchestration |
| `KaraokeCodingChallenges.MediaLibraryDatabase` | SQLite schema, repositories, and track persistence |
| `KaraokeCodingChallenges.KaraokeFilePairing` | Matching MP3 and CDG companion files |
| `KaraokeCodingChallenges.MetadataReader` | Metadata reader abstraction |

The demo reuses the existing application services rather than implementing separate scanning or persistence logic.

## Demo Library

The demo creates a temporary library containing simulated media files.

```text
karaoke-challenge-044-demo/
├── library.db
└── Karaoke/
    ├── Queen.mp3
    ├── Queen.cdg
    └── ABBA.mp3
```

Queen initially has a matching CDG companion, while ABBA begins as a standalone MP3.

The demo modifies this library throughout the execution to simulate changes that would occur in a real media collection.

All files are created within the system's temporary directory and are removed when the demo finishes.

## Demo Scenarios

The milestone executes seven scanning passes.

| Pass | Scenario | Expected Behaviour |
|---|---|---|
| 1 | Initial library scan | Discover and persist two MP3 tracks, including one MP3+CDG pair |
| 2 | Repeat scan | Skip both unchanged MP3 files |
| 3 | Modified MP3 | Update Queen's metadata and fingerprint while preserving its track ID |
| 4 | Add CDG companion | Associate a new CDG file with ABBA without rescanning the MP3 |
| 5 | Remove CDG companion | Remove Queen's CDG relationship while preserving its MP3 record |
| 6 | Missing MP3 reconciliation | Mark ABBA unavailable without deleting its database record |
| 7 | Restore missing MP3 | Restore ABBA's availability while preserving its track ID and CDG relationship |

### Incremental Scanning

File fingerprints contain the file size and last-modified timestamp.

These values are compared against the stored database records to determine whether metadata extraction is required.

In the completed demonstration, only four metadata reads are performed across seven scanning passes.

### Karaoke Companion Synchronisation

MP3 and CDG files are matched using their filenames and directory paths.

A CDG file is stored as a companion relationship rather than as an independent media track.

Adding or removing a CDG companion does not require metadata extraction when the corresponding MP3 remains unchanged.

### Reconciliation and Restoration

When an MP3 file disappears, its database record is marked unavailable rather than deleted.

The existing track ID and stored metadata are retained.

CDG relationships may also remain associated with unavailable tracks when the companion files still exist.

When the MP3 reappears, incremental scanning updates its existing record and restores its availability.

## Running the Demo

From the repository root:

```powershell
dotnet run --project KaraokeCodingChallenges.PersistentLibraryMilestone
```

The project targets .NET 10.

No external media files or existing music library are required.

## Expected Results

The completed demonstration should report the following scanning results:

| Pass | Added | Updated | Skipped | Missing |
|---|---:|---:|---:|---:|
| Initial scan | 2 | 0 | 0 | 0 |
| Repeat scan | 0 | 0 | 2 | 0 |
| Modified MP3 | 0 | 1 | 1 | 0 |
| Add CDG | 0 | 0 | 2 | 0 |
| Remove CDG | 0 | 0 | 2 | 0 |
| Missing MP3 | 0 | 0 | 1 | 1 |
| Restore MP3 | 0 | 1 | 1 | 0 |

### Final Database State

After the seventh pass, the SQLite database contains:

| Track | Available | CDG Companion |
|---|---|---|
| Queen | Yes | None |
| ABBA | Yes | ABBA.cdg |

The expected final summary is:

```text
Stored tracks: 2
Available tracks: 2
Total metadata reads: 4
```

Both records retain their original database IDs despite modifications and temporary file removal.

## Testing and Validation

The milestone builds upon the unit and integration tests introduced throughout the earlier challenges.

These tests cover:

- Incremental file fingerprint detection.
- New and modified track persistence.
- Unchanged-file skipping.
- Missing-file reconciliation and track restoration.
- MP3+CDG pairing and relationship synchronisation.
- Transactional track and CDG persistence.
- Transaction rollback when CDG persistence fails.

The milestone demo provides an additional end-to-end demonstration of these components working together.

To run the full solution test suite:

```powershell
dotnet test
```

## Limitations

- **Simulated media:** The demo uses placeholder MP3 and CDG files rather than real audio and karaoke graphics.
- **Simulated metadata:** A demo metadata reader supplies predictable metadata instead of reading embedded audio tags.
- **Temporary storage:** The demo resets its temporary directory on startup and removes it after execution.
- **No playback:** Audio playback and CDG graphics rendering are outside the scope of this milestone.
- **No graphical interface:** The workflow is demonstrated through console output.
- **Transaction scope:** MP3 and CDG persistence is transactional per matched pair; the entire multi-file library scan is not executed within one database transaction.

## Outcome

Challenge 044 establishes an integrated persistent library workflow capable of maintaining media metadata and karaoke companion relationships across repeated scans.

The milestone demonstrates that tracks can be added, modified, marked unavailable, and restored without unnecessarily duplicating records or repeating metadata extraction.

This provides a foundation for future development of the Windows music and karaoke application.