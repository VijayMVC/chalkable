using System;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class SubjectWrapper
    {
        [JsonProperty("subject")]
        public Subject Subject { get; set; }
    }

    public class Subject
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("broad")]
        public string Broad { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }

    }

    public class SubjectDocumentWrapper
    {
        [JsonProperty("subject_doc")]
        public SubjectDocument SubjectDocument { get; set; }
    }

    public class SubjectDocument
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
    }
}
