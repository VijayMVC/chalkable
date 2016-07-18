using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class Attachment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}