# Challenge 029 - Playback History Tests

Unit tests for playback history behaviour.

## Test Coverage

The tests verify that:

- A new playback history starts empty.
- Recording a play adds a history entry.
- A track played once has a play count of one.
- Repeated plays increase the play count.
- An unplayed track has a play count of zero.
- The most recent playback timestamp is returned.
- An unplayed track has no last-played timestamp.
- Playback history is returned newest first.
- Empty history returns an empty collection.
- Empty track paths are rejected.

## Test Results

```text
Test summary: total: 10, failed: 0, succeeded: 10, skipped: 0
```