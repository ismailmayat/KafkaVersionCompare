using Newtonsoft.Json;

namespace KafkaVersionCompare.Model;

 public class Release
    {
        public Release()
        {
            Issues = new List<Issue>();
            SubTask = new List<Issue>();
            Improvement = new List<Issue>();
            Bug = new List<Issue>();
            NewFeature = new List<Issue>();
            Task = new List<Issue>();
            Test = new List<Issue>();
        }
        
        [JsonProperty("version")]
        public Version Version { get; set; }
        
        [JsonProperty("latestRelease")]
        public bool LatestRelease { get; set; }
        
        [JsonProperty("isPatch")]
        public bool IsPatch { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime ReleaseDate { get; set; }


        [JsonProperty("issues")] public IReadOnlyList<Issue> Issues { get; set; } 

        [JsonProperty("subTask")]
        public IReadOnlyList<Issue> SubTask { get; set; }

        [JsonProperty("currentRelease")]
        public bool CurrentRelease { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IReadOnlyList<Issue> Bug { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IReadOnlyList<Issue> Improvement { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IReadOnlyList<Issue> NewFeature { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IReadOnlyList<Issue> Task { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public IReadOnlyList<Issue> Test { get; set; }

        [JsonProperty("categorizedIssues")]
        public List<Category>? CategorizedIssues { get; set; }
        
        public class Issue
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("title")]
            public string? Title { get; set; }

            [JsonProperty("url")]
            public string? Url { get; set; }

        }
        
        public class Category
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public List<string> MatchingLabels { get; set; }

            [JsonProperty("priority")]
            public int Priority { get; set; }

            [JsonProperty("issues")]
            public List<Issue> Issues { get; set; }
        }
    }