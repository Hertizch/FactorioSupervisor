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
    }
}
