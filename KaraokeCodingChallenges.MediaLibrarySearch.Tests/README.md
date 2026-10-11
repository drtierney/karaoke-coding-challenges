# Media Library Search Tests

Tests the SQLite database-backed search functionality introduced in **Challenge 039 - Database-Backed Search**.

The project contains **32 tests** covering search criteria, title and artist filtering, match modes, case-insensitive matching, SQL wildcard escaping, input validation, and database-backed integration behaviour.

## Search Criteria Tests

`MediaTrackSearchCriteriaTests` contributes **3 tests** covering the search criteria model.

Tests verify that:

- Title and artist default to `null`
- The default match mode is `All`
- Title, artist, and match-mode properties can be assigned

## SQLite Search Service Tests

`SqliteMediaTrackSearchServiceTests` contributes **29 test cases** covering database-backed searching.

### Searching Without Criteria

Tests verify that:

- An empty search criteria object returns all persisted tracks
- Returned tracks retain their expected title and artist values

### Title Searching

Tests verify that:

- A partial title returns matching tracks
- Title searches are case-insensitive for tested ASCII values
- Searches without matches return an empty collection
- Null, empty, and whitespace-only title criteria are ignored

Title casing is tested using multiple `[InlineData]` values.

### Artist Searching

Tests verify that:

- Artist searches return matching tracks
- Multiple tracks belonging to the same artist are returned
- Artist searches are case-insensitive for tested ASCII values

### Search Match Modes

Tests verify that:

- `All` returns tracks matching both title and artist criteria
- `Any` returns tracks matching either title or artist criteria
- Tracks satisfying neither condition are excluded
- Both match modes work when only one criterion is populated
- Undefined match-mode enum values are rejected

The single-criterion tests use a `[Theory]` covering title-only and artist-only searches with both match modes.

### SQL Wildcard Escaping

A parameterized theory verifies literal handling of:

- `%`
- `_`
- `\`

Each character is tested in both title and artist searches, producing **6 test cases**.

Each case inserts a matching record and a similar non-matching record to verify that special characters are not interpreted as unintended SQL wildcards.

The tests exercise escaping through the public search service rather than testing the private helper directly.

### Input Validation

Tests verify that:

- Null connection strings are rejected
- Empty connection strings are rejected
- Whitespace-only connection strings are rejected
- Null search criteria are rejected
- Invalid match-mode values are rejected

## Temporary SQLite Databases

Database-backed tests use `MediaLibrarySearchTestContext`.

The context:

- Generates a unique temporary database path
- Constructs a SQLite connection string
- Initializes the schema using `MediaLibraryDatabaseInitializer`
- Creates library sources using `LibrarySourceRepository`
- Inserts sample tracks using `MediaTrackRepository`
- Deletes the temporary database when disposed

Test database filenames use generated GUIDs to avoid collisions.

Connection pooling is disabled using `Pooling=False` to support reliable database cleanup.

Each integration test uses its own SQLite database, preventing one test's data from affecting another.

## Test Approach

The tests use real SQLite databases rather than mocking database connections or repositories.

This verifies the behaviour of:

- SQL query construction
- SQLite `LIKE` expressions
- Parameterized queries
- `AND` and `OR` conditions
- SQL wildcard escaping
- Database result mapping

The challenge follows a test-first approach, introducing failing tests before implementing or extending the corresponding search functionality.

## Running the Tests

Run the media library search tests with:

```powershell
dotnet test KaraokeCodingChallenges.MediaLibrarySearch.Tests
```

Or run the complete solution test suite with:

```powershell
dotnet test
```

## Test Results

After Challenge 039, the media library search project reports:

```text
Total: 32
Passed: 32
Failed: 0
Skipped: 0
```

The complete solution test suite reports:

```text
Test summary: total: 300, failed: 0, succeeded: 300, skipped: 0
```