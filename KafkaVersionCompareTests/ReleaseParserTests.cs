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
        ReleaseParser releaseParser;
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get a stream for the HTML file
        Stream stream = assembly.GetManifestResourceStream("KafkaVersionCompareTests.TestData.Release.html");

        string html=string.Empty;
        // Read the HTML file into a string
        using (StreamReader reader = new StreamReader(stream))
        { 
            html = reader.ReadToEnd();
        }
        
        var parser = new HtmlParser();
        
        var document = parser.ParseDocument(html);

        releaseParser = new ReleaseParser();

        _release = releaseParser.BuildRelease(document,"0.10.1");
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

    [Test]
    public void Parse_Should_Give_Improvement()
    {
        _release.Improvement.Count().Should().Be(4);
    }

    [Test]
    public void Parse_Should_Give_New_Feature()
    {
        _release.NewFeature.Count().Should().Be(3);
    }

    [Test]
    public void Parse_Should_Give_Task()
    {
        _release.Task.Count().Should().Be(8);
    }

    [Test]
    public void Parse_Should_Give_Test()
    {
        _release.Test.Count().Should().Be(16);
    }

    [Test]
    public void Can_Parse()
    {
        ReleaseParser releaseParser;
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get a stream for the HTML file
        Stream stream = assembly.GetManifestResourceStream("KafkaVersionCompareTests.TestData.Failing.html");

        string html=string.Empty;
        // Read the HTML file into a string
        using (StreamReader reader = new StreamReader(stream))
        { 
            html = reader.ReadToEnd();
        }
        
        var parser = new HtmlParser();
        
        var document = parser.ParseDocument(html);

        releaseParser = new ReleaseParser();

        Action act = () => releaseParser.BuildRelease(document, "0.10.1");
        act.Should().NotThrow<Exception>();
        
      
    }

}