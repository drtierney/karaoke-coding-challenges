# Media Library Database Tests

Tests the SQLite schema introduced in **Challenge 036 - Media Library Database Schema**.

The test suite contains **21 tests** covering schema creation, constraints, relationships, referential integrity, delete behaviour, and indexes.

## Schema Initialization

Tests verify that initialization:

- Creates the expected database tables
- Creates the expected `LibrarySources` columns
- Creates the planned explicit indexes

The expected application tables are:

- `LibrarySources`
- `Tracks`
- `KaraokeFiles`
- `Playlists`
- `PlaylistTracks`
- `PlaybackHistory`

## Library Source Constraints

Tests verify that:

- Duplicate library-source paths are rejected regardless of casing
- Invalid library-source type values are rejected
- Invalid enabled values are rejected

## Track Constraints

Tests verify that:

- Negative durations are rejected
- Invalid karaoke flag values are rejected
- Duplicate file paths are rejected regardless of casing
- Tracks cannot reference an unknown library source

## Karaoke File Relationships

Tests verify that:

- A track can have one karaoke companion file
- A second karaoke file for the same track is rejected
- Deleting a track automatically deletes its karaoke-file row

This verifies the one-to-zero-or-one relationship between `Tracks` and `KaraokeFiles`.

## Playlist Relationships

Tests verify that:

- The same track can appear multiple times in a playlist
- Two playlist entries cannot use the same position
- Negative playlist positions are rejected
- Deleting a playlist automatically deletes its entries
- Deleting a track referenced by a playlist is rejected
- Blank playlist GUIDs are rejected

Duplicate track IDs are intentionally allowed because playlist ordering and repeated tracks are valid application behaviour.

## Playback History

Tests verify that:

- Multiple playback-history entries can reference the same track
- Playback history cannot reference an unknown track
- Deleting a track automatically removes its playback history

Playback counts remain derived values rather than stored database columns.

## Indexes

The tests verify creation of:

- `IX_Tracks_SourceId`
- `IX_PlaylistTracks_PlaylistId`
- `IX_PlaylistTracks_TrackId`
- `IX_PlaybackHistory_TrackId`
- `IX_PlaybackHistory_PlayedAt`

SQLite-generated `sqlite_autoindex_*` indexes are excluded from the explicit-index test.

## Temporary Databases

Each test uses a separate SQLite database created in the operating system temporary directory.

```text
<guid>.db
```

`MediaLibraryDatabaseTestContext` handles:

- Temporary database path creation
- Database initialization
- Connection creation
- Foreign-key enforcement
- Common test-data insertion
- Database cleanup

Connection pooling is disabled for the test databases so each temporary file can be deleted reliably after the test completes.

Using a separate database for each test keeps the tests isolated and prevents data from one test affecting another.

## Running the Tests

Run the database-schema tests with:

```text
dotnet test KaraokeCodingChallenges.MediaLibraryDatabase.Tests
```

Or run the complete solution test suite with:

```text
dotnet test
```

## Test Results

```text
Test summary: total: 21, failed: 0, succeeded: 21, skipped: 0
```
