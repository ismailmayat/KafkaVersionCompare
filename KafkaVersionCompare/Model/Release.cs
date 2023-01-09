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


        [JsonProperty("issues")] 
        public IReadOnlyList<Issue> Issues { get; set; } 

        [JsonProperty("subTasks")]
        public IReadOnlyList<Issue> SubTask { get; set; }

        [JsonProperty("currentReleases")]
        public bool CurrentRelease { get; set; }
        
        [JsonProperty("bug")]
        public IReadOnlyList<Issue> Bug { get; set; }
        
        [JsonProperty("improvements")]
        public IReadOnlyList<Issue> Improvement { get; set; }
        
        [JsonProperty("newFeatures")]
        public IReadOnlyList<Issue> NewFeature { get; set; }
        
        [JsonProperty("tasks")]
        public IReadOnlyList<Issue> Task { get; set; }
        
        [JsonProperty("tests")]
        public IReadOnlyList<Issue> Test { get; set; }
        
        public class Issue
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("title")]
            public string? Title { get; set; }

            [JsonProperty("url")]
            public string? Url { get; set; }

        }
        
    }