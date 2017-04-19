using Newtonsoft.Json;

namespace FactorioSupervisor.Models
{
    public class ModListItem
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }
    }
}
