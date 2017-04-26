using Newtonsoft.Json;

namespace ModsApi.Models
{
    public class Release
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("released_at")]
        public string ReleasedAt { get; set; }

        [JsonProperty("factorio_version")]
        public string FactorioVersion { get; private set; }
    }
}
