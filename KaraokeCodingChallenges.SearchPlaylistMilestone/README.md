# Challenge 033 - Search & Playlist Milestone

Integrate the library scanning, search, playlist, favourites, and playback features into a complete end-to-end workflow.

## Concepts Practised

- Application composition
- Dependency injection
- Cross-project integration
- Configuration-driven workflows
- Mapping between application models
- Integration testing
- Targeted refactoring
- Separation of responsibilities

## Implementation

Challenge 033 combines features developed across the previous challenges into a single application workflow.

The milestone:

- Loads and validates library source configuration.
- Scans enabled music, karaoke, and mixed library sources.
- Applies source-specific rules to identify karaoke files.
- Reads embedded metadata from supported audio files.
- Maps successful scan results into `MediaTrackModel` instances.
- Builds an in-memory `MediaLibraryCollection`.
- Filters tracks using reusable smart list rules.
- Sorts filtered tracks using the existing media library sorting service.
- Marks tracks as favourites.
- Creates playlists from library tracks.
- Exports playlists to M3U.
- Imports M3U playlists as path-based tracks.
- Resolves imported playlist tracks against the media library.
- Queues resolved tracks for playback.
- Records played tracks in playback history.

The console application acts as the composition root, creating the required services and coordinating the complete workflow.

## Media Track Mapping

Added `MediaTrackMapper` to convert successful `MediaScanResult` instances into `MediaTrackModel` instances.

The mapper:

- Ignores unsuccessful scan results.
- Ignores scan results without metadata.
- Uses the scanned metadata title when available.
- Falls back to the filename when the title is missing or whitespace.
- Uses an empty artist when artist metadata is unavailable.
- Uses a duration of `0` when duration metadata is unavailable.
- Applies the supplied karaoke classification without modifying the metadata model.

This provides a boundary between the scanning and media library models.

## Media Library Building

Added `MediaLibraryBuilder` to create a `MediaLibraryCollection` from scan results.

The builder:

- Uses `MediaTrackMapper` to create library tracks.
- Receives the mapper through constructor injection.
- Skips scan results that cannot be mapped.
- Matches karaoke paths case-insensitively.
- Sets `IsKaraoke` when a scanned file belongs to the karaoke file collection.

The mapper is created in the application composition root and supplied to the builder rather than being created internally.

## Playlist Library Resolution

M3U playlists intentionally contain path-based track information rather than full library metadata.

Added `PlaylistLibraryResolver` to reconnect imported playlist tracks to the in-memory media library.

For each imported track:

- The library is searched using its file path.
- Matching paths resolve to the existing rich `MediaTrackModel`.
- Unmatched paths remain as the imported track.
- Playlist order is preserved.

This keeps M3U parsing independent from the media library while allowing imported playlists to regain metadata when a matching library track exists.

## Configuration and Library Scanning

The milestone reuses the existing configuration and scanning pipeline.

Configured library sources can be:

- `Music`
- `Karaoke`
- `Mixed`

Disabled sources are skipped and unavailable source paths do not prevent other configured sources from being processed.

The demo configuration uses a small mixed library source for quick manual verification. Larger library sources can be enabled when full-library verification is required.

## Targeted Refactoring

`LibrarySourceRulesService` was extracted from the earlier Metadata Library Milestone into the dedicated `KaraokeCodingChallenges.LibrarySourceRules` project.

This allows source-specific karaoke classification to be reused by Challenge 033 without coupling the new milestone to the previous console application.

Favourite and playback history file path comparisons were also made case-insensitive so path-based application state behaves consistently across the integrated workflow.

## Project Integration

Challenge 033 composes functionality from the existing:

- Configuration
- Library Scanner
- Library Source Rules
- Metadata Reader and Scan Service
- Media Library
- Smart Lists
- Track Sorting
- Favourites
- Playlist and M3U Playlist File
- Playback Queue
- Playback History

The milestone coordinates these projects without moving their responsibilities into the milestone application.

## End-to-End Workflow

The completed milestone demonstrates the following application flow:

```text
Configuration
    ↓
Library Sources
    ↓
Library Scanning
    ↓
Source-Specific Rules
    ↓
Metadata Scanning
    ↓
Media Track Mapping
    ↓
Media Library
    ↓
Smart List Filtering
    ↓
Track Sorting
    ↓
Favourites
    ↓
Playlist Creation
    ↓
M3U Export / Import
    ↓
Library Resolution
    ↓
Playback Queue
    ↓
Playback History
```

## Manual Verification

The milestone was manually verified using a small mixed library containing music files and matching MP3/CDG karaoke pairs.

The configured test library produced:

```text
Files found: 7
Metadata files: 5
Karaoke files: 4
Total tracks: 5
```

The resulting media library contained three music tracks and two karaoke tracks.

The two karaoke tracks were successfully:

- Filtered using `MediaTrackRules.KaraokeOnly()`.
- Sorted by duration.
- Added to a playlist.
- Exported to M3U.
- Imported as path-based tracks.
- Resolved back to the rich library tracks.
- Added to the playback queue.
- Used to record playback history.

The milestone was also verified against the full music and karaoke library.

The full verification processed:

```text
Files found: 859
Metadata files: 714
Karaoke files: 294
Total tracks: 714
```

This confirmed that the integrated workflow also operates successfully against the complete library rather than only the smaller test dataset.

## Automated Verification

Challenge 033 is covered by focused mapper and builder tests together with integration tests spanning the major cross-project workflows.

The full solution passes:

```text
Test summary: total: 178, failed: 0, succeeded: 178, skipped: 0
```

Detailed Challenge 033 test coverage is documented in the milestone test project.

## Scope

Challenge 033 focuses on composing the existing in-memory application features into a complete workflow.

Whole-library persistence is intentionally not introduced in this milestone. Library persistence is planned for Challenge 034 so that persistence can be added after the integrated in-memory model has been verified.