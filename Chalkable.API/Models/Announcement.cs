using System;
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

    public class ShortAnnouncement
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("announcementtypename")]
        public string AnnouncementTypeName { get; set; }

        [JsonProperty("personid")]
        public int? PersonId { get; set; }

        [JsonProperty("personname")]
        public string PersonName { get; set; }

        [JsonProperty("persongender")]
        public string PersonGender { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("isowner")]
        public bool IsOwner { get; set; }
    }

    public class Announcement : ShortAnnouncement
    {
        [JsonProperty("classid")]
        public int? ClassId { get; set; }

        [JsonProperty("classname")]
        public string ClassName { get; set; }
        
        [JsonProperty("classannouncementdata")]
        public ClassAnnouncement ClassAnnouncement { get; set; }

        [JsonProperty("lessonplandata")]
        public LessonPlan LessonPlan { get; set; }

        [JsonProperty("adminannouncementdata")]
        public AdminAnnouncement AdminAnnouncement { get; set; }

        [JsonProperty("supplementalannouncementdata")]
        public SupplementalAnnouncement SupplementalAnnouncement { get; set; }

        [JsonProperty("studentannouncements")]
        public StudentAnnouncements StudentAnnouncements { get; set; }
    }


    public class AnnouncementApplicationRecipient
    {
        [JsonProperty("announcementappicationid")]
        public int AnnouncementAppicationId { get; set; }
        [JsonProperty("classname")]
        public string ClassName { get; set; }
        [JsonProperty("studentcount")]
        public int? StudentCount { get; set; }
    }
}
