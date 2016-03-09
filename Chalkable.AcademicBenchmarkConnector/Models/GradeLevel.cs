using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class GradeLevel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }

        //TODO: think do we need these fields 
        [JsonProperty("lo")]
        public string Low { get; set; }
        [JsonProperty("hi")]
        public string Hight { get; set; }
    }
}
