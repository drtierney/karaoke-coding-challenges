# Challenge 031 - Smart Lists Tests

Unit tests for smart list rules, rule management, and track filtering behaviour.

## Test Coverage

### Smart List Rules

- A rule returns `true` when a track matches its predicate.
- A rule returns `false` when a track does not match its predicate.
- A `null` rule name is rejected.
- An empty rule name is rejected.
- A whitespace-only rule name is rejected.
- A `null` predicate is rejected.

### Smart List Management

- A `null` smart list name is rejected.
- An empty smart list name is rejected.
- A whitespace-only smart list name is rejected.
- A smart list stores its supplied name.
- A new smart list starts with no rules.
- Rules can be added to a smart list.
- Rule insertion order is preserved.
- Removing an existing rule returns `true`.
- Removing a missing rule returns `false`.
- All rules can be cleared.

### Smart List Filtering

- A smart list with no rules returns all tracks.
- A track matching a single rule is returned.
- A track not matching a single rule is excluded.
- A track matching all rules is returned.
- A track failing one of multiple rules is excluded.
- Multiple tracks are filtered so only tracks matching every rule are returned.

## Test Results

```text
Test summary: total: 22, failed: 0, succeeded: 22, skipped: 0
```

# Challenge 032 - Advanced Search & Filter Rules Tests

Expanded unit test coverage for generic smart lists, configurable matching behaviour, rule composition, and reusable media rules.

## Test Coverage

### Generic Smart Lists

- Existing smart list behaviour continues to work after introducing generics.
- `SmartList<T>` stores rules for the supplied item type.
- `SmartListService.Apply<T>()` filters generic collections.
- The generic rule system works with types other than `MediaTrackModel`.

### Match Modes

- `All` is the default match mode.
- `All` requires every rule to match.
- `Any` returns an item when at least one rule matches.
- `Any` excludes an item when no rules match.
- A smart list with no rules returns all items regardless of match mode.
- Unsupported match mode values are rejected.

### Rule Composition

- `SmartListRules.Any()` matches when one or more child rules match.
- `SmartListRules.Any()` does not match when no child rules match.
- `SmartListRules.All()` matches when every child rule matches.
- `SmartListRules.All()` does not match when a child rule fails.
- Composite rules reject `null` rule collections.
- Composite rules reject empty rule collections.
- Composite rules reject collections containing `null` rules.

### Media Track Rules

- Artist searches match case-insensitively.
- Title searches match case-insensitively.
- Combined searches can match either artist or title.
- Karaoke rules distinguish karaoke tracks from music tracks.
- Minimum duration rules include tracks meeting the minimum.
- Tracks below the minimum duration are excluded.
- Tracks exactly equal to the minimum duration are included.
- Negative minimum durations are rejected.
- File path searches return matching tracks.
- File path searches exclude non-matching tracks.
- Search rules reject `null`, empty, or whitespace-only values where applicable.

### Combined Filtering

- Multiple reusable media rules can be added to the same smart list.
- Tracks must satisfy the configured smart list conditions.
- Different tracks can fail different conditions while only the fully matching track is returned.
- Reusable and composed rules work through the same `SmartListService` filtering pipeline.

## Test Results

Challenge 032 tests pass as part of the full solution test suite.

```text
Test summary: total: 69, failed: 0, succeeded: 69, skipped: 0
```
