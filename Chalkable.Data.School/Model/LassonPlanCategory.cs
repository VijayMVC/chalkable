using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class LassonPlanCategory
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
