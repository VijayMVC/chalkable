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

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            var o = (Course)obj;
            return Id == o.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
