using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class GradeLevelWrapper
    {
        [JsonProperty("grade")]
        public GradeLevel GradeLevel { get; set; }
    }

    public class GradeLevel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }
        
        [JsonProperty("lo")]
        public string Low { get; set; }
        [JsonProperty("hi")]
        public string High { get; set; }

        public override bool Equals(object obj)
        {
            var o = (GradeLevel) obj;
            return o.Code == Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }

    public class GradeLevelRange
    {
        [JsonProperty("low")]
        public string Low { get; set; }
        [JsonProperty("high")]
        public string High { get; set; }
    }
}
