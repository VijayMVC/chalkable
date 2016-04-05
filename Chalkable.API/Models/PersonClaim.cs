using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class PersonClaim
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("values")]
        public IList<string> Values { get; set; }
    }
}
