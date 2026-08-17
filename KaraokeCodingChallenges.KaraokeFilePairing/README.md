# Challenge 003 - Karaoke File Pairing

## Objective

Identify matching MP3 and CDG karaoke file pairs from a collection of files.

## Requirements

- Accept a collection of file paths.
- Recognise MP3 and CDG files.
- Ignore unsupported file types.
- Handle file extensions case-insensitively.
- Match MP3 and CDG files that share the same filename and directory.
- Store matched files as `KaraokeFilePair` objects.
- Identify MP3 files without a matching CDG file.
- Identify CDG files without a matching MP3 file.
- Keep file pairing logic separate from console output.
- Display the input files, matched pairs, unmatched files and summary counts.

## Concepts Practised

- Classes and objects
- Collections
- Generics
- Dictionaries
- HashSets
- File and path handling
- String comparison
- Nested loops
- Nullable reference types
- Separating application logic from presentation

## What I Learned

- How to use a `HashSet` with `StringComparer.OrdinalIgnoreCase` for case-insensitive extension matching.
- How to use a dictionary to group related files using a generated key.
- Why the directory should form part of the pairing key when identical filenames may exist in different folders.
- How `Path.GetDirectoryName()`, `Path.GetFileNameWithoutExtension()` and `Path.Combine()` can be used together to create a file pairing key.
- How nullable strings can represent files that may or may not exist while checking for a matching pair.
- How target-typed `new()` allows C# to infer the object type from the surrounding context.
- How to keep processing logic separate from console output.
- How `Directory.GetFiles()` requires a search pattern when using the overload that accepts `SearchOption`.

## Sample Output

```text
Files:
ABBA - Dancing Queen.cdg
ABBA - Dancing Queen.MP3
Journey - Don't Stop Believin.mp3
Missing Audio.cdg
notes.txt
Queen - Bohemian Rhapsody.Cdg
Queen - Bohemian Rhapsody.mp3

Matched Karaoke Pairs:
ABBA - Dancing Queen
Queen - Bohemian Rhapsody

MP3 Without CDG:
Journey - Don't Stop Believin.mp3

CDG Without MP3:
Missing Audio.cdg

Summary:
Matched pairs: 2
MP3 without CDG: 1
CDG without MP3: 1
```
