# Challenge 030 - Favourites

Mark tracks as favourites and preserve that preference across application runs.

## Concepts Practised

- `HashSet<T>`
- State management
- JSON serialization
- JSON deserialization
- File persistence
- Input validation
- Read-only collections
- Constructor initialization

## Implementation

Created a `FavouriteManager` class that:

- Adds tracks to favourites.
- Removes tracks from favourites.
- Checks whether a track is currently favourited.
- Prevents duplicate favourite entries.
- Returns favourites through a read-only collection.
- Rejects empty or whitespace-only track paths.
- Supports initialization from an existing favourites collection.

Created a `FavouriteStore` class that:

- Serializes favourites to JSON.
- Saves favourite state to a file.
- Loads previously saved favourites.
- Returns an empty collection when no favourites file exists.
- Rejects empty file paths.
- Rejects a `null` favourites collection.
- Allows invalid JSON to surface as a `JsonException`.

Track paths are currently used as identifiers for favourite entries, matching the existing playback-history approach.

The persistence layer is kept separate from `FavouriteManager`, allowing favourite state management and file storage to remain independent responsibilities.