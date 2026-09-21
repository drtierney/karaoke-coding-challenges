# Metadata Library Milestone Tests

Automated tests for source-specific library scan rules introduced in Challenge 025.

## Concepts Practised

- Testing configuration-driven behaviour
- Testing source-specific rules
- Testing file pairing behaviour
- Edge-case testing

## Tests Performed

- Music sources return no karaoke files.
- Karaoke sources return all discovered files for pairing.
- Mixed sources include matched MP3/CDG pairs.
- Mixed sources exclude standalone MP3 files.
- Mixed sources include unmatched CDG files.
- Files with the same name in different directories are not paired.
- File extension matching is case-insensitive.

## Test Results

- 6 Challenge 025 tests passing.
- 21 total solution tests passing.