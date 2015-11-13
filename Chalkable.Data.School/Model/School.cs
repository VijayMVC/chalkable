using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class School
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsChalkableEnabled{ get; set; }
        public bool IsLEEnabled { get; set; }
        public bool IsLESyncComplete { get; set; }
    }


    public class ShortSchoolSummary : School
    {
        public int StudentsCount { get; set; }
    }
}