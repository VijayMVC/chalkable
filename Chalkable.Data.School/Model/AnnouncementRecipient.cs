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
        public const string ANNOUNCEMENT_REF_FIELD = "announcementRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        
        public bool ToAll { get; set; }
        public int? RoleRef { get; set; }
        public int? GradeLevelRef { get; set; }
        public int? PersonRef { get; set; }

        [DataEntityAttr]
        public Person Person { get; set; }
    }
}
