using Newtonsoft.Json;

namespace KafkaVersionCompare.Model;

public class CpRelease
{
    
    public CpRelease(IEnumerable<Section> sections)
    {
        Sections = sections;
    }
    
    [JsonProperty("version")]
    public Version Version { get; set; }

    public class Section
    {
        [JsonProperty("sectionTitle")]
        public string SectionTitle { get; set; }
    
        [JsonProperty("sectionBody")]
        public string SectionBody { get; set; }

    }
    
    public IEnumerable<Section> Sections { get; }

}