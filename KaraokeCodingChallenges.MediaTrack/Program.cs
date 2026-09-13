namespace KaraokeCodingChallenges.MediaTrack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MediaTrackModel track1 = new MediaTrackModel("The Only Exception", "Paramore", "F:\\Music\\Paramore - The Only Exception.mp3");
            MediaTrackModel track2 = new MediaTrackModel(artist: "Harry Styles", title: "Sign of the Times", filePath: "F:\\Music\\Harry Styles - Sign of the Times.mp3", durationSeconds: 340);
            MediaTrackModel track3 = new MediaTrackModel("Something New", "Axwell Λ Ingrosso", "F:\\Music\\Axwell Λ Ingrosso - Something New.mp3", 295, isKaraoke: false);
            MediaTrackModel track4 = new MediaTrackModel(filePath: "F:\\Karaoke\\CANDI STANTON - YOUNG HEARTS RUN FREE.mp3", artist: "Candi Stanton", title: "Young Hearts run free", isKaraoke: true, durationSeconds: 241);
            MediaTrackModel track5 = new MediaTrackModel("Red red wine", "UB40", "F:\\Karaoke\\UB40 - Red red wine.mp3");

            track1.DurationSeconds = 167;
            track5.DurationSeconds = 189;
            track5.IsKaraoke = true;

            List<MediaTrackModel> tracks = new List<MediaTrackModel>() { track1, track2, track3, track4, track5 };

            foreach (MediaTrackModel track in tracks)
            {
                Console.WriteLine(track);
                Console.WriteLine();
            }
        }
    }
}
