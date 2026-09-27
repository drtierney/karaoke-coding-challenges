using KaraokeCodingChallenges.MediaTrack;

namespace KaraokeCodingChallenges.SmartLists;

public static class MediaTrackRules
{
    public static SmartListRule<MediaTrackModel> ArtistContains(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Artist search value cannot be empty.", nameof(value));
        }

        return new SmartListRule<MediaTrackModel>(
            name: $"Artist contains '{value}'",
            predicate: track => track.Artist.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    public static SmartListRule<MediaTrackModel> TitleContains(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Title search value cannot be empty.", nameof(value));
        }

        return new SmartListRule<MediaTrackModel>(
            name: $"Title contains '{value}'",
            predicate: track => track.Title.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    public static SmartListRule<MediaTrackModel> SearchContains(string value)
    {
        return SmartListRules.Any(
            ArtistContains(value),
            TitleContains(value));
    }

    public static SmartListRule<MediaTrackModel> KaraokeOnly()
    {
        return new SmartListRule<MediaTrackModel>(
            name: "Karaoke only",
            predicate: track => track.IsKaraoke);
    }

    public static SmartListRule<MediaTrackModel> MinimumDuration(int duration)
    {
        if (duration < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be at least 0 seconds.");
        }

        return new SmartListRule<MediaTrackModel>(
            name: $"At least {duration} seconds",
            predicate: track => track.DurationSeconds >= duration);
    }

    public static SmartListRule<MediaTrackModel> FilePathContains(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("File path search value cannot be empty.", nameof(value));
        }

        return new SmartListRule<MediaTrackModel>(
            name: $"File path contains '{value}'",
            predicate: track => track.FilePath.Contains(value,StringComparison.OrdinalIgnoreCase));
    }
}
