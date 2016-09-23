using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementGroup
    {
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string GROUP_REF_FIELD = "GroupRef";

        [PrimaryKeyFieldAttr]
        public int AnnouncementRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int GroupRef { get; set; }

        [DataEntityAttr]
        public Group Group { get; set; }

        [NotDbFieldAttr]
        public int StudentCount { get; set; }

        [NotDbFieldAttr]
        public IList<Student> Students { get; set; } 
    }
}
