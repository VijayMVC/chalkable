
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class MarkingPeriodClass
    {
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";

        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int MarkingPeriodRef { get; set; }
        public int SchoolRef { get; set; }
    }
}
