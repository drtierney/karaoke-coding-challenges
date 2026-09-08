# Challenge 012 - Media Metadata Model

## Objective

Create a reusable metadata model for representing information read from music and karaoke files.

## Requirements

- Create a `MediaMetadata` record.
- Require a file path for every metadata instance.
- Represent common embedded metadata including title, artist, album, genre, year and track number.
- Represent technical metadata including duration, bitrate, sample rate and channel count.
- Allow metadata fields to be nullable when values are missing or cannot be detected.
- Use `init` properties so metadata values cannot be changed after initialization.
- Demonstrate record value equality.
- Demonstrate creating a modified copy using a `with` expression.
- Demonstrate handling missing metadata values.
- Use the default record string representation to display a metadata instance.

## Concepts Practised

- Records
- Data modelling
- Nullable reference types
- Nullable value types
- `required` properties
- `init` properties
- Immutability
- Value equality
- `with` expressions
- Null-coalescing operators
- Record-generated `ToString()`

## What I Learned

- Records are useful for representing data where the stored values are more important than the identity of the object.
- Records use value-based equality by default, so two records with matching values compare as equal.
- Changing a single value causes two records to compare as unequal.
- `init` properties allow values to be assigned during object creation while preventing normal modification afterwards.
- A `with` expression can create a new record based on an existing record while changing selected values without modifying the original.
- `required` can ensure important properties such as `FilePath` are supplied when creating an object.
- Metadata such as title, artist, album, year and duration may be unavailable even for valid media files, so nullable properties provide a clear way to represent unknown values.
- Using `null` for unavailable metadata keeps the raw model separate from display values such as `"Unknown"`.
- Records automatically provide a useful `ToString()` representation containing their property values.

## Sample Output

```text
Record equality:
True

Changed copy:
Original genre: Rock
Corrected genre: Classic Rock
Records equal after change: False

Nullable metadata:
Title: Unknown
Duration: Unknown
Bitrate: Unknown

Record output:
MediaMetadata { FilePath = D:\Music\Queen - Bohemian Rhapsody.mp3, Title = Bohemian Rhapsody, Artist = Queen, Album = A Night at the Opera, Genre = Rock, Year = 1975, TrackNumber = 11, DurationSeconds = 354, Bitrate = 320, SampleRate = 44100, Channels = 2 }
```
