# Library Source Rules Tests

Automated tests for the source-specific library scan rules introduced in Challenge 025 and extracted into a dedicated project during Challenge 033.

## Concepts Practised

- Testing configuration-driven behaviour
- Testing source-specific rules
- Testing file pairing behaviour
- Edge-case testing

## Test Coverage

### Music Sources

- Music sources return no karaoke files.

### Karaoke Sources

- Karaoke sources return all discovered files.

### Mixed Sources

- Mixed sources include matched MP3/CDG pairs.
- Mixed sources exclude standalone MP3 files.
- Mixed sources include unmatched CDG files.
- Files with the same name in different directories are not paired.
- File extension matching is case-insensitive.

## Test Results

- 6 library source rule tests passing.
- 178 total solution tests passing.