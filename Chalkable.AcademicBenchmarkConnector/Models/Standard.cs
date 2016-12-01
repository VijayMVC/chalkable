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
        [JsonProperty("seq")]
        public string Seq { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
        [JsonProperty("deepest")]
        public string Deepest { get; set; }
        [JsonProperty("level")]
        public short Level { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("placeholder")]
        public string PlaceHolder { get; set; }
        [JsonProperty("adopt_year")]
        public string AdoptYear { get; set; }
        [JsonProperty("parent")]
        public GuidIdDto Parent { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("date_modified")]
        public DateTime DateModified { get; set; }

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
        [JsonProperty("subject")]
        public Subject Subject { get; set; }
        [JsonProperty("grade")]
        public GradeLevelRange GradeLevel { get; set; }
        [JsonProperty("course")]
        public Course Course { get; set; }
    }

    public class GuidIdDto
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
    }
}
