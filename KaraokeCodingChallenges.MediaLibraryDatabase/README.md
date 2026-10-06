# Challenge 036 - Media Library Database Schema

Introduces the relational SQLite schema for the persistent media library, building on the SQLite fundamentals introduced in Challenge 035.

The challenge focuses on database design, normalization, relationships, constraints, and indexes before the data access layer is introduced in Challenge 037.

## Concepts Practised

- Relational database design
- Normalization
- Primary keys
- Foreign keys
- One-to-many relationships
- One-to-one relationships
- Many-to-many relationships
- Referential integrity
- `CHECK` constraints
- `UNIQUE` constraints
- Indexes
- SQLite schema initialization
- Embedded resources
- Foreign-key enforcement

## Implementation

The database schema is defined in:

```text
Sql/CreateSchema.sql
```

The SQL file is embedded into the project as an `EmbeddedResource`, allowing the database project to remain self-contained without requiring a separate schema file to be copied alongside the compiled application.

### `MediaLibraryDatabaseInitializer`

`MediaLibraryDatabaseInitializer`:

- Accepts a SQLite connection string
- Opens the database connection
- Enables SQLite foreign-key enforcement
- Loads the embedded schema SQL
- Creates the database tables and indexes when required

Schema creation uses `CREATE TABLE IF NOT EXISTS` and `CREATE INDEX IF NOT EXISTS`, allowing initialization to be safely repeated against an existing database.

## Database Schema

The schema contains six application tables.

### `LibrarySources`

Represents configured media-library source locations.

Contains:

- `Id`
- `Path`
- `Type`
- `Enabled`

`Id` is an auto-incrementing database identifier.

`Path` is unique using case-insensitive comparison so paths differing only by casing cannot be added twice.

`Type` stores the integer value of the corresponding library-source enum and is restricted to the currently supported values.

`Enabled` is stored as an integer restricted to `0` or `1`.

### `Tracks`

Represents media tracks discovered within a library source.

Contains:

- `Id`
- `SourceId`
- `Title`
- `Artist`
- `FilePath`
- `DurationSeconds`
- `IsKaraoke`

Each track belongs to a valid `LibrarySources` row.

Track file paths are unique using case-insensitive comparison.

`DurationSeconds` cannot be negative and `IsKaraoke` is restricted to `0` or `1`.

### `KaraokeFiles`

Represents the optional companion graphics file associated with a karaoke track.

Contains:

- `TrackId`
- `FilePath`

`TrackId` is both the primary key and a foreign key to `Tracks`, creating a one-to-zero-or-one relationship.

A track can therefore have at most one karaoke companion file.

Deleting the related track automatically deletes its `KaraokeFiles` row.

### `Playlists`

Represents persisted playlists.

Contains:

- `Id`
- `Guid`
- `Name`

`Id` is the internal database identifier.

`Guid` preserves the stable application identity already used by the playlist domain model and must be unique and non-blank.

Playlist names are not required to be unique.

### `PlaylistTracks`

Represents the tracks contained within a playlist.

Contains:

- `Id`
- `PlaylistId`
- `TrackId`
- `Position`

This table creates the many-to-many relationship between `Playlists` and `Tracks`.

Duplicate tracks are deliberately allowed within a playlist, matching the existing playlist behaviour.

`Position` preserves playlist ordering and cannot be negative.

The combination of `PlaylistId` and `Position` must be unique so two entries cannot occupy the same playlist position.

Deleting a playlist automatically deletes its playlist entries.

Deleting a track that is still referenced by a playlist is rejected.

### `PlaybackHistory`

Represents individual track-play events.

Contains:

- `Id`
- `TrackId`
- `PlayedAt`

Multiple history rows can reference the same track.

Playback counts and last-played values can be derived from these history rows rather than stored separately.

Deleting a track automatically removes its playback history.

## Relationships

The core relationships are:

```text
LibrarySources
      │
      └── 1 : many ── Tracks
                         │
                         ├── 1 : 0..1 ── KaraokeFiles
                         │
                         ├── 1 : many ── PlaylistTracks ── many : 1 ── Playlists
                         │
                         └── 1 : many ── PlaybackHistory
```

## Delete Behaviour

Different relationships use different delete rules:

- Deleting a playlist cascades to its `PlaylistTracks` entries.
- Deleting a track referenced by a playlist is restricted.
- Deleting a track cascades to its `KaraokeFiles` row.
- Deleting a track cascades to its `PlaybackHistory` rows.
- Deleting a library source with existing tracks is not automatically cascaded.

This prevents user-managed playlist content from being silently removed while allowing dependent technical data to be cleaned up automatically.

## Indexes

The schema defines the following explicit indexes:

- `IX_Tracks_SourceId`
- `IX_PlaylistTracks_PlaylistId`
- `IX_PlaylistTracks_TrackId`
- `IX_PlaybackHistory_TrackId`
- `IX_PlaybackHistory_PlayedAt`

Additional search-focused indexes such as artist and title indexes are intentionally deferred until database-backed search is introduced in a later challenge.

SQLite also creates internal indexes for primary-key and unique constraints where required.

## Foreign-Key Enforcement

SQLite foreign-key definitions are stored in the schema, but enforcement must be enabled for each connection.

Connections that modify related data therefore execute:

```sql
PRAGMA foreign_keys = ON;
```

This ensures foreign-key, cascade, and restrict behaviour is enforced.

## Scope

This challenge defines and initializes the relational schema only.

The following are intentionally deferred to later persistence challenges:

- Repository and data-access abstractions
- Full CRUD services
- Mapping existing domain models to database rows
- Persisting scan results
- Transactions and upserts
- Database-backed search
- Incremental scanning
- Library reconciliation
- Search-specific indexes
