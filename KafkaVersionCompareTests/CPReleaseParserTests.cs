using System.Reflection;
using AngleSharp.Html.Parser;
using FluentAssertions;
using KafkaVersionCompare.Model;
using KafkaVersionCompare.Services;

namespace KafkaVersionCompareTests;

public class CPReleaseParserTests
{
     private CpRelease _release;
    
    [SetUp]
    public void Init()
    {
        CPReleaseParser releaseParser;
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get a stream for the HTML file
        Stream stream = assembly.GetManifestResourceStream("KafkaVersionCompareTests.TestData.CPReleasePage60.html");

        string html=string.Empty;
        // Read the HTML file into a string
        using (StreamReader reader = new StreamReader(stream))
        { 
            html = reader.ReadToEnd();
        }
        
        var parser = new HtmlParser();
        
        var document = parser.ParseDocument(html);

        releaseParser = new CPReleaseParser();

        _release = releaseParser.BuildRelease(document,"6.0");
    }

    [Test]
    public void Can_Parse()
    {
        _release.Version.Should().Be(new Version("6.0"));
    }
}