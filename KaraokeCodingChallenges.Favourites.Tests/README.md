# Challenge 030 - Favourites Tests

Unit tests for favourite state and persistence behaviour.

## Test Coverage

The tests verify that:

- A new favourites manager starts empty.
- A `null` initial favourites collection is rejected.
- Adding a favourite returns `true`.
- Adding the same favourite again returns `false`.
- Existing favourites are identified correctly.
- Missing favourites return `false`.
- Removing an existing favourite returns `true`.
- Removing a missing favourite returns `false`.
- Empty track paths are rejected.
- Existing favourites can be supplied when creating the manager.
- Duplicate initial favourites are removed.
- Saved favourites can be loaded and restored into a manager.
- Saving favourites creates a JSON file.
- Saved favourites can be loaded successfully.
- Missing favourites files return an empty collection.
- Empty persistence file paths are rejected.
- A `null` favourites collection cannot be saved.
- Invalid JSON throws a `JsonException`.

## Test Results

```text
Test summary: total: 20, failed: 0, succeeded: 20, skipped: 0
```