using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Homeroom
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolYearRef { get; set; }
        public string Name { get; set; }
        public int? TeacherRef { get; set; }
        public int? RoomRef { get; set; }
    }
}
