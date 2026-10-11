# Challenge 039 - Database-Backed Search

Introduces SQLite-backed searching for persisted media tracks, building on the database schema, repository layer, and scan-result persistence introduced in Challenges 036–038.

The challenge moves track searching into the database, allowing matching records to be retrieved directly through parameterized SQL queries without loading the entire media library into memory.

## Concepts Practised

- Database-backed searching
- SQL `SELECT`, `WHERE`, `LIKE`, and `ORDER BY`
- Parameterized SQL queries
- Dynamic SQL condition construction
- Optional search criteria
- Enum-based match modes
- SQL `AND` and `OR` operators
- Case-insensitive partial matching
- SQL wildcard escaping
- `SqliteDataReader`
- Database row-to-object mapping
- Connection and resource disposal
- Input validation

## Implementation

The search functionality is implemented through:

- `MediaTrackSearchCriteria`
- `MediaTrackSearchMatchMode`
- `SqliteMediaTrackSearchService`

The search project reuses the SQLite schema and `MediaTrackRecord` model introduced by the existing media library database project.

### `MediaTrackSearchCriteria`

Represents the optional filters supplied to a database search.

Contains:

- `Title`
- `Artist`
- `MatchMode`

`Title` and `Artist` are nullable strings, allowing searches by either field, both fields, or neither.

When neither field is populated, the search returns all persisted tracks.

Null, empty, and whitespace-only criteria are ignored. Non-empty search values are trimmed before being applied.

### `MediaTrackSearchMatchMode`

Controls how multiple search conditions are combined.

Supports:

- `All` - Every populated criterion must match
- `Any` - At least one populated criterion must match

`All` is the default match mode.

When only one criterion is populated, both match modes produce the same results.

### `SqliteMediaTrackSearchService`

Executes searches against the persisted `Tracks` table.

The service:

- Accepts a SQLite connection string
- Validates the connection string
- Validates search criteria and match-mode values
- Opens and disposes its own SQLite connections
- Builds SQL conditions from populated search criteria
- Executes parameterized queries
- Maps SQLite rows into `MediaTrackRecord` objects
- Returns matching records as `IReadOnlyList<MediaTrackRecord>`
- Orders results by database ID

The service creates a connection for each search rather than retaining a shared open connection.

## Database Search Behaviour

### Searching Without Criteria

When no title or artist is provided, all persisted tracks are returned.

```sql
SELECT
    Id,
    SourceId,
    Title,
    Artist,
    FilePath,
    DurationSeconds,
    IsKaraoke
FROM Tracks
ORDER BY Id;
```

### Partial Matching

Title and artist searches use SQL `LIKE` with surrounding `%` wildcards.

For example, searching for `Bohemian` matches:

- `Bohemian Rhapsody`
- `Bohemian Like You`

SQLite's default `LIKE` behaviour provides case-insensitive matching for ASCII characters.

### Combined Search Criteria

Multiple conditions are collected before constructing the SQL `WHERE` clause.

With `MatchMode.All`:

```sql
WHERE Title LIKE @title ESCAPE '\'
AND Artist LIKE @artist ESCAPE '\'
```

With `MatchMode.Any`:

```sql
WHERE Title LIKE @title ESCAPE '\'
OR Artist LIKE @artist ESCAPE '\'
```

The separator is selected using the supplied match mode and the conditions are combined using `string.Join()`.

This avoids constructing multiple `WHERE` clauses and allows searches with one or more populated criteria.

## SQL Wildcard Escaping

SQLite `LIKE` expressions interpret `%` and `_` as wildcard characters.

The search service escapes user-supplied special characters so they are treated literally rather than as SQL patterns.

`EscapeLikePattern()` handles:

- `\` - Escape character
- `%` - Matches zero or more characters in an unescaped SQL pattern
- `_` - Matches exactly one character in an unescaped SQL pattern

Existing backslashes are escaped first, followed by percentage signs and underscores.

The SQL `ESCAPE` clause identifies the backslash as the escape character.

The service then surrounds the escaped search value with `%` characters to preserve partial matching.

For example, searching for `100%` matches titles containing the literal text `100%`, rather than treating the supplied percentage sign as a wildcard.

The same escaping behaviour applies to title and artist searches.

## Parameterized SQL

Search values are supplied through SQLite parameters:

- `@title`
- `@artist`

User-supplied values are not interpolated directly into SQL statements.

Only predefined SQL condition fragments are combined when constructing the query.

This separates user input from SQL commands while allowing optional search conditions.

## Result Mapping

SQLite query results are read using `SqliteDataReader`.

Each result is mapped into a `MediaTrackRecord` containing:

- `Id`
- `SourceId`
- `Title`
- `Artist`
- `FilePath`
- `DurationSeconds`
- `IsKaraoke`

SQLite integer values representing karaoke status are converted into C# Boolean values.

Results are returned in ascending database ID order.

## Validation

The search service validates:

- Null, empty, and whitespace-only connection strings
- Null search criteria
- Undefined `MediaTrackSearchMatchMode` values

Invalid connection strings are rejected with `ArgumentException`.

Null criteria are rejected with `ArgumentNullException`.

Undefined match modes are rejected with `ArgumentOutOfRangeException`.

A valid search with no matching records returns an empty collection.

## Scope

Challenge 039 introduces database-backed searches for persisted media tracks using optional title and artist criteria.

The following remain outside the scope of this challenge:

- Pagination
- User-selectable sorting
- Additional metadata filters
- Karaoke-only filtering
- Full-text search
- Search-specific indexes and query optimization
- Asynchronous database searches

These capabilities can be introduced in later challenges as the persistent media library evolves.