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

Challenge 032 extends this implementation with generic rules, configurable matching behaviour, reusable media rules, and rule composition.

# Challenge 032 - Advanced Search & Filter Rules

Extend smart lists into a generic and reusable filtering system that can combine multiple search and filter conditions.

## Concepts Practised

- Generics
- Generic predicates
- LINQ `All()` and `Any()`
- Predicate composition
- Reusable filter rules
- Enum-based behaviour
- Generic type inference
- Input validation
- Separation of generic and domain-specific logic
- Reuse across existing services

## Implementation

Refactored `SmartListRule` into `SmartListRule<T>` so that:

- Rules can evaluate any item type rather than only `MediaTrackModel`.
- Predicates are stored using `Func<T, bool>`.
- Existing rule name and predicate validation is retained.
- The same rule infrastructure can be reused outside the media library.

Refactored `SmartList` into `SmartList<T>` so that:

- A smart list contains rules for a specific item type.
- Rules remain ordered and exposed through a read-only collection.
- Existing add, remove, clear, and rule count behaviour is retained.
- A `SmartListMatchMode` determines how multiple rules are evaluated.

Added `SmartListMatchMode` with:

- `All` - every rule must match.
- `Any` - at least one rule must match.

`All` remains the default matching mode.

A smart list with no rules returns all supplied items regardless of match mode, treating an empty smart list as no filtering.

Updated `SmartListService` with a generic `Apply<T>()` method that:

- Accepts a `SmartList<T>` and `IEnumerable<T>`.
- Uses LINQ `All()` for `All` matching.
- Uses LINQ `Any()` for `Any` matching.
- Returns the filtered items as a read-only list.
- Allows the generic type to be inferred from the supplied smart list and collection.

Added `SmartListRules` to support generic rule composition.

`SmartListRules.Any()` creates a rule that matches when any supplied child rule matches.

`SmartListRules.All()` creates a rule that matches only when every supplied child rule matches.

Composite rules reject:

- A `null` rule collection.
- An empty rule collection.
- Collections containing `null` rules.

Added `MediaTrackRules` as a media-specific convenience layer over the generic rule system.

Reusable media rules include:

- `ArtistContains()` for case-insensitive artist searching.
- `TitleContains()` for case-insensitive title searching.
- `SearchContains()` for matching either artist or title.
- `KaraokeOnly()` for karaoke tracks.
- `MinimumDuration()` for tracks meeting a minimum duration.
- `FilePathContains()` for case-insensitive file path searching.

Search values reject `null`, empty, or whitespace-only input where applicable. Minimum durations cannot be negative.

`SearchContains()` demonstrates rule composition by combining the existing artist and title rules using `SmartListRules.Any()`.

The generic rule engine remains independent of media-specific behaviour. `MediaTrackRules` provides convenient rules for `MediaTrackModel` without coupling `SmartList<T>` or `SmartListService` to the media library.

The filtering results can also be passed to existing services such as `MediaLibrarySortService`, allowing search and filter rules to be combined with existing sorting behaviour without introducing a new sorting implementation.

## Future Extensions

The reusable media rule collection can be extended as additional application requirements are introduced.

Potential rules include:

- `MusicOnly()` for excluding karaoke tracks.
- `MaximumDuration()` for applying an upper duration limit.
- `DurationBetween()` for filtering within a duration range.
- `FavouriteOnly()` using favourite state.
- `UnplayedOnly()` using playback history.
- `PlayedAtLeast()` using play counts.
- Additional metadata rules as the media model expands.

Rules that depend on other application services, such as favourites or playback history, can capture those services through their predicates without coupling the generic smart list engine to them.
