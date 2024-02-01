using FluentAssertions;
using KafkaVersionCompare.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace KafkaVersionCompareTests;

[TestFixture]
public class ConfluentPlatformCompareTests
{
    [Test]
    public async Task Can_Get_CP_Versions()
    {
        Microsoft.Playwright.Program.Main(new[] { "install" });

        var cpReleaseBuilder = new CPReleasePageCrawlerBuilder(
            "https://docs.confluent.io/platform/current/release-notes/index.html",
            "https://docs.confluent.io/platform/{0}/release-notes/index.html",
            Substitute.For<ILogger<CPReleasePageCrawlerBuilder>>(), Substitute.For<IMemoryCache>());

        var versions = await cpReleaseBuilder.BuildReleaseFromCrawl();

        versions.Count.Should().BePositive();
    }
}