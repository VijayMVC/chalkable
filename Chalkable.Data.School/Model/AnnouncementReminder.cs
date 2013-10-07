using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementReminder
    {
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public const string REMIND_DATE_FIELD = "RemindDate";
        public DateTime? RemindDate { get; set; }
        public const string PROCESSED_FIELD = "Processed";
        public bool Processed { get; set; }
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public Guid AnnouncementRef { get; set; }
        public int? Before { get; set; }
        public Guid? PersonRef { get; set; }
        
        [NotDbFieldAttr]
        public Announcement Announcement { get; set; }
    }
}
