# Challenge 010 - Duplicate Detection

## Objective

Identify duplicate media tracks using custom equality rules.

## Requirements

- Accept a collection of `MediaTrack` objects.
- Compare tracks using artist and title.
- Perform artist and title comparisons case-insensitively.
- Treat tracks with matching artist and title as duplicates even when their file paths differ.
- Use an `IEqualityComparer<MediaTrack>` to define equality.
- Use a `HashSet<MediaTrack>` to identify distinct and duplicate tracks.
- Return distinct and duplicate tracks separately.
- Demonstrate duplicate detection using sample track data.

## Concepts Practised

- `HashSet<T>`
- `IEqualityComparer<T>`
- Custom equality rules
- `Equals()`
- `GetHashCode()`
- `HashCode`
- Case-insensitive string comparison
- `StringComparer.OrdinalIgnoreCase`
- Collections
- `IEnumerable<T>`
- Static service classes
- Project references
- Separation of processing logic from console output

## What I Learned

- A `HashSet<T>` uses equality rules to determine whether an item has already been added.
- `HashSet.Add()` returns `false` when an equivalent item already exists.
- `IEqualityComparer<T>` can define custom equality rules without changing the original model class.
- `Equals()` should compare the actual values used to determine equality.
- `GetHashCode()` should use the same values and comparison rules as `Equals()`.
- Matching hash codes do not necessarily mean two objects are equal.
- `StringComparer.OrdinalIgnoreCase` can be used when generating case-insensitive hash codes.
- Distinct tracks represent the first occurrence of each artist and title combination, while later matching tracks can be identified as duplicates.


## Sample Output

```text
Sample Tracks:
Queen - Bohemian Rhapsody --> D:\Music\Queen - Bohemian Rhapsody.mp3
Queen - Bohemian Rhapsody --> E:\Backup\Queen - Bohemian Rhapsody.mp3
QUEEN - BOHEMIAN RHAPSODY --> D:\Music\TRACK1.MP3
Queen - Don't Stop Me Now --> D:\Karaoke\Queen - Don't Stop Me Now.mp3
ABBA - Dancing Queen --> D:\Music\ABBA - Dancing Queen.mp3
abba - dancing queen --> E:\Backup\ABBA - Dancing Queen.mp3
Oasis - Wonderwall --> D:\Music\Oasis - Wonderwall.mp3
Toto - Africa --> D:\Music\Toto - Africa.mp3

Distinct Tracks:
Queen - Bohemian Rhapsody --> D:\Music\Queen - Bohemian Rhapsody.mp3
Queen - Don't Stop Me Now --> D:\Karaoke\Queen - Don't Stop Me Now.mp3
ABBA - Dancing Queen --> D:\Music\ABBA - Dancing Queen.mp3
Oasis - Wonderwall --> D:\Music\Oasis - Wonderwall.mp3
Toto - Africa --> D:\Music\Toto - Africa.mp3

Duplicate Tracks:
Queen - Bohemian Rhapsody --> E:\Backup\Queen - Bohemian Rhapsody.mp3
QUEEN - BOHEMIAN RHAPSODY --> D:\Music\TRACK1.MP3
abba - dancing queen --> E:\Backup\ABBA - Dancing Queen.mp3
```
