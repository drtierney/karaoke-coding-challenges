# Smart Lists Demo

Manual verification application for Challenge 031 - Smart Lists and Challenge 032 - Advanced Search & Filter Rules.

# Challenge 031 - Smart Lists

## Demonstration

The demo:

- Creates a sample collection of music and karaoke tracks.
- Displays all available track details.
- Creates a smart list named `Queen Karaoke`.
- Adds a rule requiring tracks to be karaoke.
- Adds a rule requiring the artist to be Queen.
- Applies the smart list using AND semantics.
- Displays the matching track details.
- Demonstrates that only tracks satisfying every rule are returned.

The demo uses top-level statements and collection expressions to demonstrate modern C# syntax while retaining explicit types for important domain objects.

## Example Output

```text
All Tracks

Title: Bohemian Rhapsody
Artist: Queen
FilePath: D:\Karaoke\Queen - Bohemian Rhapsody.mp3
DurationFormatted: 5:54
IsKaraoke: True

Title: Don't Stop Me Now
Artist: Queen
FilePath: D:\Music\Queen - Don't Stop Me Now.mp3
DurationFormatted: 3:30
IsKaraoke: False

Title: Misery Business
Artist: Paramore
FilePath: D:\Karaoke\Paramore - Misery Business.mp3
DurationFormatted: 3:20
IsKaraoke: True

Title: Still Into You
Artist: Paramore
FilePath: D:\Music\Paramore - Still Into You.mp3
DurationFormatted: 3:40
IsKaraoke: False

Smart List: Queen Karaoke

Rule: Karaoke tracks
Rule: Queen tracks

Matching Tracks

Title: Bohemian Rhapsody
Artist: Queen
FilePath: D:\Karaoke\Queen - Bohemian Rhapsody.mp3
DurationFormatted: 5:54
IsKaraoke: True
```

# Challenge 032 - Advanced Search & Filter Rules

## Demonstration

The updated demo:

- Creates a sample collection containing music and karaoke tracks.
- Displays the available tracks in a compact format.
- Creates a smart list named `Long Karaoke`.
- Uses the `All` match mode.
- Adds the reusable `KaraokeOnly()` rule.
- Adds the reusable `MinimumDuration(180)` rule.
- Applies the rules using `SmartListService`.
- Sorts the matching tracks by artist and title using the existing `MediaLibrarySortService`.
- Displays the filtered and sorted results.
- Displays the number of tracks that matched.

The updated demo shows how reusable smart list rules can be combined with existing library services without coupling filtering and sorting responsibilities.

## Example Output

```text
All Tracks:

Queen - Bohemian Rhapsody [Karaoke] (5:54)
Queen - Don't Stop Me Now [Music] (3:30)
Paramore - Misery Business [Karaoke] (3:20)
Paramore - Still Into You [Music] (3:40)

Smart List: Long Karaoke
Match Mode: All

Rules:
- Karaoke only
- At least 180 seconds

Matching Tracks (sorted by artist and title):

Paramore - Misery Business [Karaoke] (3:20)
Queen - Bohemian Rhapsody [Karaoke] (5:54)

2 of 4 tracks matched.
```
