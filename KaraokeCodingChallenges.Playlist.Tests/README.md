# Challenge 026 - Playlist Tests

Add automated unit tests for playlist behaviour.

## Concepts Practised

- Testing collection behaviour
- Testing ordered collections
- Testing method return values
- Testing observable behaviour

## Tests Performed

- A new playlist starts with no tracks.
- Adding a track increases the playlist count.
- Added tracks are stored in insertion order.
- Removing an existing track returns `true`.
- Removing a track that does not exist returns `false`.
- Clearing a playlist removes all tracks.

## Test Results

```text
Test summary: total: 6, failed: 0, succeeded: 6, skipped: 0
```

## Key Takeaways

- Playlist behaviour can be verified through its public interface.
- Ordered collections can be tested by checking item positions.
- Returning `bool` from `RemoveTrack` makes successful and unsuccessful removals easy to verify.
- Tests should focus on observable behaviour rather than the private `_tracks` collection.
