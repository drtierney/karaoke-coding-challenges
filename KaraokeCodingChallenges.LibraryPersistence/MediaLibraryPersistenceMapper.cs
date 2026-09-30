using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.LibraryPersistence;

public class MediaLibraryPersistenceMapper
{
    public const int CurrentVersion = 1;

    public MediaLibraryDto ToDto(MediaLibraryCollection library)
    {
        MediaLibraryDto libraryDto = new()
        {
            Version = CurrentVersion
        };

        foreach (MediaTrackModel track in library.Tracks)
        {
            MediaTrackDto trackDto = new MediaTrackDto
            {
                Title = track.Title,
                Artist = track.Artist,
                FilePath = track.FilePath,
                DurationSeconds = track.DurationSeconds,
                IsKaraoke = track.IsKaraoke
            };

            libraryDto.Tracks.Add(trackDto);
        }

        return libraryDto;
    }

    public MediaLibraryCollection FromDto(MediaLibraryDto dto)
    {
        MediaLibraryCollection library = new();

        if (dto.Version != CurrentVersion)
        {
            throw new NotSupportedException(
                $"Media library version {dto.Version} is not supported.");
        }

        foreach (MediaTrackDto trackDto in dto.Tracks)
        {
            MediaTrackModel track = new(
                trackDto.Title,
                trackDto.Artist,
                trackDto.FilePath,
                trackDto.DurationSeconds,
                trackDto.IsKaraoke);

            library.AddTrack(track);
        }

        return library;
    }
}
