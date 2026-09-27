# Challenge 031 - Smart Lists

Create dynamic track collections using reusable filtering rules.

## Concepts Practised

- `Func<T, bool>`
- Predicate-based filtering
- LINQ `Where()`
- LINQ `All()`
- Read-only collections
- Input validation
- Rule composition
- Separation of responsibilities

## Implementation

Created a `SmartListRule` class that:

- Stores a descriptive rule name.
- Stores a predicate using `Func<MediaTrackModel, bool>`.
- Evaluates whether a track matches the rule.
- Rejects `null`, empty, or whitespace-only rule names.
- Rejects a `null` predicate.

Created a `SmartList` class that:

- Requires a non-empty name.
- Stores rules in insertion order.
- Exposes rules through a read-only collection.
- Adds rules.
- Removes rules.
- Clears all rules.
- Exposes the current rule count.

Created a `SmartListService` class that:

- Accepts a smart list and a collection of tracks.
- Evaluates each track against the smart list rules.
- Uses AND semantics so a track must satisfy every rule.
- Returns all tracks when the smart list contains no rules.
- Returns filtered results without modifying the original collection.

Rule evaluation is implemented using LINQ `Where()` and `All()`.

The current implementation uses AND semantics for multiple rules. Support for alternative matching behaviour such as OR semantics may be introduced in a future challenge.