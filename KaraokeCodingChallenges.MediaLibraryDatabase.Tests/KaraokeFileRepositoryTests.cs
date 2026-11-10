using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Tests;

public class KaraokeFileRepositoryTests
{
    [Fact]
    public void GetByTrackId_WhenRelationshipDoesNotExist_ReturnsNull()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        KaraokeFileRepository repository = new KaraokeFileRepository(context.ConnectionString);

        string? path = repository.GetByTrackId(999);

        Assert.Null(path);
    }

    [Fact]
    public void GetByTrackId_WhenRelationshipExists_ReturnsCdgPath()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository mediaTrackRepository = new MediaTrackRepository(context.ConnectionString);

        string mp3Path = Path.Combine("Karaoke", "Paramore - Still into You.mp3");
        string cdgPath = Path.Combine("Karaoke", "Paramore - Still into You.cdg");

        MediaTrackRecord track = new MediaTrackRecord
        {
            SourceId = sourceId,
            Title = "Still into You",
            Artist = "Paramore",
            FilePath = mp3Path,
            DurationSeconds = 216,
            IsKaraoke = true
        };

        long id = mediaTrackRepository.Add(track);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO KaraokeFiles (TrackId, FilePath)
        VALUES ($trackId, $filePath);
        """;

        command.Parameters.AddWithValue("$trackId", id);
        command.Parameters.AddWithValue("$filePath", cdgPath);

        Assert.Equal(1, command.ExecuteNonQuery());

        KaraokeFileRepository karaokeFileRepository = new KaraokeFileRepository(context.ConnectionString);

        string? path = karaokeFileRepository.GetByTrackId(id);

        Assert.Equal(cdgPath, path);
    }

    [Fact]
    public void Upsert_WhenRelationshipDoesNotExist_InsertsRecord()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository mediaTrackRepository = new MediaTrackRepository(context.ConnectionString);

        string mp3Path = Path.Combine("Karaoke", "Paramore - Still into You.mp3");
        string cdgPath = Path.Combine("Karaoke", "Paramore - Still into You.cdg");

        MediaTrackRecord track = new MediaTrackRecord
        {
            SourceId = sourceId,
            Title = "Still into You",
            Artist = "Paramore",
            FilePath = mp3Path,
            DurationSeconds = 216,
            IsKaraoke = true
        };

        long trackId = mediaTrackRepository.Add(track);

        KaraokeFileRepository repository = new KaraokeFileRepository(context.ConnectionString);

        repository.Upsert(trackId, cdgPath);

        string? result = repository.GetByTrackId(trackId);

        Assert.Equal(cdgPath, result);
    }

    [Fact]
    public void Upsert_WhenRelationshipExists_UpdatesFilePath()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository mediaTrackRepository = new MediaTrackRepository(context.ConnectionString);

        string mp3Path = Path.Combine("Karaoke", "Paramore - Still into You.mp3");
        string originalCdgPath = Path.Combine("Karaoke", "Paramore - Still into You.cdg");

        string updatedCdgPath = Path.Combine("Karaoke", "Paramore - Still Into You.cdg");

        MediaTrackRecord track = new MediaTrackRecord
        {
            SourceId = sourceId,
            Title = "Still into You",
            Artist = "Paramore",
            FilePath = mp3Path,
            DurationSeconds = 216,
            IsKaraoke = true
        };

        long trackId = mediaTrackRepository.Add(track);

        KaraokeFileRepository karaokeFileRepository = new KaraokeFileRepository(context.ConnectionString);

        karaokeFileRepository.Upsert(trackId, originalCdgPath);
        karaokeFileRepository.Upsert(trackId, updatedCdgPath);

        string? result = karaokeFileRepository.GetByTrackId(trackId);

        Assert.Equal(updatedCdgPath, result);
    }

    [Fact]
    public void DeleteByTrackId_WhenRelationshipExists_RemovesRelationship()
    {
        using MediaLibraryDatabaseTestContext context = new MediaLibraryDatabaseTestContext();

        using SqliteConnection connection = context.CreateConnection();

        long sourceId = context.InsertLibrarySource(connection);

        MediaTrackRepository mediaTrackRepository = new MediaTrackRepository(context.ConnectionString);

        string mp3Path = Path.Combine("Karaoke", "Paramore - Still into You.mp3");
        string cdgPath = Path.Combine("Karaoke", "Paramore - Still into You.cdg");

        MediaTrackRecord track = new MediaTrackRecord
        {
            SourceId = sourceId,
            Title = "Still into You",
            Artist = "Paramore",
            FilePath = mp3Path,
            DurationSeconds = 216,
            IsKaraoke = true
        };

        long trackId = mediaTrackRepository.Add(track);

        KaraokeFileRepository karaokeFileRepository = new KaraokeFileRepository(context.ConnectionString);

        karaokeFileRepository.Upsert(trackId, cdgPath);

        Assert.Equal(cdgPath, karaokeFileRepository.GetByTrackId(trackId));

        karaokeFileRepository.DeleteByTrackId(trackId);

        Assert.Null(karaokeFileRepository.GetByTrackId(trackId));
        Assert.NotNull(mediaTrackRepository.GetById(trackId));
    }
}
