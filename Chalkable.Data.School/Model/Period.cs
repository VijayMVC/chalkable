using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Period
    {
        public const string ID_FIELD = "Id";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public const string ORDER_FIELD = "Order";
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolYearRef { get; set; }
        public int Order { get; set; }
    }
}
