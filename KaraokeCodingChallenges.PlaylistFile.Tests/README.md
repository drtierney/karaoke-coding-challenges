# M3U Playlist Tests

Automated tests for Challenge 027 - M3U Playlist Import & Export.

## Test Coverage

### M3uPlaylistReaderTests

Tests verify that the reader:

- Imports absolute track paths.
- Resolves relative paths against the playlist directory.
- Ignores blank lines.
- Ignores comments and M3U directives.
- Preserves track order.
- Throws `FileNotFoundException` when the playlist file does not exist.

### M3uPlaylistWriterTests

Tests verify that the writer:

- Writes playlist track paths.
- Preserves track order.
- Creates an empty M3U file for an empty playlist.
- Can export paths relative to the playlist location.

### M3uPlaylistRoundTripTests

A round-trip test writes a playlist using `M3uPlaylistWriter` and reads it back using `M3uPlaylistReader`.

This verifies that track paths and ordering are preserved across M3U export and import.

## Result

11 tests passed.