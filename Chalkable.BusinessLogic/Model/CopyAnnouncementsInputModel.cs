using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model
{
    public class AnnouncementToCopyInputModel
    {
        public int AnnouncementId { get; set; }
        public int AnnouncementType { get; set; }
    }
    public class CopyAnnouncementsInputModel
    {
        public int FromClassId { get; set; }
        public int ToClassId { get; set; }
        public DateTime? StartDate { get; set; }
        public IList<AnnouncementToCopyInputModel> Announcements { get; set; }
    }
}
