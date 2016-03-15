using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class PaginatedResponse<TResource> : BaseResponse
    {
        [JsonProperty("resources")]
        public IList<TResource> Resources { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("took")]
        public int Took { get; set; }
    }
}
