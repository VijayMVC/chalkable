using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class GradingComment
    {
        public const string SCHOOL_ID_FIELD = "SchoolRef";
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolRef { get; set; }
        public string Code { get; set; }
        public string Comment { get; set; }
    }
}
