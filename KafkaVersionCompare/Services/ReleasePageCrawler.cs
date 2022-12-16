using System.Text.RegularExpressions;
using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using KafkaVersionCompare.Model;

namespace KafkaVersionCompare.Services;

public class ReleasePageCrawler
{
    private readonly string _kafkaReleasePageToCrawl;

    private List<Release> _releases = new();

    public ReleasePageCrawler(string kafkaReleasePageToCrawl)
    {
        _kafkaReleasePageToCrawl = kafkaReleasePageToCrawl;
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

        var crawler = new PoliteWebCrawler();
        
        crawler.CrawlBag.AllowedOutboundLinks = releasePageUrls;
    
        crawler.PageCrawlCompleted += PageCrawlCompleted;
    
        await crawler.CrawlAsync(new Uri(_kafkaReleasePageToCrawl));
    
        return _releases;
    }
    


    private void PageCrawlCompleted(object? sender, PageCrawlCompletedArgs e)
    {
        string url = e.CrawledPage.Uri.AbsolutePath;
        string releasePattern = "\\d";
        if (Regex.IsMatch(url,releasePattern))
        {
            var version = new Release()
            {
                Version = url
            };
            _releases.Add(version);
        }
    }
}