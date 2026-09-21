# Challenge 021 - Metadata Library Milestone

Integrate the metadata scanning components from Challenges 012-020 into a complete library scanning workflow.

The milestone application was subsequently extended through Challenges 022-025 with JSON configuration, configuration validation, multiple library sources, and source-specific scan rules.

## Concepts Practised

- Service integration
- Project references
- File discovery
- Metadata filtering by file extension
- LINQ with `Where` and `ToList`
- Deferred execution and materialisation
- Error and warning reporting
- Karaoke file pairing
- Scan statistics
- Console output formatting
- Configuration-driven behaviour
- Multiple library sources
- Source-specific processing
- Separation of responsibilities

## Implementation

The milestone combines the existing library scanner, metadata reader, media scan service, karaoke pairing service, statistics service, and configuration components into one workflow.

The program:

- Loads library sources from `appsettings.json`.
- Validates the loaded configuration before scanning.
- Supports multiple enabled or disabled library sources.
- Handles unavailable library locations without stopping the complete scan.
- Discovers supported files recursively from each available source.
- Applies source-specific rules based on whether a library is configured as `Music`, `Karaoke`, or `Mixed`.
- Separates metadata-capable files from karaoke graphics files.
- Scans MP3, FLAC, and Opus files for metadata.
- Preserves per-file warnings and errors.
- Pairs matching MP3 and CDG karaoke files where appropriate.
- Generates scan statistics.
- Reports metadata completeness and karaoke pairing results.

CDG files are included in library discovery and karaoke pairing, but are not passed to the metadata reader.

## Library Source Types

Configured library sources use a `LibrarySourceType` to control how discovered files are processed.

### Music

Music sources contain standalone audio files.

- Audio files are included in metadata scanning.
- Files are not treated as karaoke files.
- MP3 files without matching CDG files are not reported as incomplete karaoke pairs.

### Karaoke

Karaoke sources are expected to contain karaoke content.

- MP3 and CDG files are passed to the karaoke pairing service.
- Matching MP3/CDG files are reported as karaoke pairs.
- MP3 files without matching CDG files are reported as unmatched.
- CDG files without matching MP3 files are reported as unmatched.

### Mixed

Mixed sources may contain both standalone music and karaoke files.

- All supported audio files remain available for metadata scanning.
- Matching MP3/CDG files are treated as karaoke pairs.
- Standalone MP3 files are treated as valid music and are not reported as missing CDG files.
- Unmatched CDG files are still treated as incomplete karaoke files.

## Source-Specific Scan Rules

Challenge 025 introduced `LibrarySourceRulesService` to keep source-specific behaviour separate from the main application workflow.

The service receives:

- The configured `LibrarySourceType`.
- The files discovered within that source.

It returns the files that should participate in karaoke pairing.

This keeps `Program.cs` responsible for application orchestration while the rules for `Music`, `Karaoke`, and `Mixed` sources remain isolated in a dedicated service.

## Automated Tests

Challenge 025 added a dedicated `KaraokeCodingChallenges.MetadataLibraryMilestone.Tests` project.

Six automated tests verify that:

- Music sources return no karaoke files.
- Karaoke sources return all discovered files for karaoke processing.
- Mixed sources include matching MP3/CDG pairs.
- Mixed sources exclude standalone MP3 files from karaoke processing.
- Mixed sources include unmatched CDG files.
- Files with matching names in different directories are not incorrectly paired.
- File extension matching is case-insensitive.

## Example Results

The original milestone was tested against a mixed music and karaoke library containing:

- Standard music folders across multiple years.
- Karaoke folders containing matching MP3 and CDG files.

The scan produced:

- 859 supported files discovered.
- 714 metadata-capable files scanned.
- 714 successful metadata scans.
- 145 matched karaoke pairs.
- 569 unmatched MP3 files.
- 0 unmatched CDG files.

The pairing totals matched the original library structure: the karaoke collection contained 145 MP3/CDG pairs, while the remaining 569 MP3 files belonged to the standard music library.

Challenge 025 was manually verified using separate Music, Karaoke, and Mixed sources. The scan correctly distinguished standalone music from incomplete karaoke content.

The updated source-specific rules were also verified against the full library. The scan classified 569 files as Music and 290 files as Karaoke, producing 145 matched karaoke pairs with 0 unmatched MP3 files and 0 unmatched CDG files.

```text
Music source: 569 files
Karaoke source: 290 files
Files found: 859
Metadata files: 714

Matched karaoke pairs: 145
Unmatched MP3 files: 0
Unmatched CDG files: 0
```

## Notes

The milestone demonstrates the difference between:

- Supported library files.
- Metadata-capable files.
- Karaoke pairing files.
- Source-specific processing rules.

Materialising scan results with `ToList()` allows the same results to be reused for both display and statistics without rescanning the files.

Separating source-specific rules into `LibrarySourceRulesService` keeps configuration-driven behaviour out of the main orchestration logic and allows the rules to be tested independently.