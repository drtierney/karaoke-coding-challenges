# Challenge 007 - Media Library Queries

## Objective

Use LINQ to search a collection of `MediaTrack` objects by artist, title and file path while supporting case-insensitive and partial matching.

## Requirements

- Accept collections of `MediaTrack` objects using `IEnumerable<MediaTrack>`.
- Find all tracks by a specified artist.
- Match artist names case-insensitively.
- Find tracks containing a search term in the title.
- Find tracks containing a search term in the file path.
- Perform title and file-path searches case-insensitively.
- Return matching `MediaTrack` objects rather than formatted strings.
- Handle searches that return no matching tracks.
- Use `Any()` to determine whether query results contain tracks before printing.
- Keep LINQ query logic separate from console output.
- Demonstrate searches using multiple sample tracks and search terms.

## Concepts Practised

- LINQ
- `Where()`
- `Any()`
- `IEnumerable<T>`
- Lambda expressions
- `Contains()`
- `Equals()`
- `StringComparison.OrdinalIgnoreCase`
- Exact and partial matching
- Case-insensitive searching
- Collection expressions
- Helper methods
- Project references
- Reusing the `MediaTrack` model from another project
- Separation of query logic from console output

## What I Learned

- How `Where()` can filter a collection and return all objects that match a condition.
- How LINQ queries can return `IEnumerable<MediaTrack>` without needing to create a new `List<MediaTrack>`.
- How exact searches and partial searches can use different string comparison methods.
- How `StringComparison.OrdinalIgnoreCase` allows searches to work regardless of character casing.
- How `Contains()` can be used for partial title and file-path searches.
- How `Any()` can check whether a query returned any results.
- How query methods can return track objects while separate helper methods decide how those tracks should be displayed.
- How the same query method can be reused for both individual searches and multiple searches in a loop.

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

ABBA Tracks:
Title: Dancing Queen
Artist: ABBA
FilePath: D:\Karaoke\ABBA - Dancing Queen.Mp3
DurationFormatted: 3:51
IsKaraoke: True

Title: Mamma Mia
Artist: ABBA
FilePath: D:\Music\ABBA - Mamma Mia.flac
DurationFormatted: 3:33
IsKaraoke: False

Search results for: Queen
Queen - Bohemian Rhapsody
Queen - Don't Stop Me Now

Search results for: queen
Queen - Bohemian Rhapsody
Queen - Don't Stop Me Now

Search results for: QUEEN
Queen - Bohemian Rhapsody
Queen - Don't Stop Me Now

Search results for: Oasis
Oasis - Wonderwall

Search results for: Unknown Artist
No tracks found.

Tracks containing 'stop' in the title:
Queen - Don't Stop Me Now
Journey - Don't Stop Believin'

Tracks containing 'Yesterday' in the title:
No tracks found.

Tracks containing 'take' in the file path:
D:\Music\a-ha - Take On Me.mp3

Tracks containing 'D:\Music\a' in the file path:
D:\Music\ABBA - Mamma Mia.flac
D:\Music\a-ha - Take On Me.mp3
```
