namespace KaraokeCodingChallenges.MediaTrack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaTrack track1 = new MediaTrack("The Only Exception", "Paramore", "F:\\Music\\Paramore - The Only Exception.mp3");
            MediaTrack track2 = new MediaTrack(artist: "Harry Styles", title: "Sign of the Times", filePath: "F:\\Music\\Harry Styles - Sign of the Times.mp3", durationSeconds: 340);
            MediaTrack track3 = new MediaTrack("Something New", "Axwell Λ Ingrosso", "F:\\Music\\Axwell Λ Ingrosso - Something New.mp3", 295, isKaraoke: false);
            MediaTrack track4 = new MediaTrack(filePath: "F:\\Karaoke\\CANDI STANTON - YOUNG HEARTS RUN FREE.mp3", artist: "Candi Stanton", title: "Young Hearts run free", isKaraoke: true, durationSeconds: 241);
            MediaTrack track5 = new MediaTrack("Red red wine", "UB40", "F:\\Karaoke\\UB40 - Red red wine.mp3");

            track1.DurationSeconds = 167;
            track5.DurationSeconds = 189;
            track5.IsKaraoke = true;

            List<MediaTrack> tracks = new List<MediaTrack>() { track1, track2, track3, track4, track5 };

            foreach (MediaTrack track in tracks)
            {
                Console.WriteLine(track);
                Console.WriteLine();
            }
        }
    }
}
