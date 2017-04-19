using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModsApi.Models
{
    public class ApiData
    {
        [JsonProperty("results")]
        public List<Result> Results { get; private set; }
    }
}
