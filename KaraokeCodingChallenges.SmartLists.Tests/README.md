# Challenge 031 - Smart Lists Tests

Unit tests for smart list rules, rule management, and track filtering behaviour.

## Test Coverage

The tests verify that:

- A rule returns `true` when a track matches its predicate.
- A rule returns `false` when a track does not match its predicate.
- A `null` smart list rule name is rejected.
- An empty smart list rule name is rejected.
- A whitespace-only smart list rule name is rejected.
- A `null` smart list rule predicate is rejected.
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