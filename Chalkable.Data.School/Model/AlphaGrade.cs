using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AlphaGrade
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
