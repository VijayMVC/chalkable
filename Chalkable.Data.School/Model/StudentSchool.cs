using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentSchool
    {
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
        public int? CounselorRef { get; set; }
        public bool IsTitle1Eligible { get; set; }
    }
}
