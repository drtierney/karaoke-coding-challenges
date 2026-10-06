CREATE TABLE IF NOT EXISTS LibrarySources (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Path TEXT NOT NULL UNIQUE COLLATE NOCASE,
    Type INTEGER NOT NULL CHECK (Type IN (0, 1, 2)),
    Enabled INTEGER NOT NULL CHECK (Enabled IN (0, 1))
);

CREATE TABLE IF NOT EXISTS Tracks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SourceId INTEGER NOT NULL,
    Title TEXT NOT NULL,
    Artist TEXT NOT NULL,
    FilePath TEXT NOT NULL UNIQUE COLLATE NOCASE,
    DurationSeconds INTEGER NOT NULL CHECK (DurationSeconds >= 0),
    IsKaraoke INTEGER NOT NULL CHECK (IsKaraoke IN (0, 1)),
    FOREIGN KEY (SourceId)
        REFERENCES LibrarySources(Id)
);

CREATE TABLE IF NOT EXISTS KaraokeFiles (
    TrackId INTEGER PRIMARY KEY,
    FilePath TEXT NOT NULL UNIQUE COLLATE NOCASE,
    FOREIGN KEY (TrackId)
        REFERENCES Tracks(Id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Playlists (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Guid TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    CHECK (length(trim(Guid)) > 0)
);

CREATE TABLE IF NOT EXISTS PlaylistTracks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlaylistId INTEGER NOT NULL,
    TrackId INTEGER NOT NULL,
    Position INTEGER NOT NULL CHECK (Position >= 0),
    UNIQUE (PlaylistId, Position),
    FOREIGN KEY (PlaylistId)
        REFERENCES Playlists(Id)
        ON DELETE CASCADE,
    FOREIGN KEY (TrackId)
        REFERENCES Tracks(Id)
        ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS PlaybackHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TrackId INTEGER NOT NULL,
    PlayedAt TEXT NOT NULL,
    FOREIGN KEY (TrackId)
        REFERENCES Tracks(Id)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Tracks_SourceId
ON Tracks(SourceId);

CREATE INDEX IF NOT EXISTS IX_PlaylistTracks_PlaylistId
ON PlaylistTracks(PlaylistId);

CREATE INDEX IF NOT EXISTS IX_PlaylistTracks_TrackId
ON PlaylistTracks(TrackId);

CREATE INDEX IF NOT EXISTS IX_PlaybackHistory_TrackId
ON PlaybackHistory(TrackId);

CREATE INDEX IF NOT EXISTS IX_PlaybackHistory_PlayedAt
ON PlaybackHistory(PlayedAt);
