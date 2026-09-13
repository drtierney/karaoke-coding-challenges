# Challenge 009 - Track Sorting

## Objective

Sort media library tracks using LINQ and a custom comparer.

## Requirements

- Create a `MediaLibrarySortService` class.
- Accept `IEnumerable<MediaTrackModel>` collections as input.
- Sort tracks by artist and then title.
- Sort tracks by title.
- Sort tracks by duration from shortest to longest.
- Sort tracks by duration from longest to shortest.
- Use both LINQ method syntax and query syntax.
- Create a custom `IComparer<MediaTrackModel>` for sorting by artist and title.
- Perform case-insensitive artist and title comparisons in the custom comparer.
- Return sorted results without modifying the original collection.
- Demonstrate each sorting method against a sample collection of `MediaTrackModel` objects.
- Display the sorted results clearly in the console.

## Concepts Practised

- LINQ sorting
- `OrderBy()`
- `OrderByDescending()`
- `ThenBy()`
- LINQ query syntax
- `orderby`
- `IEnumerable<T>`
- `IComparer<T>`
- Custom comparison logic
- `string.Compare()`
- `StringComparison.OrdinalIgnoreCase`
- Nullable reference types
- Static service classes
- Reusing models through project references
- Separating sorting logic from console output

## What I Learned

- `OrderBy()` sorts a collection using a primary sort condition, while `ThenBy()` can provide a secondary sort condition.
- `OrderByDescending()` can reverse the sorting direction for values such as track duration.
- LINQ query syntax provides the `orderby` keyword as an alternative to method syntax.
- `IComparer<T>` can define reusable custom rules for comparing and sorting objects.
- A comparer returns a negative value when the first object comes before the second, zero when they are equal for sorting, and a positive value when the first object comes after the second.
- `string.Compare()` does not necessarily return only `-1`, `0` or `1`; the sign of the returned value is what determines the ordering.
- `StringComparison.OrdinalIgnoreCase` allows strings to be compared without differences in letter casing affecting the result.
- LINQ sorting returns a sorted sequence without changing the order of the original collection.

## Sample Output

```text
Tracks Unsorted:
Queen - Bohemian Rhapsody
Queen - Don't Stop Me Now
ABBA - Dancing Queen
Oasis - Wonderwall
Journey - Don't Stop Believin'
Toto - Africa
ABBA - Mamma Mia
a-ha - Take On Me

Tracks Sorted By Artist Then Title:
a-ha - Take On Me
ABBA - Dancing Queen
ABBA - Mamma Mia
Journey - Don't Stop Believin'
Oasis - Wonderwall
Queen - Bohemian Rhapsody
Queen - Don't Stop Me Now
Toto - Africa

Tracks Sorted By Title:
Toto - Africa
Queen - Bohemian Rhapsody
ABBA - Dancing Queen
Journey - Don't Stop Believin'
Queen - Don't Stop Me Now
ABBA - Mamma Mia
a-ha - Take On Me
Oasis - Wonderwall

Tracks Sorted By Shortest Duration:
Queen - Don't Stop Me Now
ABBA - Mamma Mia
a-ha - Take On Me
ABBA - Dancing Queen
Journey - Don't Stop Believin'
Oasis - Wonderwall
Toto - Africa
Queen - Bohemian Rhapsody

Tracks Sorted By Longest Duration:
Queen - Bohemian Rhapsody
Toto - Africa
Oasis - Wonderwall
Journey - Don't Stop Believin'
ABBA - Dancing Queen
a-ha - Take On Me
ABBA - Mamma Mia
Queen - Don't Stop Me Now
```
