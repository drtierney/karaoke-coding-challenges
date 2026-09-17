# Challenge 021 - Metadata Library Milestone

Integrate the metadata scanning components from Challenges 012-020 into a complete library scanning workflow.

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
- Separation of scanning and presentation logic

## Implementation

The milestone combines the existing library scanner, metadata reader, media scan service, karaoke pairing service, and statistics service into one workflow.

The program:

- Prompts for a library folder.
- Discovers supported files recursively.
- Separates metadata-capable files from karaoke graphics files.
- Scans MP3, FLAC, and Opus files for metadata.
- Preserves per-file warnings and errors.
- Pairs matching MP3 and CDG karaoke files.
- Generates scan statistics.
- Reports metadata completeness and karaoke pairing results.

CDG files are included in library discovery and karaoke pairing, but are not passed to the metadata reader.

## Example Results

The milestone was tested against a mixed music and karaoke library containing:

- Standard music folders across multiple years.
- Karaoke folders containing matching MP3 and CDG files.

The scan produced:

- 859 supported files discovered.
- 714 metadata-capable files scanned.
- 714 successful metadata scans.
- 145 matched karaoke pairs.
- 569 unmatched MP3 files.
- 0 unmatched CDG files.

The pairing totals matched the expected library structure: the karaoke collection contained 145 MP3/CDG pairs, while the remaining 569 MP3 files belonged to the standard music library.

## Notes

The milestone demonstrated the difference between:

- supported library files
- metadata-capable files
- karaoke pairing files

Materialising scan results with `ToList()` allows the same results to be reused for both display and statistics without rescanning the files.