using KaraokeCodingChallenges.LibraryPersistence;
using KaraokeCodingChallenges.MediaLibrary;
using KaraokeCodingChallenges.MediaTrack;

MediaLibraryCollection library = new();

library.AddTrack(new MediaTrackModel(
    "Bohemian Rhapsody",
    "Queen",
    Path.Combine("Music", "Queen - Bohemian Rhapsody.mp3"),
    354,
    true));

library.AddTrack(new MediaTrackModel(
    "Misery Business",
    "Paramore",
    Path.Combine("Music", "Paramore - Misery Business.mp3"),
    199,
    false));

library.AddTrack(new MediaTrackModel(
    "As It Was",
    "Harry Styles",
    Path.Combine("Music", "Harry Styles - As It Was.mp3"),
    167,
    false));

string filePath = Path.Combine(AppContext.BaseDirectory, "library.json");

MediaLibraryPersistenceMapper mapper = new();
JsonMediaLibraryStore store = new(mapper);

store.Save(filePath, library);

Console.WriteLine("Library saved");
Console.WriteLine($"Tracks: {library.Count}");
Console.WriteLine($"File: {filePath}");

library.Clear();

MediaLibraryCollection loadedLibrary = store.Load(filePath);

Console.WriteLine();
Console.WriteLine("Loaded Library");
Console.WriteLine($"Tracks: {loadedLibrary.Count}");
Console.WriteLine();

foreach (MediaTrackModel track in loadedLibrary.Tracks)
{
    string karaokeLabel = track.IsKaraoke ? " [Karaoke]" : "";

    Console.WriteLine($"{track.Artist} - {track.Title}{karaokeLabel} ({track.GetFormattedDuration()})");
}
