using Abot2.Core;
using Abot2.Poco;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using KafkaVersionCompare.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Playwright;

namespace KafkaVersionCompare.Services;

public class CPReleaseBuilder
{
    private readonly string _cpCurrentReleasePage;
    private readonly string _cpReleaseBasePage;
    private readonly ILogger<CPReleaseBuilder> _logger;
    private readonly IMemoryCache _cache;

    public CPReleaseBuilder(string cpCurrentReleasePage,string cpReleaseBasePage,ILogger<CPReleaseBuilder> logger,IMemoryCache cache)
    {
        _cpCurrentReleasePage = cpCurrentReleasePage;
        _cpReleaseBasePage = cpReleaseBasePage;
        _logger = logger;
        _cache = cache;
    }
    
    public async Task<IReadOnlyList<CpRelease>> BuildReleaseFromCrawl()
    {
        string key = "cp_releases";
        
        // var releases = await
        //     
        //     _cache.GetOrCreateAsync(key, entry =>
        //     {
        //         entry.SlidingExpiration = TimeSpan.FromDays(1);
        //         return Task.Run(Build);
        //     });
        //
        // return releases;

        return await Build();
    }

    private async Task<IReadOnlyList<CpRelease>> Build()
    {
        _logger.LogInformation("running cp crawl");

        //need to get page content with js exectued 
        var htmlContent = await GetPageContentWithJsExecuted();

        var parser = new HtmlParser();
        
        var document = parser.ParseDocument(htmlContent);

        //on the cp releases page get the versions dropdown we can use that to build the list of pages to crawl
        var versionsSelect =
            document.QuerySelector("select#version-select");

        var releases = new List<CpRelease>();

        if (versionsSelect.HasChildNodes)
        {
            var versions = versionsSelect.ChildNodes.Where(x => !x.Text().Contains("Legacy Docs"));
            
            foreach (var optionNode in versions)
            {
                var release = new CpRelease
                {
                    Version = new Version(optionNode.Text().Replace(" (current)",String.Empty))
                };
                
                releases.Add(release);
            }
        }

        return releases;
    }

    private async Task<string> GetPageContentWithJsExecuted()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(_cpCurrentReleasePage);
        return await page.ContentAsync();
    }
}