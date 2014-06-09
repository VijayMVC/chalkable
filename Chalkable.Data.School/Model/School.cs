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
    }
}