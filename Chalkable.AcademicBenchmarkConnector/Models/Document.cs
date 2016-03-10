using System;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class DocumentWrapper
    {
        [JsonProperty("document")]
        public Document Document { get; set; }
    }
    public class Document
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("acronym")]
        public string Code { get; set; }
    }
}
