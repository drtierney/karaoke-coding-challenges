namespace KaraokeCodingChallenges.IncrementalScanning.Models
{
    public class FileFingerprint
    {
        public required string FilePath { get; set; }

        public long FileSize { get; set; }

        public DateTime LastModifiedUtc { get; set; }
    }
}
