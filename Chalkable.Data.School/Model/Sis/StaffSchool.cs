using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class StaffSchool
    {
        [PrimaryKeyFieldAttr]
        public int StaffRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
    }
}
