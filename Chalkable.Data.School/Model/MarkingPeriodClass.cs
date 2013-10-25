using System;

namespace Chalkable.Data.School.Model
{
    public class MarkingPeriodClass
    {
        public const string ID_FIELD = "Id";
        public int Id { get; set; }
        public const string CLASS_REF_FIELD = "ClassRef";
        public int ClassRef { get; set; }
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";
        public int MarkingPeriodRef { get; set; }
    }
}
