using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class GradingScale
    {
        public const string SCHOOL_ID_FIELD = "SchoolRef";
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? HomeGradeToDisplay { get; set; }
    }
}
