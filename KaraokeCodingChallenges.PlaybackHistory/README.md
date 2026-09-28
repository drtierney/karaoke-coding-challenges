# Challenge 029 - Playback History & Play Counts

Track playback history, play counts, and last-played timestamps.

## Concepts Practised

- Records
- Collections
- LINQ
- `DateTimeOffset`
- Counters
- State management
- Domain methods
- Input validation

## Implementation

Created a `PlaybackHistoryEntry` record containing:

- Track path.
- Playback timestamp.

Created a `PlaybackHistoryManager` class that:

- Records individual playback events.
- Maintains playback history in memory.
- Returns playback history ordered from newest to oldest.
- Calculates the number of times a track has been played.
- Returns the most recent playback time for a track.
- Compares track paths case-insensitively when querying play counts and last-played timestamps.
- Returns zero for tracks that have not been played.
- Returns `null` when a track has no previous playback time.
- Rejects empty or whitespace-only track paths.

A play is recorded when playback starts.

Each playback is stored as a separate history entry, allowing repeated plays of the same track to be retained while aggregated information such as play counts and last-played times can still be queried.

Track paths are preserved as recorded, while play-count and last-played lookups use case-insensitive path comparisons.