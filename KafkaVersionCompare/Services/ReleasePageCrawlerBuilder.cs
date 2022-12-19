using System.Text.RegularExpressions;
using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using KafkaVersionCompare.Model;
using Microsoft.Extensions.Caching.Memory;

namespace KafkaVersionCompare.Services;

public interface IReleaseBuilder
{
    Task<IReadOnlyList<Release>> BuildReleaseFromCrawl();
}

public class ReleasePageCrawlerBuilder : IReleaseBuilder
{
    private readonly string _kafkaReleasePageToCrawl;
    private readonly ReleaseParser _releaseParser;
    private readonly ILogger<ReleasePageCrawlerBuilder> _logger;
    private readonly IMemoryCache _cache;
    private List<Release> _releases = new();

    public ReleasePageCrawlerBuilder(string kafkaReleasePageToCrawl,ReleaseParser releaseParser, ILogger<ReleasePageCrawlerBuilder> logger, IMemoryCache cache)
    {
        _kafkaReleasePageToCrawl = kafkaReleasePageToCrawl;
        _releaseParser = releaseParser;
        _logger = logger;
        _cache = cache;
    }
    
    public async Task<IReadOnlyList<Release>> BuildReleaseFromCrawl()
    {
        string key = "releases";
        var releases = await
            
        _cache.GetOrCreateAsync(key, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromDays(1);
            return Task.Run(Build);
        });
       
        return releases;

    }

    private async Task<IReadOnlyList<Release>> Build()
    {
        _logger.LogInformation("running crawl");
        
        var pageRequester = new PageRequester(new CrawlConfiguration(), new WebContentExtractor());

        var crawledPage = await pageRequester.MakeRequestAsync(new Uri(_kafkaReleasePageToCrawl));

        string versionRegEx = "^(?:[0-9]{1,3}\\.){3}|[0-9]{1,3}/$";

        var anchors = crawledPage.AngleSharpHtmlDocument.Links.Where(l => Regex.IsMatch(l.InnerHtml, versionRegEx));

        var releasePages = new List<string>();

        foreach (var anchor in anchors)
        {
            //builds list of release page urls
            releasePages.Add($"{_kafkaReleasePageToCrawl}{anchor.InnerHtml}RELEASE_NOTES.html");
        }

        var releases = await BuildReleaseFromCrawl(releasePages);


        return releases;
    }

    private async Task<IReadOnlyList<Release>> BuildReleaseFromCrawl(IEnumerable<string> releasePageUrls)
    {

        var config = new CrawlConfiguration
        {
            MaxCrawlDepth = 0
        };

        _logger.LogInformation($"processing {releasePageUrls.Count()} release page urls");
        
        // Create a custom scheduler that is pre-queued with the list
        // of URLs to crawl.
        var scheduler = new UrlScheduler(releasePageUrls.OrderByDescending(x=>x));
        
        var crawler = new PoliteWebCrawler(config, null, null, scheduler, null, null, null, null, null);
        
        crawler.PageCrawlCompleted += PageCrawlCompleted;
    
        await crawler.CrawlAsync(new Uri(_kafkaReleasePageToCrawl));
    
        _logger.LogInformation($"created {_releases.Count()} releases");
        
        return _releases;
    }
    
    private void PageCrawlCompleted(object? sender, PageCrawlCompletedArgs e)
    {
        //url looks like /dist/kafka/0.10.0.1/RELEASE_NOTES.html
        string version = e.CrawledPage.Uri.AbsolutePath.Split('/')[3];

        _logger.LogInformation($"building release for {version} using url {e.CrawledPage.Uri.AbsolutePath}");

        try
        {
            var release = _releaseParser.BuildRelease(e.CrawledPage.AngleSharpHtmlDocument, version);

            if (release == null || release.Version == null || release.Version.Major == null)
            {
                _logger.LogWarning($"parsing issue with {e.CrawledPage.Uri.AbsolutePath}");
            }
            else
            {
                _releases.Add(release);  
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"could process {e.CrawledPage.Uri.AbsolutePath} {ex}");
        }

    }
}