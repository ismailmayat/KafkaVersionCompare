using System.Text.RegularExpressions;
using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using KafkaVersionCompare.Model;

namespace KafkaVersionCompare.Services;

public interface IReleaseBuilder
{
    Task<IReadOnlyList<Release>> BuildReleaseFromCrawl();
}

public class ReleasePageCrawlerBuilder : IReleaseBuilder
{
    private readonly string _kafkaReleasePageToCrawl;
    private readonly ReleaseParser _releaseParser;

    private List<Release> _releases = new();

    public ReleasePageCrawlerBuilder(string kafkaReleasePageToCrawl,ReleaseParser releaseParser)
    {
        _kafkaReleasePageToCrawl = kafkaReleasePageToCrawl;
        _releaseParser = releaseParser;
    }
    
    public async Task<IReadOnlyList<Release>> BuildReleaseFromCrawl()
    {
        var pageRequester = new PageRequester(new CrawlConfiguration(), new WebContentExtractor());

        var crawledPage = await pageRequester.MakeRequestAsync(new Uri(_kafkaReleasePageToCrawl));

        string versionRegEx = "^(?:[0-9]{1,3}\\.){3}|[0-9]{1,3}/$";
        
        var anchors = crawledPage.AngleSharpHtmlDocument.Links.Where(l=>Regex.IsMatch(l.InnerHtml,versionRegEx));

        var releasePages = new List<string>(); 

        foreach (var anchor in anchors)
        {
            //builds list of release page urls
            releasePages.Add($"{_kafkaReleasePageToCrawl}{anchor.InnerHtml}RELEASE_NOTES.html");
        }

        return await BuildReleaseFromCrawl(releasePages);
        
    }

    private async Task<IReadOnlyList<Release>> BuildReleaseFromCrawl(IEnumerable<string> releasePageUrls)
    {

        var config = new CrawlConfiguration
        {
            MaxCrawlDepth = 0
        };

        // Create a custom scheduler that is pre-queued with the list
        // of URLs to crawl.
        var scheduler = new UrlScheduler(releasePageUrls);
        
        var crawler = new PoliteWebCrawler(config, null, null, scheduler, null, null, null, null, null);
        
        crawler.PageCrawlCompleted += PageCrawlCompleted;
    
        await crawler.CrawlAsync(new Uri(_kafkaReleasePageToCrawl));
    
        return _releases;
    }
    


    private void PageCrawlCompleted(object? sender, PageCrawlCompletedArgs e)
    {
        //url looks like /dist/kafka/0.10.0.1/RELEASE_NOTES.html
        string version = e.CrawledPage.Uri.AbsolutePath.Split('/')[3];

        var release = _releaseParser.BuildRelease(e.CrawledPage.AngleSharpHtmlDocument, version);

        _releases.Add(release);

    }
}