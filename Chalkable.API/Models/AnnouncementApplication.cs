using System;
using Chalkable.API.Enums;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class AnnouncementApplication
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("applicationid")]
        public int ApplicationId { get; set; }

        [JsonProperty("announcementid")]
        public int AnnouncementId { get; set; }

        [JsonProperty("announcementtype")]
        public AnnouncementType AnnouncementType { get; set; }

        [JsonProperty("schoolpersonid")]
        public int? SchoolPersonId { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}