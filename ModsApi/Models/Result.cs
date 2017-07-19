using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModsApi.Models
{
    public class Result
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("releases")]
        public List<Release> Releases { get; private set; }

        [JsonProperty("github_path")]
        public string GithubPath { get; private set; }
    }
}
