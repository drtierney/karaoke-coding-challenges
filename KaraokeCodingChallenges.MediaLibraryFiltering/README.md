# Challenge 008 - Media Library Filtering

## Objective

Filter media library tracks using LINQ query syntax based on useful track properties.

## Requirements

- Create a `MediaLibraryFilterService` class.
- Accept `IEnumerable<MediaTrack>` collections as input.
- Use LINQ query syntax for filtering.
- Filter karaoke tracks.
- Filter non-karaoke music tracks.
- Filter tracks by a minimum duration.
- Filter tracks by a maximum duration.
- Return filtered results without modifying the original collection.
- Demonstrate each filter against a sample collection of `MediaTrack` objects.
- Display the filtered results clearly in the console.

## Concepts Practised

- LINQ query syntax
- `IEnumerable<T>`
- Filtering collections
- `where`
- `select`
- Boolean conditions
- Method parameters
- Static service classes
- Reusing models through project references
- Separating filtering logic from console output

## What I Learned

- LINQ query syntax provides a readable alternative to method syntax for filtering collections.
- `IEnumerable<T>` works well for methods that only need to read and return collections.
- Small helper methods such as `PrintTracks()` can keep `Main()` cleaner by separating presentation logic from filtering logic.

## Sample Output

```text
All Tracks:
Queen - Bohemian Rhapsody
Queen - Don't Stop Me Now
ABBA - Dancing Queen
Oasis - Wonderwall
Journey - Don't Stop Believin'
Toto - Africa
ABBA - Mamma Mia
a-ha - Take On Me

Karaoke Tracks:
Queen - Don't Stop Me Now
ABBA - Dancing Queen
Journey - Don't Stop Believin'

Music Tracks:
Queen - Bohemian Rhapsody
Oasis - Wonderwall
Toto - Africa
ABBA - Mamma Mia
a-ha - Take On Me

Tracks with Minimum Duration of 240 seconds:
Queen - Bohemian Rhapsody
Oasis - Wonderwall
Journey - Don't Stop Believin'
Toto - Africa

Tracks with Maximum Duration of 240 seconds:
Queen - Don't Stop Me Now
ABBA - Dancing Queen
ABBA - Mamma Mia
a-ha - Take On Me
```
