using System.Collections.Specialized;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using KafkaVersionCompare.Model;

namespace KafkaVersionCompare.Services;

public class CPReleaseParser
{
    public CpRelease BuildRelease(IHtmlDocument htmlDocument, string version)
    {
        var cpRelease = PopulateRelease(htmlDocument);
     
        cpRelease.Version = new Version(version);
        
        return cpRelease;
    }

    private CpRelease PopulateRelease(IHtmlDocument htmlDocument)
    {
        var releaseNotes = htmlDocument.QuerySelector("div#cp-version-release-notes");
        
        var htmlSections = releaseNotes.QuerySelectorAll("div.section").Where(s=>!s.Id.Contains("how-to-download") && !s.Id.Contains("questions"));

        List<CpRelease.Section> sections = new List<CpRelease.Section>();

        foreach (var htmlSection in htmlSections)
        {
            var section = new CpRelease.Section();

            string sectionTitle = string.Empty;

            var title = htmlSection.QuerySelector("h2");
            
            if (title == null)
            {
                title = htmlSection.QuerySelector("h3");
            }


            section.SectionTitle =title.Text();
            section.SectionBody = htmlSection.OuterHtml;
            
            sections.Add(section);
        }

        var cprelease = new CpRelease(sections);
        
        return cprelease;
    }
}