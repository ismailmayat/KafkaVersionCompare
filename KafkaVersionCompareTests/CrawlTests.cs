using FluentAssertions;
using KafkaVersionCompare.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace KafkaVersionCompareTests;

public class CrawlTests
{
    
    [Test]
    public async Task Can_Crawl()
    {
        var crawler = new ReleasePageCrawlerBuilder("https://archive.apache.org/dist/kafka/", new ReleaseParser(),Substitute.For<ILogger<ReleasePageCrawlerBuilder>>(),Substitute.For<IMemoryCache>());

        var versions = await crawler.BuildReleaseFromCrawl();

        versions.Count.Should().BePositive();
    }
}