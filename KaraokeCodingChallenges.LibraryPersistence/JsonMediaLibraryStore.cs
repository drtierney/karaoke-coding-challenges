using System.Text.Json;
using KaraokeCodingChallenges.MediaLibrary;

namespace KaraokeCodingChallenges.LibraryPersistence;

public class JsonMediaLibraryStore
{
    private readonly MediaLibraryPersistenceMapper _mapper;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public JsonMediaLibraryStore(MediaLibraryPersistenceMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        _mapper = mapper;
    }

    public void Save(string filePath, MediaLibraryCollection library)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(library);

        MediaLibraryDto dto = _mapper.ToDto(library);

        string json = JsonSerializer.Serialize(dto, JsonOptions);

        File.WriteAllText(filePath, json);
    }

    public MediaLibraryCollection Load(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            return new MediaLibraryCollection();
        }

        string json = File.ReadAllText(filePath);

        MediaLibraryDto dto = JsonSerializer.Deserialize<MediaLibraryDto>(json) ?? throw new InvalidDataException("Failed to load the media library.");

        return _mapper.FromDto(dto);
    }
}
