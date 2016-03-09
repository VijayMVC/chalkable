using System;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class Subject
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("broad")]
        public string Broad { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }

    }

    public class SubjectDocument
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
    }
}
