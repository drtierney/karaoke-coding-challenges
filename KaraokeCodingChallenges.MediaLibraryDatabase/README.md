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

Challenge 036 defines and initializes the relational schema.

The following were intentionally deferred to later persistence challenges:

- Repository and data-access abstractions
- Full CRUD services
- Mapping existing domain models to database rows
- Persisting scan results
- Transactions and upserts
- Database-backed search
- Incremental scanning
- Library reconciliation
- Search-specific indexes

---

# Challenge 037 - Database Data Access Layer

Introduces a repository-based data access layer over the media library SQLite schema created in Challenge 036.

The challenge separates SQL and relational persistence concerns from application logic by introducing database record models, repository interfaces, and SQLite-backed repository implementations.

## Concepts Practised

- Repository/data access pattern
- Interfaces
- Parameterized SQL
- Database record models
- Database-generated identities
- Row-to-object mapping
- Foreign-key relationships
- SQLite boolean representation
- Enum persistence
- CRUD operations
- Separation of concerns

## Database Records

Database-specific record models represent persisted rows while keeping database identity separate from the existing application models.

### `LibrarySourceRecord`

Represents a persisted library source.

Contains:

- `Id`
- `Path`
- `Type`
- `Enabled`

`Id` represents the SQLite-generated database identity.

`Type` reuses the existing `LibrarySourceType` enum:

- `Music`
- `Karaoke`
- `Mixed`

The enum values map directly to the integer values stored by the database schema.

### `MediaTrackRecord`

Represents a persisted media track.

Contains:

- `Id`
- `SourceId`
- `Title`
- `Artist`
- `FilePath`
- `DurationSeconds`
- `IsKaraoke`

`Id` represents the track's database identity.

`SourceId` represents the foreign-key relationship to the owning `LibrarySources` row.

## Repository Interfaces

The data-access contracts are defined through:

- `ILibrarySourceRepository`
- `IMediaTrackRepository`

These interfaces keep repository consumers independent from the SQL used by the SQLite implementations.

### `ILibrarySourceRepository`

Supports:

- Adding a library source
- Retrieving a library source by ID
- Retrieving all library sources
- Updating a library source
- Deleting a library source

### `IMediaTrackRepository`

Supports:

- Adding a media track
- Retrieving a media track by ID
- Retrieving a media track by file path
- Retrieving all media tracks
- Updating a media track
- Deleting a media track

## SQLite Repository Implementations

The repository interfaces are implemented by:

- `LibrarySourceRepository`
- `MediaTrackRepository`

Each repository:

- Accepts a SQLite connection string
- Opens and disposes its own database connections
- Uses parameterized SQL
- Converts database rows into record models
- Returns generated IDs from `INSERT` operations
- Returns `null` when requested records do not exist
- Returns `bool` results from update and delete operations
- Preserves schema constraints and foreign-key behaviour

Generated identities are retrieved using:

```sql
SELECT last_insert_rowid();
```

## Row Mapping

Database rows are mapped into record models inside the repository layer.

For example, SQLite integer values are converted back into:

- `LibrarySourceType` enum values
- C# `bool` values

This keeps SQLite-specific representation details out of repository consumers.

## Parameterized SQL

All repository values are supplied through SQL parameters rather than string interpolation.

For example:

```text
$sourceId
$title
$artist
$filePath
$durationSeconds
$isKaraoke
```

This keeps values separate from SQL commands and avoids embedding application data directly into SQL strings.

## Preserved Database Behaviour

The repositories continue to rely on the constraints defined by Challenge 036.

Examples include:

- Library-source paths remain unique regardless of casing
- Track file paths remain unique regardless of casing
- Tracks must reference an existing library source
- Library sources containing tracks cannot be deleted automatically
- Invalid relational operations continue to raise SQLite errors

The repository layer does not duplicate these rules in application code.

## Scope

Challenge 037 introduces repository-based CRUD access for:

- Library sources
- Media tracks

The following remain intentionally deferred:

- Persisting scan results
- Bulk track persistence
- Transactions spanning scan operations
- Upserts
- Batch operations
- Database-backed search and filtering
- Incremental scanning
- Library reconciliation
- Karaoke companion-file repositories
- Playlist repositories
- Playback-history repositories

These capabilities are introduced when required by later persistence challenges.