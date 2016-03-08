using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class BaseResource<TData>
    {
        [JsonProperty("data")]
        public TData Data { get; set; }
    }
}
