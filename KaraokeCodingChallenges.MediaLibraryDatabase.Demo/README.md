# Media Library Database Demo

Demonstrates the SQLite database functionality introduced across:

- **Challenge 036 - Media Library Database Schema**
- **Challenge 037 - Database Data Access Layer**

The demo first initializes and inspects the relational schema before demonstrating repository-based CRUD operations for library sources and media tracks.

## Challenge 036 - Schema Demo

The first part of the demo creates a temporary database, initializes the schema, and inspects the resulting tables, indexes, foreign keys, and configured delete actions.

### Schema Demo Flow

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

## Challenge 037 - Data Access Layer Demo

The second part of the demo uses the repository layer introduced in Challenge 037.

- Creates `LibrarySourceRepository`
- Creates `MediaTrackRepository`
- Adds music and karaoke library sources
- Retrieves persisted library sources
- Adds music and karaoke tracks
- Retrieves persisted tracks
- Retrieves an individual track by ID
- Updates the track duration
- Deletes a track
- Retrieves the remaining tracks to confirm the persisted changes

The Challenge 037 workflow performs CRUD operations without executing SQL directly from the demo.

### Repository Demo Flow

```text
Create repositories
        ↓
Add library sources
        ↓
Retrieve library sources
        ↓
Add media tracks
        ↓
Retrieve media tracks
        ↓
Update a track
        ↓
Delete a track
        ↓
Retrieve remaining tracks
```

## Running the Demo

Run the demo with:

```powershell
dotnet run --project KaraokeCodingChallenges.MediaLibraryDatabase.Demo
```

The database is created in the operating system temporary directory:

```text
karaoke-media-library-demo.db
```

The demo deletes any previous copy before initialization so each run starts with a fresh database.

## Example Output

```text
Media Library Database Demo

Database initialized at:
C:\Users\<user>\AppData\Local\Temp\karaoke-media-library-demo.db

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

Database Data Access Layer

Library Sources

1 - Music - D:\Music
2 - Karaoke - D:\Karaoke

Tracks

1 - Queen - Don't Stop Me Now [Music]
2 - Queen - Bohemian Rhapsody [Karaoke]

Updated track: True
Deleted track: True

Remaining Tracks

1 - Queen - Don't Stop Me Now (210 seconds)
```

## Schema Inspection

The Challenge 036 portion queries SQLite metadata rather than printing hard-coded schema information.

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

Repository operations continue to use the constraints defined by the initialized database schema.