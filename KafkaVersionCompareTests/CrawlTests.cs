using FluentAssertions;
using KafkaVersionCompare.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace KafkaVersionCompareTests;

public class CrawlTests
{

    [SetUp]
    public void Setup()
    {

    }


    [Test]
    public async Task Can_Crawl()
    {
        var crawler = new ReleasePageCrawlerBuilder("https://archive.apache.org/dist/kafka/", new ReleaseParser(),Substitute.For<ILogger<ReleasePageCrawlerBuilder>>());

        var versions = await crawler.BuildReleaseFromCrawl();

        versions.Count.Should().Be(64);
    }
}