using Abot2.Core;
using Abot2.Poco;

namespace KafkaVersionCompare.Services;

public class UrlScheduler : Scheduler
{
    /// <summary>
    /// Instantiate the URL queue with list of URLs.
    /// </summary>
    /// <param name="urls"></param>
    public UrlScheduler(IEnumerable<string> urls)
        : base()
    {
        this.Add(urls.Select(url => new PageToCrawl(new Uri(url))));
    }
}