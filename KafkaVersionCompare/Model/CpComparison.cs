namespace KafkaVersionCompare.Model;

public class CpComparison
{
    public Version From { get; }
    public Version To { get; }
    public IReadOnlyCollection<CpRelease> Releases { get; }

    public CpComparison(Version from, Version to, IReadOnlyCollection<CpRelease> releases)
    {
        From = from;
        To = to;
        Releases = releases;
    }
}