# Challenge 006 - Media Library

## Objective

Build an in-memory media library that supports adding, removing, finding, counting and clearing tracks while exposing its contents through a read-only collection.

## Requirements

- Store `MediaTrack` objects in a private collection.
- Add a single track to the library.
- Add multiple tracks to the library.
- Remove a track by object reference.
- Remove a track by file path.
- Find a track by file path.
- Compare file paths case-insensitively.
- Expose the library contents as a read-only collection.
- Return the current number of tracks in the library.
- Clear all tracks from the library.
- Keep library management logic separate from console output.
- Demonstrate the library operations using sample `MediaTrack` objects.

## Concepts Practised

- `List<T>`
- `IEnumerable<T>`
- `IReadOnlyList<T>`
- Encapsulation
- Collection expressions
- Method overloading
- Nullable reference types
- Lambda expressions
- Case-insensitive string comparison
- Collection add, remove, find and clear operations
- Project references
- Reusing functionality from another project
- Separation of application logic from console output

## What I Learned

- How to encapsulate a `List<T>` inside a dedicated class.
- How `IReadOnlyList<T>` can expose collection contents without allowing callers to modify them directly.
- How method overloading can support removing tracks in different ways.
- How nullable return types represent a search that may not find a match.
- How lambda expressions can be used with `List<T>.Find()`.
- How case-insensitive comparison makes file-path matching more reliable.

## Sample Output

```text
Tracks in library: 5
Removed test track: True
Removed missing track: False
Tracks in library: 4

Found: Oasis - Wonderwall
Removed 'Oasis - Wonderwall' by file path: True
Tracks in library: 3

Media Library Contents:
Title: Bohemian Rhapsody
Artist: Queen
FilePath: D:\Music\Queen - Bohemian Rhapsody.mp3
DurationFormatted: 5:54
IsKaraoke: False

Title: Don't Stop Believin'
Artist: Journey
FilePath: D:\Karaoke\Journey - Don't Stop Believin.mp3
DurationFormatted: 4:11
IsKaraoke: True

Title: Dancing Queen
Artist: ABBA
FilePath: D:\Karaoke\ABBA - Dancing Queen.mp3
DurationFormatted: 3:51
IsKaraoke: True

Tracks after clearing library: 0
```
