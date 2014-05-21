using System;

namespace Chalkable.Data.School.Model
{
    public class SchoolOption
    {
        public int Id { get; set; }
        public bool AllowSectionAverageModification { get; set; }
        public DateTime? EarliestPaymentDate { get; set; }
        public int? NextReceiptNumber { get; set; }
        public int? DefaultCombinationIndex { get; set; }
        public string TimeZoneName { get; set; }
        public int? BaseHoursOffset { get; set; }
        public int? BaseMinutesOffset { get; set; }
        public bool? ObservesDst { get; set; }
        public bool AllowScoreEntryForUnexcused { get; set; }
        public bool DisciplineOverwritesAttendance { get; set; }
        public bool AllowDualEnrollment { get; set; }
        public string CompleteStudentScheduleDefinition { get; set; }
        public string AveragingMethod { get; set; }
        public bool CategoryAveraging { get; set; }
        public int? StandardsGradingScaleRef { get; set; }
        public string StandardsCalculationMethod { get; set; }
        public string StandardsCalculationRule { get; set; }
        public bool StandardsCalculationWeightMaximumValues { get; set; }
        public bool LockCategories { get; set; }
        public bool IncludeReportCardCommentsInGradebook { get; set; }
        public bool MergeRostersForAttendance { get; set; }
    }
}
