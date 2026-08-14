using System;
using System.Collections.Generic;

namespace KaraokeCodingChallenges.MediaTrack
{
    internal class MediaTrack
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public string FilePath { get; set; }

        public int DurationSeconds { get; set; }

        public bool IsKaraoke { get; set; }

        public string DisplayName { get { return $"{Artist} - {Title}"; } }

        public MediaTrack(string title, string artist, string filePath, int durationSeconds = 0, bool isKaraoke = false)
        {
            Title = title;
            Artist = artist;
            FilePath = filePath;
            // May not be known until the file is scanned
            DurationSeconds = durationSeconds;
            IsKaraoke = isKaraoke;
        }


        public string GetFormattedDuration()
        {
            int minutes = DurationSeconds / 60;
            int seconds = DurationSeconds % 60;

            // Return them formatted as m:ss
            return $"{minutes}:{seconds:D2}";
        }

        public override string ToString()
        {
            return $"Title: {Title}\n" +
                   $"Artist: {Artist}\n" +
                   $"FilePath: {FilePath}\n" +
                   $"DurationFormatted: {GetFormattedDuration()}\n" +
                   $"IsKaraoke: {IsKaraoke}";
        }
    }
}
