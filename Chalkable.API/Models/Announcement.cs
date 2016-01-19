using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class StudentAnnouncement
    {
        [JsonProperty("studentinfo")]
        public SchoolPerson StudentInfo { get; set; }
    }

    public class StudentAnnouncements
    {
        [JsonProperty("items")]
        public IEnumerable<StudentAnnouncement> Items { get; set; }
    }

    public class Announcement
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("announcementtypename")]
        public string AnnouncementTypeName { get; set; }

        [JsonProperty("personid")]
        public int PersonId { get; set; }

        [JsonProperty("personname")]
        public string PersonName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("classid")]
        public int? ClassId { get; set; }

        [JsonProperty("classname")]
        public string ClassName { get; set; }

        [JsonProperty("studentannouncements")]
        public StudentAnnouncements StudentAnnouncements { get; set; }
    }
}
