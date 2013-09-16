using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementRecipient
    {
        public Guid Id { get; set; }
        public Guid AnnouncementRef { get; set; }
        public const string ANNOUNCEMENT_REF_FIELD = "announcementRef";

        public bool ToAll { get; set; }
        public int? RoleRef { get; set; }
        public Guid? GradeLevelRef { get; set; }
        public Guid? PersonRef { get; set; }

        [DataEntityAttr]
        public Person Person { get; set; }
    }
}
