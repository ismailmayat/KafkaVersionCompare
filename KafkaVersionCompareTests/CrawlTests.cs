using FluentAssertions;
using KafkaVersionCompare.Services;


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
        var crawler = new ReleasePageCrawler("https://archive.apache.org/dist/kafka/");

        var versions = await crawler.BuildReleaseFromCrawl();

        versions.Count.Should().BePositive();
    }
}