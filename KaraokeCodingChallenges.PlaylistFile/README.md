# M3U Playlist Import & Export

Import and export M3U playlist files using the playlist model introduced in Challenge 026.

## Challenge 027 - M3U Playlist Import & Export

### Concepts Practised

- Text file reading and writing
- M3U playlist format
- Relative and absolute paths
- Path resolution
- File interoperability
- Separation of responsibilities

### Implementation

Created an `M3uPlaylistReader` that:

- Reads tracks from M3U playlist files.
- Creates a `MediaPlaylist` using the playlist filename as its name.
- Preserves absolute track paths.
- Resolves relative track paths against the playlist directory.
- Uses the track filename as the initial title.
- Ignores blank lines.
- Ignores M3U comments and directives beginning with `#`.
- Preserves playlist track order.
- Allows standard file exceptions such as `FileNotFoundException` to propagate.

Created an `M3uPlaylistWriter` that:

- Writes playlist track paths to an M3U file.
- Preserves track order.
- Supports empty playlists.
- Writes existing track paths by default.
- Optionally converts absolute track paths to paths relative to the playlist file.

Relative path export can be enabled using:

```csharp
writer.Write(
    playlistPath,
    playlist,
    useRelativePaths: true
);
```

### M3U Example

```text
..\Music\Track One.mp3
..\Music\Track Two.mp3
```

### Verification

The implementation was verified using automated tests covering:

- Absolute path import.
- Relative path import and resolution.
- Blank lines.
- M3U comments and directives.
- Track ordering.
- Missing playlist files.
- Track path export.
- Writer ordering.
- Empty playlist export.
- Relative path export.
- M3U write/read round trips.

The generated M3U playlist was also manually tested using real audio files.

The playlist loaded successfully in both VLC Media Player and Winamp, confirming interoperability with external media players.