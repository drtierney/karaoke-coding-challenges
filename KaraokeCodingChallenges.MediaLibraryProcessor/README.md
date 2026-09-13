# Challenge 011 - Media Library Processor

## Objective

Combine functionality from previous challenges into a simple media library processing workflow.

## Requirements

- Accept a collection of `MediaScannerService` objects.
- Identify duplicate tracks using the existing duplicate detection service.
- Create a distinct collection containing one track for each artist and title combination.
- Filter the distinct collection to karaoke tracks.
- Sort karaoke tracks by artist, then title.
- Display the original, duplicate, distinct, karaoke and sorted karaoke collections.
- Display a summary showing the number of original, duplicate, distinct and karaoke tracks.
- Reuse functionality from previous challenge projects rather than duplicating existing logic.

## Concepts Practised

- Collections
- `IEnumerable<T>`
- LINQ
- Filtering
- Sorting
- Custom equality comparison
- Duplicate detection
- `HashSet<T>`
- Project references
- Reusing functionality across projects
- Static service classes
- Breaking processing into logical stages
- Separation of processing logic from console output
- Combining multiple components into a processing workflow

## What I Learned

- Separate services can be combined to build a larger processing workflow.
- The output from one service can be passed directly into another service using `IEnumerable<T>`.
- Duplicate detection can be performed before filtering and sorting to avoid processing repeated tracks.
- Distinct tracks contain the first occurrence of each artist and title combination according to the custom equality rules.
- Reusing existing services keeps the processor focused on coordinating the workflow rather than duplicating logic.
- Helper methods can keep `Main()` easier to read by separating repeated console output and summary logic.
- A sequence of small processing steps can make a larger task easier to understand and maintain.

## Sample Output

```text
Original Tracks:
Queen - Bohemian Rhapsody --> D:\Music\Queen - Bohemian Rhapsody.mp3
Queen - Bohemian Rhapsody --> E:\Backup\Queen - Bohemian Rhapsody.mp3
Queen - Don't Stop Me Now --> D:\Karaoke\Queen - Don't Stop Me Now.mp3
ABBA - Dancing Queen --> D:\Karaoke\ABBA - Dancing Queen.mp3
ABBA - Mamma Mia --> D:\Music\ABBA - Mamma Mia.flac
a-ha - Take On Me --> D:\Karaoke\a-ha - Take On Me.mp3
Toto - Africa --> D:\Music\Toto - Africa.mp3
ABBA - DANCING QUEEN --> E:\Backup\ABBA - Dancing Queen.mp3

Duplicate Tracks:
Queen - Bohemian Rhapsody --> E:\Backup\Queen - Bohemian Rhapsody.mp3
ABBA - DANCING QUEEN --> E:\Backup\ABBA - Dancing Queen.mp3

Distinct Tracks:
Queen - Bohemian Rhapsody --> D:\Music\Queen - Bohemian Rhapsody.mp3
Queen - Don't Stop Me Now --> D:\Karaoke\Queen - Don't Stop Me Now.mp3
ABBA - Dancing Queen --> D:\Karaoke\ABBA - Dancing Queen.mp3
ABBA - Mamma Mia --> D:\Music\ABBA - Mamma Mia.flac
a-ha - Take On Me --> D:\Karaoke\a-ha - Take On Me.mp3
Toto - Africa --> D:\Music\Toto - Africa.mp3

Karaoke Tracks:
Queen - Don't Stop Me Now --> D:\Karaoke\Queen - Don't Stop Me Now.mp3
ABBA - Dancing Queen --> D:\Karaoke\ABBA - Dancing Queen.mp3
a-ha - Take On Me --> D:\Karaoke\a-ha - Take On Me.mp3

Sorted Karaoke Tracks:
a-ha - Take On Me --> D:\Karaoke\a-ha - Take On Me.mp3
ABBA - Dancing Queen --> D:\Karaoke\ABBA - Dancing Queen.mp3
Queen - Don't Stop Me Now --> D:\Karaoke\Queen - Don't Stop Me Now.mp3

Media Library Summary
---------------------
Original tracks: 8
Duplicate tracks: 2
Distinct tracks: 6
Karaoke tracks: 3
```
