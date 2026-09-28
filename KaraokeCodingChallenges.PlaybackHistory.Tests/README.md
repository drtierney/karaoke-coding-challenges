# Challenge 029 - Playback History Tests

Unit tests for playback history behaviour.

## Test Coverage

### Recording History

- A new playback history starts empty.
- Recording a play adds a history entry.
- Playback history is returned newest first.
- Empty history returns an empty collection.
- Empty track paths are rejected.

### Play Counts

- A track played once has a play count of one.
- Repeated plays increase the play count.
- An unplayed track has a play count of zero.
- Play-count lookups are case-insensitive.

### Last Played

- The most recent playback timestamp is returned.
- An unplayed track has no last-played timestamp.
- Last-played lookups are case-insensitive.

## Test Results

```text
Test summary: total: 12, failed: 0, succeeded: 12, skipped: 0
```