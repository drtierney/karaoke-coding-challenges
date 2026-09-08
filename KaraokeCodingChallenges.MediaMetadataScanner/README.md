# Challenge 013 - Read Embedded Audio Metadata

## Objective

Read embedded metadata and technical audio properties from media files using a third-party metadata library.

## Requirements

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

## Concepts Practised

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

## What I Learned

- NuGet packages can add third-party functionality to a .NET project without having to implement complex file-format parsing manually.
- TagLibSharp can read both embedded tags and technical properties from supported media files.
- Descriptive metadata such as title, artist, album and genre is accessed through `Tag`.
- Technical properties such as duration, bitrate, sample rate and channel count are accessed through `Properties`.
- Embedded metadata is not guaranteed to contain every field, so missing values should remain nullable.
- Numeric tag values such as year and track number may use `0` when no value is present, which can be converted to `null` in the application model.
- `FileInfo` can provide file-system metadata such as file name, extension and size alongside embedded audio metadata.
- A `using` declaration ensures the TagLibSharp file resource is disposed when metadata reading is complete.
- Keeping raw missing values as `null` allows presentation code to decide how they should be displayed, such as using `N/A`.

## Sample Output

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
