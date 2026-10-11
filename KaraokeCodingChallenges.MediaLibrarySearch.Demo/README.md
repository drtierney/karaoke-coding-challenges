# Media Library Search Demo

Demonstrates the SQLite database-backed searching introduced in **Challenge 039 - Database-Backed Search**.

The demo creates a temporary SQLite media library, inserts music and karaoke tracks using the existing repository layer, and executes searches using `SqliteMediaTrackSearchService`.

## Demo Flow

The demo performs the following workflow:

```text
Create temporary SQLite database
        ↓
Initialize database schema
        ↓
Create music and karaoke library sources
        ↓
Insert sample media tracks
        ↓
Create database-backed search service
        ↓
Search all tracks
        ↓
Search by title and artist
        ↓
Demonstrate All and Any match modes
        ↓
Demonstrate literal wildcard searching
        ↓
Demonstrate searches without matches
        ↓
Delete temporary database
```

## Sample Media Library

The demo creates two library sources:

- Music
- Karaoke

Six sample tracks are persisted:

| Artist | Title | Type |
|---|---|---|
| Queen | Bohemian Rhapsody | Music |
| Queen | Don't Stop Me Now | Music |
| Paramore | Misery Business | Music |
| The Dandy Warhols | Bohemian Like You | Music |
| Crystal Waters | 100% Pure Love | Music |
| Queen | Bohemian Rhapsody | Karaoke |

The music and karaoke versions of `Bohemian Rhapsody` are stored as separate database records with different file paths.

Sample tracks are inserted using `MediaTrackRepository`, and the demo does not require physical media files to exist.

## Search Demonstrations

The demo executes seven searches:

- All tracks without filters
- Partial title search for `Bohemian`
- Artist search for `Queen`
- `All` matching with title and artist criteria
- `Any` matching with title and artist criteria
- Literal percentage-character search
- Search with no matching records

The searches use `MediaTrackSearchCriteria` and return persisted `MediaTrackRecord` objects.

A shared `PrintResults()` helper displays the matching tracks and distinguishes music from karaoke records.

## Running the Demo

Run the demo from the solution root:

```powershell
dotnet run --project KaraokeCodingChallenges.MediaLibrarySearch.Demo
```

The demo creates a unique SQLite database in the operating system temporary directory.

The connection string uses `Pooling=False`, and database cleanup is performed in a `finally` block.

This allows the temporary database to be deleted after execution, including when an exception occurs during the demonstration.

## Example Output

The database path varies between runs.

```text
Challenge 039 - Database-Backed Search

Database initialized at: <temporary database path>

All Tracks:
  6 matching tracks
  Queen - Bohemian Rhapsody [Music]
  Queen - Don't Stop Me Now [Music]
  Paramore - Misery Business [Music]
  The Dandy Warhols - Bohemian Like You [Music]
  Crystal Waters - 100% Pure Love [Music]
  Queen - Bohemian Rhapsody [Karaoke]

Title Search - Bohemian:
  3 matching tracks
  Queen - Bohemian Rhapsody [Music]
  The Dandy Warhols - Bohemian Like You [Music]
  Queen - Bohemian Rhapsody [Karaoke]

Artist Search - Queen:
  3 matching tracks
  Queen - Bohemian Rhapsody [Music]
  Queen - Don't Stop Me Now [Music]
  Queen - Bohemian Rhapsody [Karaoke]

Match Mode All - Bohemian AND Queen:
  2 matching tracks
  Queen - Bohemian Rhapsody [Music]
  Queen - Bohemian Rhapsody [Karaoke]

Match Mode Any - Bohemian OR Queen:
  4 matching tracks
  Queen - Bohemian Rhapsody [Music]
  Queen - Don't Stop Me Now [Music]
  The Dandy Warhols - Bohemian Like You [Music]
  Queen - Bohemian Rhapsody [Karaoke]

Contains Percentage Search:
  1 matching track
  Crystal Waters - 100% Pure Love [Music]

Unknown Track Search:
  No matching tracks.
```

## Match Mode Behaviour

The `All` search requires both criteria to match.

For example:

```text
Title = Bohemian
Artist = Queen
MatchMode = All
```

This returns the music and karaoke versions of `Bohemian Rhapsody`.

The `Any` search accepts records matching either criterion, additionally returning `Don't Stop Me Now` and `Bohemian Like You`.

## Special Character Searching

The percentage search demonstrates that a literal `%` can be included in search criteria without being interpreted as an SQL wildcard.

For example:

```text
Title = %
```

This returns `100% Pure Love`.

The same escaping behaviour supports literal underscores and backslashes in title and artist searches.

## Database Cleanup

The demo uses a `try/finally` block to ensure database cleanup is attempted when execution finishes.

Database connections are opened and disposed by the existing repositories and search service.

The sample database is temporary and is not intended to preserve data between demo executions.