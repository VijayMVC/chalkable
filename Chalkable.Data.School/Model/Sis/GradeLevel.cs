using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class GradeLevel
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
    }
}
