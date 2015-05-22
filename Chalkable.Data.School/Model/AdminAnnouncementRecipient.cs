using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AdminAnnouncementRecipient
    {
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        
        public bool ToAll { get; set; }
        public int? Role { get; set; }
        public int? GradeLevelRef { get; set; }
        public int? PersonRef { get; set; }
        public int? SchoolRef { get; set; }

        //[DataEntityAttr]
        //public Person Person { get; set; }
    }
}
