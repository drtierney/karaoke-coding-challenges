# C# Karaoke Challenges

[![.NET Build and Test](https://github.com/drtierney/karaoke-coding-challenges/actions/workflows/dotnet.yaml/badge.svg?branch=main)](https://github.com/drtierney/karaoke-coding-challenges/actions/workflows/dotnet.yaml)

A collection of C# coding challenges designed to improve my C# skills while exploring concepts that could eventually contribute towards a Windows music and karaoke application.

## About This Repository

This repository documents my progression through a series of practical C# challenges. Rather than building a complete application immediately, each challenge focuses on a particular language feature, programming concept, or application component.

The challenges build on one another over time, starting with core C# concepts and gradually introducing areas such as:

- Collections and LINQ
- File and path handling
- Metadata processing
- Configuration and validation
- Exception handling
- Unit and integration testing with xUnit
- JSON serialization and persistence
- Playlists and playback state
- Search, filtering, and Smart Lists
- Dependency injection and application composition

Milestone challenges periodically bring several earlier components together into larger workflows. The longer-term goal is to use the knowledge gained from these challenges when developing a separate Windows music and karaoke application.

## Current Progress

The challenge series has completed **Challenge 034 - JSON Library Persistence**, introducing versioned JSON persistence for the in-memory media library using dedicated DTOs, mapping, validation, and save/load workflows.

The persistence and storage phase now continues with SQLite fundamentals before progressing toward database-backed storage, incremental scanning, library reconciliation, and the **Persistent Library Milestone**.

The challenge table below shows the completed challenges and the currently planned roadmap. More detailed roadmap information, including concepts, dependencies, expected outcomes, and notes, is available in the [roadmap](roadmap/README.md).

## Technologies and Tools

- C#
- .NET 10
- xUnit
- System.Text.Json
- TagLibSharp
- Visual Studio

## Building and Testing

Clone the repository and build the solution from the repository root:

```text
dotnet build
```

Run the complete test suite with:

```text
dotnet test
```

Individual challenge READMEs contain additional information about their implementation, concepts practised, tests, and example output where applicable.

## Challenges

| #   | Challenge | Status |
|-----|-----------|--------|
| 001 | [MediaTrack Model](KaraokeCodingChallenges.MediaTrack/README.md) | ☑ Completed |
| 002 | [Library Scanner](KaraokeCodingChallenges.LibraryScanner/README.md) | ☑ Completed |
| 003 | [Karaoke File Pairing](KaraokeCodingChallenges.KaraokeFilePairing/README.md) | ☑ Completed |
| 004 | [Media Type Detection](KaraokeCodingChallenges.MediaTypeDetection/README.md) | ☑ Completed |
| 005 | [File Validation](KaraokeCodingChallenges.FileValidation/README.md) | ☑ Completed |
| 006 | [Media Library](KaraokeCodingChallenges.MediaLibrary/README.md) | ☑ Completed |
| 007 | [Media Library Queries](KaraokeCodingChallenges.MediaLibraryQueries/README.md) | ☑ Completed |
| 008 | [Media Library Filtering](KaraokeCodingChallenges.MediaLibraryFiltering/README.md) | ☑ Completed |
| 009 | [Track Sorting](KaraokeCodingChallenges.TrackSorting/README.md) | ☑ Completed |
| 010 | [Duplicate Detection](KaraokeCodingChallenges.DuplicateDetection/README.md) | ☑ Completed |
| 011 | [Media Library Processor](KaraokeCodingChallenges.MediaLibraryProcessor/README.md) | ☑ Completed |
| 012 | [Media Metadata Model](KaraokeCodingChallenges.MediaMetadata/README.md) | ☑ Completed |
| 013 | [Read Embedded Audio Metadata](KaraokeCodingChallenges.MediaMetadataScanner/README.md#challenge-013---read-embedded-audio-metadata) | ☑ Completed |
| 014 | [Filename Metadata Fallback](KaraokeCodingChallenges.MediaMetadataScanner/README.md#challenge-014---filename-metadata-fallback) | ☑ Completed |
| 015 | [Metadata Resolution](KaraokeCodingChallenges.MetadataResolution/README.md) | ☑ Completed |
| 016 | [Metadata Reader Interface](KaraokeCodingChallenges.MetadataReader/README.md) | ☑ Completed |
| 017 | [Metadata Resolution Tests](KaraokeCodingChallenges.Metadata.Tests/README.md) | ☑ Completed |
| 018 | [Scan Result Model](KaraokeCodingChallenges.ScanResult/README.md) | ☑ Completed |
| 019 | [Exception Handling & Error Reporting](KaraokeCodingChallenges.MediaScanService/README.md) | ☑ Completed |
| 020 | [Scan Statistics](KaraokeCodingChallenges.ScanStatistics/README.md) | ☑ Completed |
| 021 | [Metadata Library Milestone](KaraokeCodingChallenges.MetadataLibraryMilestone/README.md) | ☑ Completed |
| 022 | [Configuration File](KaraokeCodingChallenges.Configuration/README.md#challenge-022---configuration-file) | ☑ Completed |
| 023 | [Configuration Validation](KaraokeCodingChallenges.Configuration/README.md#challenge-023---configuration-validation) | ☑ Completed |
| 024 | [Multiple Library Sources](KaraokeCodingChallenges.Configuration/README.md#challenge-024---multiple-library-sources) | ☑ Completed |
| 025 | [Source-Specific Scan Rules](KaraokeCodingChallenges.MetadataLibraryMilestone/README.md#source-specific-scan-rules) | ☑ Completed |
| 026 | [Playlist Model](KaraokeCodingChallenges.Playlist/README.md) | ☑ Completed |
| 027 | [M3U Playlist Import & Export](KaraokeCodingChallenges.PlaylistFile/README.md) | ☑ Completed |
| 028 | [Playback Queue Manager](KaraokeCodingChallenges.Playback/README.md) | ☑ Completed |
| 029 | [Playback History & Play Counts](KaraokeCodingChallenges.PlaybackHistory/README.md) | ☑ Completed |
| 030 | [Favourites](KaraokeCodingChallenges.Favourites/README.md) | ☑ Completed |
| 031 | [Smart Lists](KaraokeCodingChallenges.SmartLists/README.md) | ☑ Completed |
| 032 | [Advanced Search & Filter Rules](KaraokeCodingChallenges.SmartLists/README.md#challenge-032---advanced-search--filter-rules) | ☑ Completed |
| 033 | [Search & Playlist Milestone](KaraokeCodingChallenges.SearchPlaylistMilestone/README.md) | ☑ Completed |
| 034 | [JSON Library Persistence](KaraokeCodingChallenges.LibraryPersistence/README.md) | ☑ Completed |
| 035 | SQLite Fundamentals | ☐ Planned |
| 036 | Media Library Database Schema | ☐ Planned |
| 037 | Database Data Access Layer | ☐ Planned |
| 038 | Persist Scanned Tracks | ☐ Planned |
| 039 | Database-Backed Search | ☐ Planned |
| 040 | Incremental Library Scanning | ☐ Planned |
| 041 | Library Reconciliation | ☐ Planned |
| 042 | File System Watcher Library Updates | ☐ Planned |
| 043 | Database & Persistence Testing | ☐ Planned |
| 044 | Persistent Library Milestone | ☐ Planned |
| 045 | Async Library Scanner | ☐ Planned |
| 046 | Scan Progress Reporting | ☐ Planned |
| 047 | Cancellation Support | ☐ Planned |
| 048 | Application Events & Delegates | ☐ Planned |
| 049 | Service Interfaces | ☐ Planned |
| 050 | Dependency Injection | ☐ Planned |
| 051 | Performance Profiling | ☐ Planned |
| 052 | Broader Unit & Integration Testing | ☐ Planned |
| 053 | Application Architecture Milestone | ☐ Planned |
| 054 | Playback State Model | ☐ Planned |
| 055 | Player Commands | ☐ Planned |
| 056 | Karaoke Track Model | ☐ Planned |
| 057 | Karaoke Playback Coordination | ☐ Planned |
| 058 | MVVM & ViewModels | ☐ Planned |
| 059 | Desktop Library UI | ☐ Planned |
| 060 | Now Playing & Karaoke UI | ☐ Planned |
| 061 | Mini Karaoke Player Milestone | ☐ Planned |

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
