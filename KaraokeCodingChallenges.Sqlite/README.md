# Challenge 035 - SQLite Fundamentals

Introduces SQLite and basic relational database operations in C# using a small standalone database before the wider media library is migrated to relational persistence.

## Concepts Practised

- SQLite
- Relational tables
- Primary keys
- Auto-incrementing identifiers
- `INSERT`
- `SELECT`
- `UPDATE`
- `DELETE`
- Parameterized SQL
- Database connections and commands
- `SqliteDataReader`
- Mapping database rows to C# objects
- Resource disposal

## Implementation

The challenge uses a standalone `SongRecord` model and a single `Songs` table so the focus remains on SQL and SQLite fundamentals rather than the eventual media library database design.

### `SongRecord`

Contains:

- `Id`
- `Artist`
- `Title`
- `Year`

### `SqliteDatabase`

Provides:

- `Initialize()`
- `AddSong(SongRecord song)`
- `GetSong(int id)`
- `UpdateSong(SongRecord song)`
- `DeleteSong(int id)`

## Database Schema

The challenge uses a single `Songs` table:

```sql
CREATE TABLE IF NOT EXISTS Songs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Artist TEXT NOT NULL,
    Title TEXT NOT NULL,
    Year INTEGER NOT NULL
);
```

## SQLite Access

SQLite access is provided through:

- `Microsoft.Data.Sqlite`
- `SqliteConnection`
- `SqliteCommand`
- `SqliteDataReader`

Connections are opened only for the duration of each operation and disposed afterwards.

Connection pooling is disabled for this challenge so temporary database files used by the automated tests can be deleted reliably after each test completes.

## CRUD Operations

`SqliteDatabase` supports the complete CRUD lifecycle:

- `AddSong(SongRecord song)` inserts a song and returns its generated ID.
- `GetSong(int id)` retrieves and maps a database row to a `SongRecord`, or returns `null` when the record does not exist.
- `UpdateSong(SongRecord song)` updates an existing record and returns whether a row was affected.
- `DeleteSong(int id)` removes an existing record and returns whether a row was affected.

`Initialize()` creates the database table when required.

SQL parameters are used when supplying values rather than constructing SQL statements through string interpolation.

## Scope

This project intentionally does not persist the existing karaoke media library.

The full media library schema, relationships, indexes, data access abstractions, and integration with scanned tracks are planned for later persistence challenges.