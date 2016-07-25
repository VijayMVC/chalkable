using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentAnnouncementApplicationMeta
    {
        public const string ANNOUNCEMENT_APPLICATION_REF_FIELD = nameof(AnnouncementApplicationRef);
        public const string STUDENT_REF_FIELD = nameof(StudentRef);

        [PrimaryKeyFieldAttr]
        public int AnnouncementApplicationRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
        public string Text { get; set; }
    }
}
