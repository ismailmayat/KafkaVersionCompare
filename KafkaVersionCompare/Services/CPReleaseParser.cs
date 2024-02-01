using System.Text.RegularExpressions;
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

        if (releaseNotes == null)
        {
            //special case for 6.0 
            releaseNotes = htmlDocument.QuerySelector("div#cp-6-0-0-release-notes");
        }

        var htmlSections = releaseNotes.QuerySelectorAll("div.section").Where(s=>!s.Id.Contains("how-to-download") && !s.Id.Contains("questions"));

        List<CpRelease.Section> sections = new List<CpRelease.Section>();

        foreach (var htmlSection in htmlSections)
        {
            var section = new CpRelease.Section();
            
            var title = htmlSection.QuerySelector("h2");

            string sectionTitle = string.Empty;
            
            if (title == null)
            {
                title = htmlSection.QuerySelector("h3");
            }

            if (title == null)
            {
                sectionTitle = "issue with title - " + htmlSection.Id;
            }
            else
            {
                sectionTitle = RemoveNonPrintableCharacters(title.Text());
            }

            section.SectionTitle = sectionTitle;
            section.SectionBody = RemoveNonPrintableCharacters(htmlSection.OuterHtml);
            
            sections.Add(section);
        }

        var cprelease = new CpRelease(sections);
        
        return cprelease;
    }
    
    static string RemoveNonPrintableCharacters(string input)
    {
        // Use a regular expression to remove non-printable characters
        return Regex.Replace(input, @"[^\u0020-\u007E]", string.Empty);
    }
}