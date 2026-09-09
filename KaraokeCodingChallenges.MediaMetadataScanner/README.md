# Media Metadata Scanner

Build a reusable media metadata scanner that reads embedded audio metadata and falls back to filename parsing when selected tags are missing.

## Challenge 013 - Read Embedded Audio Metadata

### Objective

Read embedded metadata and technical audio properties from media files using a third-party metadata library.

### Requirements

- Add the TagLibSharp NuGet package.
- Validate that the requested media file exists.
- Read embedded metadata from an audio file.
- Populate a `MediaMetadata` record using file information and embedded tags.
- Read common metadata including title, artist, album, genre, year and track number.
- Read technical metadata including duration, bitrate, sample rate and channel count.
- Use `FileInfo` to populate file name, extension and file size.
- Represent missing metadata values as `null`.
- Dispose of the TagLibSharp file resource correctly.
- Display the resulting metadata in a readable format.

### Concepts Practised

- NuGet packages
- Third-party APIs
- TagLibSharp
- `FileInfo`
- Embedded audio metadata
- Nullable values
- Resource disposal
- `using` declarations
- Type conversion
- Console output formatting

### What I Learned

- NuGet packages can add third-party functionality to a .NET project without having to implement complex file-format parsing manually.
- TagLibSharp can read both embedded tags and technical properties from supported media files.
- Descriptive metadata such as title, artist, album and genre is accessed through `Tag`.
- Technical properties such as duration, bitrate, sample rate and channel count are accessed through `Properties`.
- Embedded metadata is not guaranteed to contain every field, so missing values should remain nullable.
- Numeric tag values such as year and track number may use `0` when no value is present, which can be converted to `null` in the application model.
- `FileInfo` can provide file-system metadata such as file name, extension and size alongside embedded audio metadata.
- A `using` declaration ensures the TagLibSharp file resource is disposed when metadata reading is complete.
- Keeping raw missing values as `null` allows presentation code to decide how they should be displayed, such as using `N/A`.

### Sample Output

```text
File Path:        D:\KaraokeTest\01 - UB40 - Can't Help Falling In Love.mp3
File Name:        01 - UB40 - Can't Help Falling In Love.mp3
Extension:        .mp3
File Size:        3.32 MB
Artist:           UB40
Title:            Can't Help Falling In Love
Album:            N/A
Genre:            N/A
Year:             N/A
Track Number:     1
Duration Seconds: 217
Bitrate:          128
Sample Rate:      44100
Channels:         2
```

### Notes

- The implementation was tested using MP3 files.
- TagLibSharp supports additional media formats, which can be tested in future as suitable sample files become available.
- Filename-based metadata fallback is introduced in Challenge 014.

## Challenge 014 - Filename Metadata Fallback

### Objective

Use filename parsing to provide artist, title and track number values when embedded metadata is missing.

### Requirements

- Build on the embedded metadata reader from Challenge 013.
- Parse supported filename formats.
- Support:
  - `Artist - Title`
  - `TrackNumber - Artist - Title`
- Use filename-derived values only when the corresponding embedded metadata is missing.
- Preserve valid embedded metadata when it already exists.
- Use `int.TryParse` for optional track number parsing.
- Fall back to the filename without extension as the title when no recognised pattern is found.
- Continue returning a populated `MediaMetadata` record.
- Preserve unrelated embedded metadata such as album, genre and year.

### Concepts Practised

- String parsing
- `Path.GetFileNameWithoutExtension`
- Tuples
- Tuple deconstruction
- `int.TryParse`
- Nullable values
- `string.IsNullOrWhiteSpace`
- Fallback logic
- Preserving existing metadata

### What I Learned

- Embedded metadata cannot always be relied upon to contain artist, title or track number values.
- Filename parsing can provide useful fallback metadata when tags are missing.
- Fallback values should only be used when embedded metadata is unavailable.
- `string.IsNullOrWhiteSpace` handles both `null` and empty tag values.
- Tuple return values provide a simple way to return several parsed filename values together.
- `int.TryParse` allows a leading filename segment to be treated as a track number only when it is numeric.
- A filename without a recognised separator can still provide a reasonable fallback title.
- Filename fallback can be applied selectively without overwriting unrelated embedded metadata.

### Supported Filename Formats

```text
Artist - Title.mp3
```

Example:

```text
Logic ft Alessia Cara & Khalid - 1-800-273-8255.mp3
```

And:

```text
TrackNumber - Artist - Title.mp3
```

Example:

```text
01 - UB40 - Can't Help Falling In Love.mp3
```

### Sample Output

A file with a missing embedded track number:

```text
D:\KaraokeTest\01 - UB40 - Can't Help Falling In Love.mp3
```

produces:

```text
File Path:        D:\KaraokeTest\01 - UB40 - Can't Help Falling In Love.mp3
File Name:        01 - UB40 - Can't Help Falling In Love.mp3
Extension:        .mp3
File Size:        3.32 MB
Artist:           UB40
Title:            Can't Help Falling In Love
Album:            N/A
Genre:            N/A
Year:             N/A
Track Number:     1
Duration Seconds: 217
Bitrate:          128
Sample Rate:      44100
Channels:         2
```

The track number is taken from the filename because the embedded value is missing.

A file with missing artist and title metadata:

```text
D:\KaraokeTest\Logic ft Alessia Cara & Khalid - 1-800-273-8255.mp3
```

falls back to:

```text
Artist:           Logic ft Alessia Cara & Khalid
Title:            1-800-273-8255
Genre:            Hip-Hop
Year:             2017
```

The embedded genre and year remain unchanged.

A filename without a recognised separator:

```text
D:\KaraokeTest\Foxes Body Talk.mp3
```

falls back to:

```text
Artist:           N/A
Title:            Foxes Body Talk
Year:             2015
```

### Notes

- Filename metadata is currently used only for artist, title and track number.
- Embedded metadata takes priority when it is available.
- Missing filename-derived values remain `null` in the underlying record and are displayed as `N/A`.
- More complete metadata precedence and resolution rules are intentionally deferred to Challenge 015.

## Challenge 016 Refactor

Challenge 016 introduced an `IMetadataReader` abstraction and moved the TagLibSharp-specific metadata-reading logic into the separate `KaraokeCodingChallenges.MetadataReader` project.

The scanner now receives an `IMetadataReader` rather than accessing TagLibSharp directly.

The scanner remains responsible for:

- File validation
- File name, extension, and file size
- Filename parsing
- Filename fallback values
- Combining embedded metadata with file-system information
- Creating the final `MediaMetadata` record

Embedded metadata reading is delegated through:

```text
MediaMetadataScanner
    -> IMetadataReader
        -> TagLibMetadataReader
            -> TagLibSharp
```

This separates the scanner from the third-party metadata library while preserving the behaviour introduced in Challenges 013 and 014.