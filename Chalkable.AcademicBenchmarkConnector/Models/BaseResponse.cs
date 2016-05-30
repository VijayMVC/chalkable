using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class BaseResponse
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }


    public interface IPaginatedResponse<out TResource>
    {
        IEnumerable<TResource> Resources { get; }
        int Offset { get; set; }
        int Limit { get; set; }
        int Took { get; set; }
    }

    public class PaginatedResponse<TResource> : BaseResponse, IPaginatedResponse<TResource>
    {
        [JsonProperty("resources")]
        public IEnumerable<TResource> Resources { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("took")]
        public int Took { get; set; }
    }

    public interface IBaseResource<out TData>
    {
        [JsonProperty("data")]
        TData Data { get; }
    }
    public class BaseResource<TData> : IBaseResource<TData>
    {
        
        [JsonProperty("data")]
        public TData Data { get; set; }
    }
}
