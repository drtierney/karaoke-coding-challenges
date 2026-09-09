# Challenge 015 - Metadata Resolution

Resolve metadata from multiple sources using clear precedence and fallback rules.

## Concepts Practised

- Fallback rules
- Null handling
- Metadata precedence
- `string.IsNullOrWhiteSpace`
- Nullable value types
- Null-coalescing operator (`??`)
- Pure functions
- Records

## Metadata Precedence

Metadata is resolved using the following priority:

1. Embedded metadata
2. Filename-derived metadata
3. `null` when neither source contains a usable value

For string values, `null`, empty strings, and whitespace-only strings are treated as missing values.

## Implementation

`MetadataResolutionService` combines embedded and filename-derived metadata into a new `MediaMetadata` record.

Reusable helper methods handle resolution for:

- String metadata using `string.IsNullOrWhiteSpace`
- Nullable integer metadata using the null-coalescing operator

The resolver covers metadata including:

- Title
- Artist
- Album
- Genre
- Year
- Track number
- Duration
- Bitrate
- Sample rate
- Channels

File information such as the file path is carried forward from the scanned metadata rather than resolved using metadata precedence.

## Example

An embedded title containing only whitespace falls back to the title parsed from the filename, while valid embedded values such as artist, album, and technical metadata are retained.

This produces a single resolved `MediaMetadata` record containing the best available values from both sources.

```
Embedded Metadata
-----------------
File Path:        D:\Music\01 - Queen - Bohemian Rhapsody.mp3
File Name:        N/A
Extension:        N/A
File Size:        N/A
Artist:           Queen
Title:
Album:            A Night at the Opera
Genre:            Rock
Year:             1975
Track Number:     N/A
Duration Seconds: 354
Bitrate:          320
Sample Rate:      44100
Channels:         2

Filename Metadata
-----------------
File Path:        D:\Music\01 - Queen - Bohemian Rhapsody.mp3
File Name:        N/A
Extension:        N/A
File Size:        N/A
Artist:           Queen
Title:            Bohemian Rhapsody
Album:            N/A
Genre:            N/A
Year:             N/A
Track Number:     1
Duration Seconds: N/A
Bitrate:          N/A
Sample Rate:      N/A
Channels:         N/A

Resolved Metadata
-----------------
File Path:        D:\Music\01 - Queen - Bohemian Rhapsody.mp3
File Name:        N/A
Extension:        N/A
File Size:        N/A
Artist:           Queen
Title:            Bohemian Rhapsody
Album:            A Night at the Opera
Genre:            Rock
Year:             1975
Track Number:     1
Duration Seconds: 354
Bitrate:          320
Sample Rate:      44100
Channels:         2
```
