# Smart Lists Demo

Manual verification application for Challenge 031 - Smart Lists.

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