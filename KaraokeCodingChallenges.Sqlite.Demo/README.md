# SQLite Fundamentals Demo

Demonstrates the SQLite functionality introduced in **Challenge 035 - SQLite Fundamentals**.

The demo creates a temporary SQLite database and exercises the complete CRUD lifecycle.

## Demo Flow

The demo performs the following workflow:

```text
Initialize database
        ↓
Insert song
        ↓
Retrieve song
        ↓
Update song
        ↓
Retrieve updated song
        ↓
Delete song
        ↓
Confirm song no longer exists
```

This demonstrates database initialization, generated identifiers, row retrieval, updates, deletion, and mapping between SQLite rows and `SongRecord` objects.

## Running the Demo

Run the demo with:

```powershell
dotnet run --project KaraokeCodingChallenges.Sqlite.Demo
```

The database is created in the operating system temporary directory rather than inside the repository.

## Example Output

```text
SQLite Fundamentals Demo

Database initialized at:
C:\Users\<user>\AppData\Local\Temp\karaoke-challenge-035-demo.db

Inserted Song
Id: 1
Artist: Paramore
Title: The Only Exception
Year: 2009

Retrieved Song
Id: 1
Artist: Paramore
Title: The Only Exception
Year: 2009

Updated Song: True
Id: 1
Artist: Paramore
Title: Hard Times
Year: 2017

Deleted Song: True
Song no longer exists.
```

The demo provides manual verification of the same CRUD behaviour covered by the automated test project.