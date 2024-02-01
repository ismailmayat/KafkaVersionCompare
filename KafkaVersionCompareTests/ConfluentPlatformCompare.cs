using FluentAssertions;
using KafkaVersionCompare.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace KafkaVersionCompareTests;

[TestFixture]
public class ConfluentPlatformCompare
{
    [Test]
    public async Task Can_Get_CP_Versions()
    {
        Microsoft.Playwright.Program.Main(new[] { "install" });
        // https://docs.confluent.io/platform/current/release-notes/index.html
        
        // https://docs.confluent.io/platform/{0}/release-notes/index.html
        
        // "/html/body/main//select[@id='version-select']"
        var cpReleaseBuilder = new CPReleaseBuilder(
            "https://docs.confluent.io/platform/current/release-notes/index.html",
            "https://docs.confluent.io/platform/{0}/release-notes/index.html",
            Substitute.For<ILogger<CPReleaseBuilder>>(), Substitute.For<IMemoryCache>());

        var versions = await cpReleaseBuilder.BuildReleaseFromCrawl();

        versions.Count.Should().BePositive();
    }
}