using System.Reflection;
using AngleSharp.Html.Parser;
using FluentAssertions;
using KafkaVersionCompare.Model;
using KafkaVersionCompare.Services;

namespace KafkaVersionCompareTests;

public class ReleaseParserTests
{
    private Release _release;
    
    [SetUp]
    public void Init()
    {
        string Html=string.Empty;

        ReleaseParser releaseParser;
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get a stream for the HTML file
        Stream stream = assembly.GetManifestResourceStream("KafkaVersionCompareTests.TestData.Release.html");

        string html=string.Empty;
        // Read the HTML file into a string
        using (StreamReader reader = new StreamReader(stream))
        { 
            Html = reader.ReadToEnd();
        }
        
        var parser = new HtmlParser();
        
        var document = parser.ParseDocument(Html);

        releaseParser = new ReleaseParser(document);

        _release = releaseParser.BuildRelease();
    }

    [Test]
    public void Parse_Should_Give_SubTasks()
    {
        _release.SubTask.Count().Should().Be(5);
    }

    [Test]
    public void Parse_Should_Give_Bug()
    {
        _release.Bug.Count().Should().Be(7);
    }

}