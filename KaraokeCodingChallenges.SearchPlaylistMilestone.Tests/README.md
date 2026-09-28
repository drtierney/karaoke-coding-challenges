# Challenge 033 - Search & Playlist Milestone Tests

Unit and integration tests for media track mapping, media library building, playlist resolution, and the cross-project workflows introduced by the Search & Playlist Milestone.

## Test Coverage

### Media Track Mapping

- Successful scan results are mapped to `MediaTrackModel`.
- Title, artist, file path, and duration metadata are mapped to the track.
- Tracks default to non-karaoke when no karaoke classification is supplied.
- `null`, empty, or whitespace-only titles fall back to the filename.
- Missing artist metadata falls back to an empty string.
- Missing duration metadata falls back to `0`.
- Unsuccessful scan results return `null`.
- Scan results without metadata return `null`.
- Karaoke classification can be applied to the mapped track.

### Media Library Building

- Multiple successful scan results are added to the media library.
- Tracks whose paths are included in the karaoke file collection are classified as karaoke.
- Karaoke file path matching is case-insensitive.
- Unsuccessful scan results are excluded from the media library.

### Source Rule Integration

- Mixed source rules identify matching MP3/CDG karaoke files.
- Karaoke MP3 files are classified as karaoke when the media library is built.
- Standalone music MP3 files remain non-karaoke.
- CDG files contribute to karaoke classification without becoming media library tracks.

### Search, Filter, and Sort Integration

- A built media library can be filtered using `SmartList<MediaTrackModel>`.
- Reusable `MediaTrackRules` can filter tracks by artist.
- Filtered tracks can be sorted using `MediaLibrarySortService`.
- Duration sorting returns the expected track order.

### Playlist Integration

- Filtered library tracks can be added to a `MediaPlaylist`.
- Playlist track order is preserved.
- Playlist entries retain the original library track instances.
- Track metadata remains available after adding tracks to a playlist.

### Playlist Library Resolution

- Imported playlist tracks are resolved against the media library by file path.
- Matching playlist tracks resolve to the existing rich library track instance.
- Resolved tracks regain library metadata.
- Tracks without a matching library entry remain as the original imported track.

### M3U Integration

- An M3U file can be imported and resolved against the media library.
- The imported playlist name is preserved.
- Matching path-based tracks resolve to existing rich library tracks.
- Unmatched tracks remain available using their imported path and filename-derived title.
- Resolved and unresolved tracks preserve their original playlist order.

### Favourites Integration

- Tracks from a built media library can be identified as favourites.
- Favourite matching works when file path casing differs from the library track.

### Playback Queue Integration

- A filtered library track can be added to the playback queue.
- The queued track retains the original library track instance.
- Track metadata remains available when the queued track is retrieved.

### Playback History Integration

- A library track can be queued, dequeued, and recorded in playback history.
- Recorded play counts can be retrieved using the library track path.
- Last-played timestamps are retained.
- Recent history contains the recorded track path and timestamp.
- Playback history lookups work when file path casing differs.

## Test Results

Challenge 033 tests pass as part of the full solution test suite.

```text
Test summary: total: 178, failed: 0, succeeded: 178, skipped: 0
```