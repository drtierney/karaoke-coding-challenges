# SQLite Fundamentals Tests

Tests the SQLite functionality introduced in **Challenge 035 - SQLite Fundamentals**.

The test suite contains **9 tests** covering database initialization and the complete CRUD lifecycle.

## Database Initialization

- `Initialize_CreatesDatabaseFile`
  - Confirms that initialization creates the SQLite database file.

- `Initialize_CreatesSongsTable`
  - Queries SQLite schema metadata and confirms that the `Songs` table exists.

## Insert

- `AddSong_InsertsSong`
  - Inserts a song.
  - Confirms that a generated ID is returned.
  - Queries the database directly and verifies the stored values.

## Select

- `GetSong_ReturnsSong`
  - Retrieves an existing song by ID.
  - Confirms that all database values are mapped into a `SongRecord`.

- `GetSong_WhenSongDoesNotExist_ReturnsNull`
  - Confirms that requesting an unknown ID returns `null`.

## Update

- `UpdateSong_UpdatesExistingSong`
  - Updates an existing database record.
  - Confirms that the operation reports success.
  - Retrieves the record again and verifies the updated values.

- `UpdateSong_WhenSongDoesNotExist_ReturnsFalse`
  - Confirms that updating an unknown record returns `false`.

## Delete

- `DeleteSong_DeletesExistingSong`
  - Deletes an existing record.
  - Confirms that the operation reports success.
  - Verifies that the deleted song can no longer be retrieved.

- `DeleteSong_WhenSongDoesNotExist_ReturnsFalse`
  - Confirms that deleting an unknown record returns `false`.

## Temporary Databases

Each test creates a uniquely named SQLite database in the operating system temporary directory.

```text
<guid>.db
```

The database is deleted after the test completes.

SQLite connection pooling is disabled so temporary database files can be removed reliably after connections are disposed.

## Running the Tests

Run the SQLite tests with:

```text
dotnet test KaraokeCodingChallenges.Sqlite.Tests
```

Or run the complete solution test suite with:

```text
dotnet test
```

## Test Results

```text
Test summary: total: 9, failed: 0, succeeded: 9, skipped: 0
```