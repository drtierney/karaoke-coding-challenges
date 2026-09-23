# Challenge 028 - Playback Queue Manager

Create an editable ordered playback queue for library tracks.

## Objective

Build a reusable playback queue that can add, inspect and remove tracks, while allowing a track to be prioritised as the next item without disturbing the order of the remaining queue.

## Requirements

- Create a `PlaybackQueue` class.
- Store `MediaTrackModel` objects in a private collection.
- Preserve the order in which tracks are queued.
- Add tracks to the end of the queue.
- Insert a track at the start of the queue using `QueueNext`.
- Inspect the next track without removing it.
- Remove and return the next track.
- Remove a specific queued track.
- Return whether a requested track was successfully removed.
- Clear all tracks from the queue.
- Expose queued tracks through a read-only collection.
- Expose the current queue count.

## Concepts Practised

- Queue-like behaviour
- Ordered collection mutation
- Encapsulation
- Read-only collection interfaces
- Nullable return values

## Implementation

The `PlaybackQueue` class uses a private `List<MediaTrackModel>` to manage queued tracks internally.

The queue is exposed through `IReadOnlyCollection<MediaTrackModel>`, allowing callers to inspect the queue while ensuring changes are made through queue methods.

Tracks can be:

- Added to the end using `Enqueue`.
- Inserted at the start using `QueueNext`.
- Inspected using `Peek`.
- Removed from the start using `Dequeue`.
- Removed directly using `Remove`.
- Cleared using `Clear`.

`Peek` returns the first queued track without changing the queue.

`Dequeue` returns and removes the first queued track.

Both methods return `null` when the queue is empty.

`Remove` returns a `bool` indicating whether the requested track was present and successfully removed.

## Example

Two tracks were initially queued:

```text
Initial queue:
Count: 2
- Queen - Bohemian Rhapsody
- Queen - Don't Stop Me Now
```

`Somebody To Love` was then queued next:
```text
After QueueNext:
Count: 3
- Queen - Somebody To Love
- Queen - Bohemian Rhapsody
- Queen - Don't Stop Me Now
```

The next track was inspected and dequeued:
```text
Peek: Somebody To Love

Dequeued: Somebody To Love
Count: 2
- Queen - Bohemian Rhapsody
- Queen - Don't Stop Me Now
```

After removing `Don't Stop Me Now`:
```text
After Remove:
Count: 1
- Queen - Bohemian Rhapsody
```

After clearing the queue:
```text
After Clear:
Count: 0
```

## Testing

A dedicated xUnit project verifies the playback queue's public behaviour.

The tests cover:

- New queues starting empty.
- Adding tracks.
- Preserving insertion order.
- Queueing a track next.
- Preserving existing order after `QueueNext`.
- Inspecting the next track without removing it.
- Handling `Peek` on an empty queue.
- Dequeueing the next track.
- Handling `Dequeue` on an empty queue.
- Removing an existing track.
- Handling attempts to remove a track that is not present.
- Clearing all tracks.

All 12 tests pass.