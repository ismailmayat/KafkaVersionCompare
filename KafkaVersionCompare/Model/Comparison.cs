namespace KafkaVersionCompare.Model;

public class Comparison
{
    public Version From { get; }
    public Version To { get; }
    public IReadOnlyCollection<Release> Releases { get; }

    public Comparison(Version from, Version to, IReadOnlyCollection<Release> releases)
    {
        From = from;
        To = to;
        Releases = releases;
    }
}