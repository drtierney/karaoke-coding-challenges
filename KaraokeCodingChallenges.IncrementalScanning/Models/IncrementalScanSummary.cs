namespace KaraokeCodingChallenges.IncrementalScanning.Models;

public class IncrementalScanSummary
{
    public int Total { get; set; }
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
}
