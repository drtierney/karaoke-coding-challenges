# Challenge 026 - Playlist Model

Create an in-memory playlist model that stores an ordered collection of library tracks.

## Objective

Build a reusable playlist that can add, remove and clear tracks while preserving their order and exposing the collection through a read-only interface.

## Requirements

- Create a `MediaPlaylist` class.
- Give each playlist a unique `Guid` identifier.
- Require a playlist name when the playlist is created.
- Store `MediaTrackModel` objects in a private collection.
- Preserve the order in which tracks are added.
- Add tracks to the playlist.
- Remove tracks from the playlist.
- Return whether a requested track was successfully removed.
- Clear all tracks from the playlist.
- Expose playlist tracks as an `IReadOnlyList<MediaTrackModel>`.
- Expose the current track count.
- Allow multiple versions or occurrences of a track to be added.
- Demonstrate playlist behaviour using sample tracks.

## Concepts Practised

- Ordered collections
- Encapsulation
- `Guid` identifiers
- Read-only collection interfaces
- Method return values
- Reusing an existing domain model

## Implementation

The `MediaPlaylist` class uses a private `List<MediaTrackModel>` to manage its tracks internally.

The tracks are exposed publicly through `IReadOnlyList<MediaTrackModel>`, allowing callers to inspect the playlist while ensuring changes are made through playlist methods.

Each playlist receives a unique `Guid` when it is created.

Tracks can be:

- Added using `AddTrack`.
- Removed using `RemoveTrack`.
- Cleared using `Clear`.

`RemoveTrack` returns a `bool` indicating whether the requested track was present and successfully removed.

Tracks remain in insertion order.

No duplicate prevention is applied. Different recordings, karaoke versions, music versions, or repeated occurrences of the same track may all be valid entries in a playlist.

## Example

Three tracks were added to a test playlist:

```text
Playlist: Test Playlist
Track count: 3

Queen - Bohemian Rhapsody
Paramore - The Only Exception
Harry Styles - Sign of the Times
```

After removing `The Only Exception`:

```text
Playlist: Test Playlist
Track count: 2

Queen - Bohemian Rhapsody
Harry Styles - Sign of the Times
```

After clearing the playlist:

```text
Playlist: Test Playlist
Track count: 0
```

The playlist retained the same `Guid` throughout each operation.

## Testing

A dedicated xUnit project verifies the playlist's public behaviour.

The tests cover:

- New playlists starting empty.
- Adding tracks.
- Preserving insertion order.
- Successfully removing an existing track.
- Handling attempts to remove a track that is not present.
- Clearing all tracks.

All 6 tests pass.
