# Challenge 028 - Playback Queue Tests

Add automated unit tests for playback queue behaviour.

## Concepts Practised

- Testing ordered collection behaviour
- Testing queue mutation
- Testing nullable results
- Testing observable state

## Tests Performed

- A new playback queue starts empty.
- Enqueueing a track adds it to the queue.
- Enqueued tracks preserve insertion order.
- `QueueNext` inserts a track at the start of the queue.
- `QueueNext` preserves the existing queue order.
- `Peek` returns the first track without removing it.
- `Peek` returns `null` when the queue is empty.
- `Dequeue` returns and removes the first track.
- `Dequeue` returns `null` when the queue is empty.
- Removing an existing track returns `true`.
- Removing a track that does not exist returns `false`.
- Clearing the queue removes all tracks.

## Test Results

```text
Test summary: total: 12, failed: 0, succeeded: 12, skipped: 0
```

## Key Takeaways
- Queue behaviour can be verified through the public interface.
- Ordered collections can support both FIFO-style operations and explicit queue editing.
- `Peek` and `Dequeue` provide different behaviour for inspecting and consuming the next track.
- Returning nullable results allows empty queue operations to be handled without exceptions.