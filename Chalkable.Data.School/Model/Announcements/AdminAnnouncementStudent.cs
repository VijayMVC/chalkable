using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public class AdminAnnouncementStudent
    {
        [PrimaryKeyFieldAttr]
        public int AdminAnnouncementRef { get; set; }

        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }

        [DataEntityAttr]
        public Student Student { get; set; }
    }
}
