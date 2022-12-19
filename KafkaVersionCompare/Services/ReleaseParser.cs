using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using KafkaVersionCompare.Model;

namespace KafkaVersionCompare.Services;

public class ReleaseParser
{

    public const string SubTask = "Sub-task";
    public const string Bug = "Bug";
    public const string Improvement = "Improvement";
    public const string NewFeature = "New Feature";
    public const string Task = "Task";
    public const string Test = "Test";
    

    public Release BuildRelease(IHtmlDocument htmlDocument,string version)
    {
        using var document = htmlDocument;
        
        var h2Nodes = htmlDocument.Body.QuerySelectorAll<IElement>("h2");
        
        var release = new Release
        {
            Version = version,
            SubTask = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(SubTask)).NextElementSibling),
        
            Bug = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(Bug)).NextElementSibling),

            Improvement = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(Improvement)).NextElementSibling),
        
            NewFeature = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(NewFeature)).NextElementSibling),
        
            Task = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(Task)).NextElementSibling),
            
            Test = BuildList(h2Nodes.FirstOrDefault(x => x.Text().Contains(Test)).NextElementSibling)
        };

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