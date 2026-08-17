# Challenge 004 - Media Type Detection

## Objective

Classify media files based on their file extension.

## Requirements

- Accept a supplied file path.
- Extract the file extension.
- Recognise MP3, FLAC and OPUS files as audio.
- Recognise CDG files as karaoke graphics.
- Identify unsupported file types.
- Handle file extensions case-insensitively.
- Use a switch expression to classify file types.
- Store the classification using a `MediaFileType` enum.
- Display the detected media type for each file.

## Concepts Practised

- Enums
- Switch expressions
- Pattern matching
- File extensions
- `Path.GetExtension()`
- `Path.GetFileName()`
- String normalisation
- Case-insensitive file handling
- Separating logic into reusable classes

## What I Learned

- How enums can represent a fixed set of related values.
- How switch expressions provide a concise alternative to traditional `switch` and `case` statements.
- How the `or` pattern can group multiple matching values within a switch expression.
- How `ToLowerInvariant()` can normalise technical values such as file extensions for case-insensitive matching.
- How `Path.GetExtension()` and `Path.GetFileName()` can be used when working with file paths.
- The importance of choosing names that accurately describe what a class or enum represents.

## Sample Output

```text
Files:
ABBA - Mamma Mia.opus -> Audio
cover.jpg -> Unsupported
notes.txt -> Unsupported
Oasis - Wonderwall.FLAC -> Audio
Queen - Bohemian Rhapsody.cdg -> KaraokeGraphics
Queen - Bohemian Rhapsody.mp3 -> Audio
```
