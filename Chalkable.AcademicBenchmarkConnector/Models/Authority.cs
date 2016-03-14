using System;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class Authority
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
    }
}
