using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;
using System.Text.Json;

namespace KaraokeCodingChallenges.LibraryPersistence.Tests;

public class JsonMediaLibraryStoreTests
{
    [Fact]
    public void Save_CreatesJsonFile()
    {
        string filePath = CreateTempFilePath();

        try
        {
            MediaLibraryCollection library = new();

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            store.Save(filePath, library);

            Assert.True(File.Exists(filePath));
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    [Fact]
    public void Save_WritesLibraryAsJson()
    {
        string filePath = CreateTempFilePath();

        try
        {
            string trackPath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3");

            MediaLibraryCollection library = new();

            library.AddTrack(new MediaTrackModel(
                "Bohemian Rhapsody",
                "Queen",
                trackPath,
                354,
                true));

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            store.Save(filePath, library);

            string json = File.ReadAllText(filePath);

            MediaLibraryDto? dto = JsonSerializer.Deserialize<MediaLibraryDto>(json);

            Assert.NotNull(dto);
            Assert.Equal(MediaLibraryPersistenceMapper.CurrentVersion, dto.Version);

            Assert.Single(dto.Tracks);

            MediaTrackDto track = dto.Tracks[0];

            Assert.Equal("Bohemian Rhapsody", track.Title);
            Assert.Equal("Queen", track.Artist);
            Assert.Equal(trackPath, track.FilePath);
            Assert.Equal(354, track.DurationSeconds);
            Assert.True(track.IsKaraoke);
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    [Fact]
    public void Save_WhenFilePathIsEmpty_ThrowsArgumentException()
    {
        MediaLibraryCollection library = new();

        MediaLibraryPersistenceMapper mapper = new();
        JsonMediaLibraryStore store = new(mapper);

        Assert.Throws<ArgumentException>(() => store.Save("", library));
    }

    [Fact]
    public void Save_WhenLibraryIsNull_ThrowsArgumentNullException()
    {
        MediaLibraryPersistenceMapper mapper = new();
        JsonMediaLibraryStore store = new(mapper);

        Assert.Throws<ArgumentNullException>(() => store.Save("library.json", null!));
    }

    [Fact]
    public void Load_ReadsLibraryFromJson()
    {
        string filePath = CreateTempFilePath();

        try
        {
            string trackPath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3");

            MediaLibraryDto dto = new()
            {
                Version = MediaLibraryPersistenceMapper.CurrentVersion,
                Tracks =
                [
                    new MediaTrackDto
                    {
                        Title = "Bohemian Rhapsody",
                        Artist = "Queen",
                        FilePath = trackPath,
                        DurationSeconds = 354,
                        IsKaraoke = true
                    }
                ]
            };

            string json = JsonSerializer.Serialize(dto);
            File.WriteAllText(filePath, json);

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            MediaLibraryCollection library = store.Load(filePath);

            Assert.Single(library.Tracks);

            MediaTrackModel track = library.Tracks[0];

            Assert.Equal("Bohemian Rhapsody", track.Title);
            Assert.Equal("Queen", track.Artist);
            Assert.Equal(trackPath, track.FilePath);
            Assert.Equal(354, track.DurationSeconds);
            Assert.True(track.IsKaraoke);
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsEmptyLibrary()
    {
        string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "library.json");

        MediaLibraryPersistenceMapper mapper = new();
        JsonMediaLibraryStore store = new(mapper);

        MediaLibraryCollection library = store.Load(filePath);

        Assert.Empty(library.Tracks);
    }

    [Fact]
    public void Load_WhenJsonIsNull_ThrowsInvalidDataException()
    {
        string filePath = CreateTempFilePath();

        try
        {
            File.WriteAllText(filePath, "null");

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            Assert.Throws<InvalidDataException>(() => store.Load(filePath));
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    [Fact]
    public void Load_WhenJsonIsInvalid_ThrowsJsonException()
    {
        string filePath = CreateTempFilePath();

        try
        {
            File.WriteAllText(filePath, "{ invalid json }");

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            Assert.Throws<JsonException>(() => store.Load(filePath));
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    [Fact]
    public void Load_WhenFilePathIsEmpty_ThrowsArgumentException()
    {
        MediaLibraryPersistenceMapper mapper = new();
        JsonMediaLibraryStore store = new(mapper);

        Assert.Throws<ArgumentException>(() => store.Load(""));
    }

    [Fact]
    public void Load_WhenVersionIsUnsupported_ThrowsNotSupportedException()
    {
        string filePath = CreateTempFilePath();

        try
        {
            MediaLibraryDto dto = new()
            {
                Version = MediaLibraryPersistenceMapper.CurrentVersion + 1
            };

            string json = JsonSerializer.Serialize(dto);
            File.WriteAllText(filePath, json);

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            Assert.Throws<NotSupportedException>(() => store.Load(filePath));
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    [Fact]
    public void Constructor_WhenMapperIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new JsonMediaLibraryStore(null!));
    }

    [Fact]
    public void SaveAndLoad_PreservesLibrary()
    {
        string filePath = CreateTempFilePath();

        try
        {
            string queenPath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3");

            string paramorePath = Path.Combine("Music", "Paramore - Misery Business.mp3");

            MediaLibraryCollection originalLibrary = new();

            originalLibrary.AddTrack(new MediaTrackModel(
                "Bohemian Rhapsody",
                "Queen",
                queenPath,
                354,
                true));

            originalLibrary.AddTrack(new MediaTrackModel(
                "Misery Business",
                "Paramore",
                paramorePath,
                199,
                false));

            MediaLibraryPersistenceMapper mapper = new();
            JsonMediaLibraryStore store = new(mapper);

            store.Save(filePath, originalLibrary);

            MediaLibraryCollection loadedLibrary = store.Load(filePath);

            Assert.Equal(2, loadedLibrary.Count);

            AssertTracksEqual(originalLibrary.Tracks[0], loadedLibrary.Tracks[0]);

            AssertTracksEqual(originalLibrary.Tracks[1], loadedLibrary.Tracks[1]);
        }
        finally
        {
            DeleteTempDirectory(filePath);
        }
    }

    private static string CreateTempFilePath()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        return Path.Combine(tempDirectory, "library.json");
    }

    private static void DeleteTempDirectory(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);

        if (directory is not null && Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

    private static void AssertTracksEqual(MediaTrackModel expected, MediaTrackModel actual)
    {
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Artist, actual.Artist);
        Assert.Equal(expected.FilePath, actual.FilePath);
        Assert.Equal(expected.DurationSeconds, actual.DurationSeconds);
        Assert.Equal(expected.IsKaraoke, actual.IsKaraoke);
    }
}
