using Newtonsoft.Json;

namespace KafkaVersionCompare.Model;

 public class Release
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        
        [JsonProperty("latestRelease")]
        public bool LatestRelease { get; set; }
        
        [JsonProperty("isPatch")]
        public bool IsPatch { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime ReleaseDate { get; set; }
        

        [JsonProperty("issues")]
        public List<Issue>Issues { get; set; }

        [JsonProperty("subTask")]
        public IEnumerable<Issue> SubTask { get; set; }

        [JsonProperty("currentRelease")]
        public bool CurrentRelease { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public int FeatureCount { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int TotalIssueCount { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Issue> Bug { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Issue> Improvement { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Issue> NewFeature { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Issue> Task { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Issue> Test { get; set; }

        [JsonProperty("categorizedIssues")]
        public List<Category> CategorizedIssues { get; set; }
        
        public class Issue
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

        }
        
        public class Category
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public List<string> MatchingLabels { get; set; }

            [JsonProperty("priority")]
            public int Priority { get; set; }

            [JsonProperty("issues")]
            public List<Issue> Issues { get; set; }
        }
    }