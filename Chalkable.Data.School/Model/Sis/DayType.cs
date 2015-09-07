using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class DayType
    {
        public const string NUMBER_FIELD = "Number";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public const string ID_FIELD = "Id";
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public int SchoolYearRef { get; set; }
    }
}
