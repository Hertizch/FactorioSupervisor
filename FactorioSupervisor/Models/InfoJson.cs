using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FactorioSupervisor.Models
{
    public class InfoJson
    {
        public InfoJson(string version, string description, string homepage, string factorioVersion, JToken author, string name, string title, JToken dependencies)
        {
            Version = version;
            Description = description;
            Homepage = homepage;
            FactorioVersion = factorioVersion;
            Author = author;
            Name = name;
            Title = title;
            Dependencies = dependencies;

            if (author is JArray)
                author.ToObject<List<string>>();

            if (author is JObject)
                author.ToObject<string>();

            if (dependencies is JArray)
                dependencies.ToObject<List<string>>();

            if (dependencies is JObject)
                dependencies.ToObject<string>();
        }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }

        [JsonProperty("factorio_version")]
        public string FactorioVersion { get; set; }

        [JsonProperty("author")]
        public JToken Author { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("dependencies")]
        public JToken Dependencies { get; set; }
    }
}
