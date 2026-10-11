
using KaraokeCodingChallenges.Configuration;
using KaraokeCodingChallenges.MediaLibraryDatabase;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;

namespace KaraokeCodingChallenges.MediaLibrarySearch.Tests;

public sealed class MediaLibrarySearchTestContext : IDisposable
{
    public string DatabasePath { get; }
    public string ConnectionString { get; }

    public MediaLibrarySearchTestContext()
    {
        DatabasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

        ConnectionString = $"Data Source={DatabasePath};Pooling=False";

        MediaLibraryDatabaseInitializer initializer = new MediaLibraryDatabaseInitializer(ConnectionString);

        initializer.Initialize();
    }

    public long AddSource()
    {
        LibrarySourceRepository repository = new LibrarySourceRepository(ConnectionString);

        LibrarySourceRecord source = new LibrarySourceRecord
        {
            Path = Path.Combine(Path.GetTempPath(), $"Music-{Guid.NewGuid()}"),
            Type = LibrarySourceType.Music,
            Enabled = true
        };

        return repository.Add(source);
    }

    public long AddTrack(
        long sourceId,
        string title,
        string artist,
        int durationSeconds,
        bool isKaraoke = false)
    {
        MediaTrackRepository repository = new MediaTrackRepository(ConnectionString);

        MediaTrackRecord track = new MediaTrackRecord
        {
            SourceId = sourceId,
            Title = title,
            Artist = artist,
            FilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3"),
            DurationSeconds = durationSeconds,
            IsKaraoke = isKaraoke
        };

        return repository.Add(track);
    }

    public void Dispose()
    {
        if (File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
        }
    }
}
