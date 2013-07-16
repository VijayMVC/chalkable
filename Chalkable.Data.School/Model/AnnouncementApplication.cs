using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementApplication
    {
        public Guid Id { get; set; }
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public Guid AnnouncementRef { get; set; }
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public Guid ApplicationRef { get; set; }
        public const string ACTIVE_FIELD = "Active";
        public bool Active { get; set; }
        public int Order { get; set; }
    }
}
