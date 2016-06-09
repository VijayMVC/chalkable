using System;
using Newtonsoft.Json;
namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class ShortStandard
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
        [JsonProperty("deepest")]
        public string Deepest { get; set; }
        [JsonProperty("level")]
        public short Level { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("parent")]
        public GuidIdDto Parent { get; set; }

        public bool IsDeepest => Deepest == "Y";
        public bool IsActive => Status == "Active";
    }
    public class Standard : ShortStandard
    {
        [JsonProperty("authority")]
        public Authority Authority { get; set; }
        [JsonProperty("document")]
        public Document Document { get; set; }
        [JsonProperty("subject_doc")]
        public SubjectDocument SubjectDocument { get; set; }
    }

    public class GuidIdDto
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
    }
}
