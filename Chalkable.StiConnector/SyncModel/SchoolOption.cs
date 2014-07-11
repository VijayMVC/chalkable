using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.SyncModel
{
    public class SchoolOption
    {
        public int SchoolID { get; set; }
        public bool AllowSectionAverageModification { get; set; }
        public DateTime? EarliestPaymentDate { get; set; }
        public int? NextReceiptNumber { get; set; }
        public short? DefaultCombinationIndex { get; set; }
        public string TimeZoneName { get; set; }
        public short? BaseHoursOffset { get; set; }
        public short? BaseMinutesOffset { get; set; }
        public bool? ObservesDst { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool AllowScoreEntryForUnexcused { get; set; }
        public bool DisciplineOverwritesAttendance { get; set; }
        public bool AllowDualEnrollment { get; set; }
        public string CompleteStudentScheduleDefinition { get; set; }
        public string AveragingMethod { get; set; }
        public bool CategoryAveraging { get; set; }
        public int? StandardsGradingScaleID { get; set; }
        public string StandardsCalculationMethod { get; set; }
        public string StandardsCalculationRule { get; set; }
        public bool StandardsCalculationWeightMaximumValues { get; set; }
        public bool LockCategories { get; set; }
        public bool IncludeReportCardCommentsInGradebook { get; set; }
        public bool MergeRostersForAttendance { get; set; }
    }
}
