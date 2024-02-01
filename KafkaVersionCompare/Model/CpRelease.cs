using Newtonsoft.Json;

namespace KafkaVersionCompare.Model;

public class CpRelease
{
    [JsonProperty("version")]
    public Version Version { get; set; }
}