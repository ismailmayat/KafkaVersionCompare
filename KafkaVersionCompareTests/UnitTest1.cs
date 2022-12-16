using FluentAssertions;
using KafkaVersionCompare.Services;
using Octokit;

namespace KafkaVersionCompareTests;

public class Tests
{
    private GitHubService _gitHubService;
    [SetUp]
    public void Setup()
    {
        _gitHubService = new GitHubService(new GitHubClient(new ProductHeaderValue(GitHubService.UserAgent)));
    }

    [Test]
    public async Task Can_Get_Contributors()
    {
        

        var contributors = await _gitHubService.GetContributorsAsync();

        contributors.Count.Should().BePositive();
    }

    [Test]
    public async Task Can_Get_Labels()
    {
        var labels = await _gitHubService.GetLabelsAsync();

        labels.Count.Should().BePositive();
    }

    [Test]
    public async Task Can_Crawl()
    {
        var crawler = new ReleasePageCrawler("https://archive.apache.org/dist/kafka/");

        var versions = await crawler.BuildReleaseFromCrawl();

        versions.Count.Should().BePositive();
    }
}