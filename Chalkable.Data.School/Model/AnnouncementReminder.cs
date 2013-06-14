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
        public Guid Id { get; set; }
        public DateTime? RemindDate { get; set; }
        public bool Processed { get; set; }
        public Guid AnnouncementRef { get; set; }
        public int? Before { get; set; }
        public Guid PersonRef { get; set; }
        
        [NotDbFieldAttr]
        public Announcement Announcement { get; set; }
    }
}
