using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class StudentSchool
    {
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
    }
}
