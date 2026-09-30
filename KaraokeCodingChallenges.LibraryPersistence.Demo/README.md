# Library Persistence Demo

Demonstrates the JSON library persistence functionality introduced in **Challenge 034 - JSON Library Persistence**.

The demo creates a media library, saves it to JSON, clears the in-memory library, and reloads the persisted data from disk.

## Demo Flow

The demo performs the following workflow:

```text
Create MediaLibraryCollection
        ↓
Add media tracks
        ↓
Save library.json
        ↓
Clear in-memory library
        ↓
Load library.json
        ↓
Display restored tracks
```

This demonstrates that the media library can be reconstructed from persisted data without rescanning the original media sources.

## Running the Demo

Run the demo with:

```powershell
dotnet run --project KaraokeCodingChallenges.LibraryPersistence.Demo
```

The generated `library.json` file is written to the demo application's output directory.

## Example Output

```text
Library saved
Tracks: 3
File: ...\KaraokeCodingChallenges.LibraryPersistence.Demo\bin\Debug\net10.0\library.json

Loaded Library
Tracks: 3

Queen - Bohemian Rhapsody [Karaoke] (5:54)
Paramore - Misery Business (3:19)
Harry Styles - As It Was (2:47)
```

The restored tracks retain their title, artist, file path, duration, karaoke status, and original ordering.

## Example JSON

The generated persistence file uses the versioned JSON format:

```json
{
  "Version": 1,
  "Tracks": [
    {
      "Title": "Bohemian Rhapsody",
      "Artist": "Queen",
      "FilePath": "Music\\Queen - Bohemian Rhapsody.mp3",
      "DurationSeconds": 354,
      "IsKaraoke": true
    },
    {
      "Title": "Misery Business",
      "Artist": "Paramore",
      "FilePath": "Music\\Paramore - Misery Business.mp3",
      "DurationSeconds": 199,
      "IsKaraoke": false
    },
    {
      "Title": "As It Was",
      "Artist": "Harry Styles",
      "FilePath": "Music\\Harry Styles - As It Was.mp3",
      "DurationSeconds": 167,
      "IsKaraoke": false
    }
  ]
}
```