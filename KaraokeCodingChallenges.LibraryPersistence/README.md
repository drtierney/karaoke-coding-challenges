# Challenge 034 - JSON Library Persistence

Adds JSON-based persistence for the media library, allowing a `MediaLibraryCollection` to be saved to disk and restored later without rescanning the original media sources.

## Concepts Practised

- JSON serialization and deserialization
- Data transfer objects (DTOs)
- File I/O
- Persistence boundaries
- Schema versioning
- Mapping between persistence and domain models
- Dependency injection
- Nullable reference handling
- Input validation
- Exception handling

## Implementation

The persistence layer uses dedicated DTOs rather than serializing the domain models directly.

### `MediaLibraryDto`

Represents the persisted media library and contains:

- `Version`
- `Tracks`

The `Version` property identifies the persistence format version. The current version is `1`.

### `MediaTrackDto`

Represents the persisted state required to reconstruct a `MediaTrackModel`:

- `Title`
- `Artist`
- `FilePath`
- `DurationSeconds`
- `IsKaraoke`

Derived properties and display formatting are not persisted because they can be reconstructed from the stored track data.

### `MediaLibraryPersistenceMapper`

Converts between the domain model and persistence DTOs.

Saving maps:

```text
MediaLibraryCollection
        ↓
MediaLibraryDto
        ↓
MediaTrackDto
```

Loading performs the mapping in the opposite direction:

```text
MediaLibraryDto
        ↓
MediaLibraryCollection
        ↓
MediaTrackModel
```

This keeps persistence concerns separate from the media library domain model.

### `JsonMediaLibraryStore`

Handles JSON serialization and file operations.

Saving follows:

```text
MediaLibraryCollection
        ↓
MediaLibraryPersistenceMapper
        ↓
MediaLibraryDto
        ↓
JSON
        ↓
File
```

Loading reverses the process:

```text
File
        ↓
JSON
        ↓
MediaLibraryDto
        ↓
MediaLibraryPersistenceMapper
        ↓
MediaLibraryCollection
```

The generated JSON is indented to keep the persisted library human-readable.

## Persistence Versioning

The current persistence format version is defined by:

```text
MediaLibraryPersistenceMapper.CurrentVersion
```

Files using an unsupported version are rejected with a `NotSupportedException`.

This provides a version boundary that can be extended in the future if the persisted library structure changes.

## Missing and Invalid Data

The persistence store handles different scenarios explicitly:

- A missing persistence file returns an empty `MediaLibraryCollection`
- Invalid JSON results in a `JsonException`
- JSON that does not contain a library object results in an `InvalidDataException`
- An unsupported persistence version results in a `NotSupportedException`
- Invalid method arguments are rejected before persistence operations are attempted

A missing file is treated as a library that has not yet been persisted rather than as an error.

## Example JSON

```json
{
  "Version": 1,
  "Tracks": [
    {
      "Title": "Bohemian Rhapsody",
      "Artist": "Queen",
      "FilePath": "Music\\Queen - Bohemian Rhapsody.mp3",
      "DurationSeconds": 354,
      "IsKaraoke": true
    }
  ]
}
```