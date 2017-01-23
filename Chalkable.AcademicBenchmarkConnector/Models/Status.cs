using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class Status
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("emsg")]
        public string ErrorMessage { get; set; }
        [JsonProperty("imsg")]
        public string InformationMessage { get; set; }
    }
}
