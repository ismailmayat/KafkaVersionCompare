using FluentAssertions;
using KafkaVersionCompare.Services;
using Octokit;

namespace KafkaVersionCompareTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Can_Get_Contributors()
    {
        var gitHubServer = new GitHubService(new GitHubClient(new ProductHeaderValue(GitHubService.UserAgent)));

        var contributors = await gitHubServer.GetContributorsAsync();

        contributors.Count.Should().BePositive();
    }
}