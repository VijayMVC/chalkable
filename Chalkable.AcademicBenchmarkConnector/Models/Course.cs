using System;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{

    public class CourseWrapper
    {
        [JsonProperty("course")]
        public Course Course { get; set; }
    }
    public class Course
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }
    }
}
