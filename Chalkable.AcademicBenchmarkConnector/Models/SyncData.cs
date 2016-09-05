using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class SyncData
    {
        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }
        [JsonProperty("guid")]
        public Guid StandardId { get; set; }
        [JsonProperty("whats_changed")]
        public string ChangeType { get; set; }
    }
}
