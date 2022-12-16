using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using KafkaVersionCompare.Model;

namespace KafkaVersionCompare.Services;

public class ReleaseParser
{
    private readonly IHtmlDocument _htmlDocument;
    public const string SubTask = "Sub-task";
    public const string Bug = "Bug";
    public ReleaseParser(IHtmlDocument htmlDocument)
    {
        _htmlDocument = htmlDocument;
    }

    public Release BuildRelease()
    { 
        var release = new Release();

        var h2Nodes = _htmlDocument.Body.QuerySelectorAll<IElement>("h2");
            
        release.SubTask = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(SubTask)).NextElementSibling);

        release.Bug = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(Bug)).NextElementSibling);
            
        return release;
    }

    private List<Release.Issue> BuildList(IElement ul)
    {
        var tasks = new List<Release.Issue>();
            
        foreach (var li in ul.Children)
        {
            tasks.Add(new Release.Issue
            {
                Id = li.FirstElementChild.Text(),
                Title = li.TextContent,
                Url = li.FirstElementChild.Attributes.GetNamedItem("href").Value
            });   
        }

        return tasks;
    }
}