# Playback Queue Demo

Manual verification application for Challenge 028 - Playback Queue Manager.

## Verification

The demo:

- Creates a `PlaybackQueue`.
- Adds tracks using `Enqueue`.
- Inserts a track at the front using `QueueNext`.
- Displays the current queue order.
- Inspects the next track using `Peek`.
- Removes the next track using `Dequeue`.
- Removes a specific queued track.
- Clears the queue.

The demo confirmed that queue ordering and mutation behaved as expected throughout each operation.