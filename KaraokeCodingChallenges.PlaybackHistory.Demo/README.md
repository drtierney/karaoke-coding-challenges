# Playback History Demo

Manual verification application for Challenge 029 - Playback History & Play Counts.

## Demonstration

The demo:

- Creates a `PlaybackHistoryManager`.
- Records multiple playback events.
- Records the same track more than once.
- Displays playback history from newest to oldest.
- Displays the total play count for each track.
- Displays the most recent playback time for a selected track.

## Example Output

```text
Playback History

Track: Queen - Bohemian Rhapsody.mp3
Played at: 24/09/2026 19:00:00 +00:00
Play count: 2

Track: Journey - Don't Stop Believin'.mp3
Played at: 24/09/2026 18:45:00 +00:00
Play count: 1

Track: Queen - Bohemian Rhapsody.mp3
Played at: 24/09/2026 18:30:00 +00:00
Play count: 2

Track Summary

Track: Queen - Bohemian Rhapsody.mp3
Play count: 2
Last played: 24/09/2026 19:00:00 +00:00
```