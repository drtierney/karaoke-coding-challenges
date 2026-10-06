# Media Library Database Schema Demo

Demonstrates the SQLite schema introduced in **Challenge 036 - Media Library Database Schema**.

The demo creates a temporary database, initializes the schema, and inspects the resulting tables, indexes, foreign keys, and configured delete actions defined by the schema.

## Demo Flow

The demo performs the following workflow:

```text
Create temporary SQLite database
        ↓
Initialize database schema
        ↓
Enable foreign-key enforcement
        ↓
Inspect created tables
        ↓
Inspect explicit indexes
        ↓
Inspect foreign-key relationships
        ↓
Display configured delete actions
```

Unlike Challenge 035, this demo does not perform a complete CRUD lifecycle.

Challenge 036 focuses on relational schema design, while database data-access operations are introduced in later challenges.

## Running the Demo

Run the demo with:

```powershell
dotnet run --project KaraokeCodingChallenges.MediaLibraryDatabase.Demo
```

The database is created in the operating system temporary directory:

```text
karaoke-challenge-036-demo.db
```

The demo recreates the database when run so the displayed schema represents a fresh initialization.

## Example Output

```text
Media Library Database Schema Demo

Database initialized at:
C:\Users\<user>\AppData\Local\Temp\karaoke-challenge-036-demo.db

Foreign key enforcement: Enabled

Tables
- KaraokeFiles
- LibrarySources
- PlaybackHistory
- PlaylistTracks
- Playlists
- Tracks

Indexes
- IX_PlaybackHistory_PlayedAt
- IX_PlaybackHistory_TrackId
- IX_PlaylistTracks_PlaylistId
- IX_PlaylistTracks_TrackId
- IX_Tracks_SourceId

Foreign Keys
- Tracks.SourceId -> LibrarySources.Id [ON DELETE NO ACTION]
- KaraokeFiles.TrackId -> Tracks.Id [ON DELETE CASCADE]
- PlaylistTracks.TrackId -> Tracks.Id [ON DELETE RESTRICT]
- PlaylistTracks.PlaylistId -> Playlists.Id [ON DELETE CASCADE]
- PlaybackHistory.TrackId -> Tracks.Id [ON DELETE CASCADE]

Schema initialization complete.
```

## Schema Inspection

The demo queries SQLite metadata rather than printing hard-coded schema information.

Tables and indexes are read from:

```text
sqlite_master
```

Foreign-key relationships are inspected using:

```sql
PRAGMA foreign_key_list(<table>);
```

This demonstrates that the relationships and delete actions displayed by the demo come directly from the initialized SQLite schema.

## Foreign-Key Enforcement

SQLite foreign-key enforcement is enabled for the demo connection using:

```sql
PRAGMA foreign_keys = ON;
```

The demo also queries the setting and confirms that enforcement is enabled before inspecting the schema.
