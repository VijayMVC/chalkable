using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class BellSchedule
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolYearRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short TotalMinutes { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public bool UseStartEndTime { get; set; }
    }
}