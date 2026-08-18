# Challenge 005 - File Validation

## Objective

Validate media file paths before they are processed by the application.

## Requirements

- Accept a nullable file path.
- Reject null, empty or whitespace-only paths.
- Use the existing `MediaTypeDetector` from Challenge 004 to identify supported file types.
- Reject unsupported file types.
- Check whether supported files exist.
- Return a `FileValidationStatus` describing the validation result.
- Demonstrate validation against multiple valid and invalid file paths.

## Validation Statuses

- `Valid`
- `InvalidPath`
- `FileNotFound`
- `UnsupportedFileType`

## Concepts Practised

- Guard clauses
- Nullable reference types
- File validation
- `File.Exists()`
- Enums
- Static methods and utility classes
- Project references
- Reusing functionality from another project
- Separation of validation logic from console output

## What I Learned

- How static methods can be called directly from a class without creating an instance.
- How one project can reference and reuse code from another project.
- How guard clauses can simplify validation logic by returning early when a condition fails.
- How nullable reference types allow a method to explicitly accept `null` values.
- How validation can return a descriptive status instead of only `true` or `false`.
- How the order of validation checks affects which result is returned.

## Sample Output

```text
File: D:\KaraokeTest\Queen - Bohemian Rhapsody.mp3 -> Validation Status: Valid
File: D:\KaraokeTest\Queen - Bohemian Rhapsody.cdg -> Validation Status: Valid
File: D:\KaraokeTest\Oasis - Wonderwall.FLAC -> Validation Status: Valid
File: D:\KaraokeTest\cover.jpg -> Validation Status: UnsupportedFileType
File: D:\KaraokeTest\missing.mp3 -> Validation Status: FileNotFound
File: <empty> -> Validation Status: InvalidPath
File: <null> -> Validation Status: InvalidPath
```
