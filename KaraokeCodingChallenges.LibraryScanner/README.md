# Challenge 002 - Karaoke Library Scanner

## Objective

Scan a directory and its subdirectories for supported music and karaoke files.

## Requirements

- Scan a supplied directory recursively.
- Recognise MP3, OPUS, FLAC and CDG files.
- Ignore unsupported file types.
- Handle file extensions case-insensitively.
- Store matching file paths in a collection.
- Count supported files by extension.
- Display the scan results.

## Concepts Practised

- File and directory handling
- Collections
- Generics
- Dictionaries
- String comparison
- Filtering
- Recursive directory scanning

## What I Learned

- Using defensive coding to validate a directory before scanning it.
- Using the `System.IO` namespace to work with files, directories and file extensions.
- Using a `HashSet` for efficient membership checks.
- Using a dictionary to count occurrences of each file extension.
- Using `TryGetValue()` to update dictionary values.

## Notes

- I used a `HashSet` to store the supported file extensions for efficient lookups.
- I needed some guidance identifying the appropriate .NET methods for recursively scanning directories and retrieving file extensions.

## Improvements

- Add error handling for file access issues.
- Implement logging for debugging and monitoring.

## Sample Output

```text
Enter folder to scan:
D:\KaraokeTest

Scanning folder: D:\KaraokeTest

Supported files found: 6

Files:
10 - The Beatles - A Hard Days Night (1964).Flac
MØ - Final Song.mp3
Snakehips Feat Tinashe & Chance The Rapper - All My Friends (Clean).MP3
Bon Jovi - Livin On A Prayer.opus
07 - Womack and Womack - Teardrops.cdg
07 - Womack and Womack - Teardrops.mp3

Files by extension:
.flac: 1
.mp3: 3
.opus: 1
.cdg: 1