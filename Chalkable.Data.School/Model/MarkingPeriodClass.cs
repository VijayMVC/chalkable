using System;

namespace Chalkable.Data.School.Model
{
    public class MarkingPeriodClass
    {
        public Guid Id { get; set; }
        public const string CLASS_REF_FIELD = "classRef";
        public Guid ClassRef { get; set; }
        public const string MARKING_PERIOD_REF_FIELD = "markingPeriodRef";
        public Guid MarkingPeriodRef { get; set; }
    }
}
