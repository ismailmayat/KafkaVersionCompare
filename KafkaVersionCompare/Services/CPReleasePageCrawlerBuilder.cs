using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using KafkaVersionCompare.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Playwright;

namespace KafkaVersionCompare.Services;

public interface ICPReleaseBuilder
{
    Task<IReadOnlyList<CpRelease>> BuildReleaseFromCrawl();
}

public class CPReleasePageCrawlerBuilder:ICPReleaseBuilder
{
    private readonly string _cpCurrentReleasePage;
    private readonly string _cpReleaseBasePage;
    private readonly CPReleaseParser _cpReleaseParser;
    private readonly ILogger<CPReleasePageCrawlerBuilder> _logger;
    private readonly IMemoryCache _cache;
    private List<CpRelease> _releases = new();
    public CPReleasePageCrawlerBuilder(string cpCurrentReleasePage,string cpReleaseBasePage,CPReleaseParser cpReleaseParser,ILogger<CPReleasePageCrawlerBuilder> logger,IMemoryCache cache)
    {
        _cpCurrentReleasePage = cpCurrentReleasePage;
        _cpReleaseBasePage = cpReleaseBasePage;
        _cpReleaseParser = cpReleaseParser;
        _logger = logger;
        _cache = cache;
    }
    
    public async Task<IReadOnlyList<CpRelease>> BuildReleaseFromCrawl()
    {
        string key = "cp_releases";
        
         var releases = await
             
             _cache.GetOrCreateAsync(key, entry =>
             {
                 entry.SlidingExpiration = TimeSpan.FromDays(1);
                 return Task.Run(Build);
             });
        
         return releases;
         
    }

    private async Task<IReadOnlyList<CpRelease>> Build()
    {
        _logger.LogInformation("running cp crawl");
        
        var htmlContent = await GetPageContentWithJsExecuted();

        var parser = new HtmlParser();
        
        var document = parser.ParseDocument(htmlContent);

        //on the cp releases page get the versions dropdown we can use that to build the list of pages to crawl
        var versionsSelect =
            document.QuerySelector("select#version-select");

        var releasePages = new List<string>();

        if (versionsSelect.HasChildNodes)
        {
            var versions = versionsSelect.ChildNodes.Where(x => !x.Text().Contains("Legacy Docs"));
            
            foreach (var optionNode in versions)
            {
                string version = optionNode.Text().Replace(" (current)", String.Empty);
               
                string releasePageUrl = string.Format(_cpReleaseBasePage, version);
                releasePages.Add(releasePageUrl);
           
            }
        }

        var releases = await BuildReleaseFromCrawl(releasePages);
        
        return releases;
    }
    
    private async Task<IReadOnlyList<CpRelease>> BuildReleaseFromCrawl(IEnumerable<string> releasePageUrls)
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
    
        await crawler.CrawlAsync(new Uri(_cpCurrentReleasePage));
    
        _logger.LogInformation($"created {_releases.Count()} releases");
        
        return _releases;
    }

    private void PageCrawlCompleted(object? sender, PageCrawlCompletedArgs e)
    {
        //url looks like /platform/7.4/release-notes/index.html
        string version = e.CrawledPage.Uri.AbsolutePath.Split('/')[2];

        _logger.LogInformation($"building release for {version} using url {e.CrawledPage.Uri.AbsolutePath}");

        try
        {
            var release = _cpReleaseParser.BuildRelease(e.CrawledPage.AngleSharpHtmlDocument, version);

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

    /// <summary>
    /// gets page content with js exectued 
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetPageContentWithJsExecuted()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(_cpCurrentReleasePage);
        return await page.ContentAsync();
    }
}