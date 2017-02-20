using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class LimitedEnglish
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("statecode")]
        public string StateCode { get; set; }

        [JsonProperty("sifcode")]
        public string SifCode { get; set; }

        [JsonProperty("ncescode")]
        public string NcesCode { get; set; }
    }
}
