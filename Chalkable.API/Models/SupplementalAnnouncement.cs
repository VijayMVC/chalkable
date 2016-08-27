using System;
using System.Collections.Generic;

namespace Chalkable.API.Models
{
    public class SupplementalAnnouncement : ShortAnnouncement
    {
        public DateTime? ExpiresDate { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public bool HideFromStudents { get; set; }
        public IList<SchoolPerson> Recipients { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }
    }
}
