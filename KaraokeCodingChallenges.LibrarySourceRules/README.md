# Library Source Rules

Reusable source-specific rules for determining which discovered files should be treated as karaoke content.

The rules were originally introduced in Challenge 025 as part of the Metadata Library Milestone and extracted into a dedicated project during Challenge 033.

## Concepts Practised

- Source-specific behaviour
- Configuration-driven rules
- File pairing
- Reusable application services
- Separation of responsibilities

## Implementation

`LibrarySourceRulesService` applies karaoke classification rules based on the configured `LibrarySourceType`.

### Music Sources

Music sources contain standalone music files and do not return files for karaoke pairing.

### Karaoke Sources

Karaoke sources treat all discovered files as karaoke content.

### Mixed Sources

Mixed sources can contain both standalone music and karaoke content.

For mixed sources:

- Matching MP3/CDG pairs are treated as karaoke files.
- Standalone MP3 files are excluded from the karaoke file collection.
- Unmatched CDG files remain included.
- Pairing is performed within the same directory.
- File extension matching is case-insensitive.

## Challenge 033 Refactoring

`LibrarySourceRulesService` was originally part of the Metadata Library Milestone application.

Challenge 033 extracted the service into `KaraokeCodingChallenges.LibrarySourceRules` so source-specific rules can be reused independently by other application workflows.

The earlier milestone and the Search & Playlist Milestone now both consume the dedicated project rather than duplicating the source rule logic.