using KaraokeCodingChallenges.LibraryPersistence;
using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.LibraryPersistence.Tests;

public class MediaLibraryPersistenceMapperTests
{
    [Fact]
    public void ToDto_MapsLibraryTracks()
    {
        string filePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3");

        MediaLibraryCollection library = new();

        library.AddTrack(new MediaTrackModel(
            "Bohemian Rhapsody",
            "Queen",
            filePath,
            354,
            true));

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryDto dto = mapper.ToDto(library);

        Assert.Single(dto.Tracks);

        MediaTrackDto track = dto.Tracks[0];

        Assert.Equal("Bohemian Rhapsody", track.Title);
        Assert.Equal("Queen", track.Artist);
        Assert.Equal(filePath, track.FilePath);
        Assert.Equal(354, track.DurationSeconds);
        Assert.True(track.IsKaraoke);
    }

    [Fact]
    public void ToDto_SetsCurrentVersion()
    {
        MediaLibraryCollection library = new();

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryDto dto = mapper.ToDto(library);

        Assert.Equal(1, dto.Version);
    }

    [Fact]
    public void ToDto_WhenLibraryIsEmpty_ReturnsEmptyTracks()
    {
        MediaLibraryCollection library = new();

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryDto dto = mapper.ToDto(library);

        Assert.Empty(dto.Tracks);
    }

    [Fact]
    public void ToDto_PreservesTrackOrder()
    {
        MediaLibraryCollection library = new();

        library.AddTrack(new MediaTrackModel(
            "Bohemian Rhapsody",
            "Queen",
            Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3")));

        library.AddTrack(new MediaTrackModel(
            "Misery Business",
            "Paramore",
            Path.Combine("Music", "Paramore - Misery Business.mp3")));

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryDto dto = mapper.ToDto(library);

        Assert.Equal("Bohemian Rhapsody", dto.Tracks[0].Title);
        Assert.Equal("Misery Business", dto.Tracks[1].Title);
    }

    [Fact]
    public void FromDto_MapsLibraryTracks()
    {
        string filePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3");

        MediaLibraryDto dto = new()
        {
            Version = MediaLibraryPersistenceMapper.CurrentVersion,
            Tracks =
            [
                new MediaTrackDto
                {
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    FilePath = filePath,
                    DurationSeconds = 354,
                    IsKaraoke = true
                }
            ]
        };

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryCollection library = mapper.FromDto(dto);

        Assert.Single(library.Tracks);

        MediaTrackModel track = library.Tracks[0];

        Assert.Equal("Bohemian Rhapsody", track.Title);
        Assert.Equal("Queen", track.Artist);
        Assert.Equal(filePath, track.FilePath);
        Assert.Equal(354, track.DurationSeconds);
        Assert.True(track.IsKaraoke);
    }

    [Fact]
    public void FromDto_WhenDtoTracksAreEmpty_ReturnsEmptyLibrary()
    {
        MediaLibraryDto dto = new()
        {
            Version = MediaLibraryPersistenceMapper.CurrentVersion
        };

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryCollection library = mapper.FromDto(dto);

        Assert.Empty(library.Tracks);
    }

    [Fact]
    public void FromDto_WhenVersionIsUnsupported_ThrowsNotSupportedException()
    {
        MediaLibraryDto dto = new()
        {
            Version = 2
        };

        MediaLibraryPersistenceMapper mapper = new();

        Assert.Throws<NotSupportedException>(() => mapper.FromDto(dto));
    }

    [Fact]
    public void FromDto_PreservesTrackOrder()
    {
        MediaLibraryDto dto = new()
        {
            Version = MediaLibraryPersistenceMapper.CurrentVersion,
            Tracks =
            [
                new MediaTrackDto
                {
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    FilePath = Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3")
                },
                new MediaTrackDto
                {
                    Title = "Misery Business",
                    Artist = "Paramore",
                    FilePath = Path.Combine("Music", "Paramore - Misery Business.mp3")
                }
            ]
        };

        MediaLibraryPersistenceMapper mapper = new();

        MediaLibraryCollection library = mapper.FromDto(dto);

        Assert.Equal("Bohemian Rhapsody", library.Tracks[0].Title);
        Assert.Equal("Misery Business", library.Tracks[1].Title);
    }
}
