using System;
using System.Linq;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class ComprehensiveProgressInputModel : BaseReportInputModel
    {
        public IntList GradingPeriodIds { get; set; }
        public IntList AbsenceReasonIds { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }

        public decimal? MaxStandardAverage { get; set; }
        public decimal? MinStandardAverage { get; set; }

        public bool AdditionalMailings { get; set; }
        public bool ClassAverageOnly { get; set; }
        public bool DisplayCategoryAverages { get; set; }
        public bool DisplayClassAverage { get; set; }
        public int DailyAttendanceDisplayMethod { get; set; }
        public bool DisplayPeriodAttendance { get; set; }
        public bool DisplaySignatureLine { get; set; }
        public bool DisplayStudentComment { get; set; }
        public bool DisplayStudentMailingAddress { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool IncludePicture { get; set; }
        public bool IncludeWithdrawn { get; set; }

        public int? StudentFilterId { get; set; }
        public bool GoGreen { get; set; }
        public int OrderBy { get; set; }
        public bool WindowEnvelope { get; set; }


        public override int GradingPeriodId
        {
            get
            {
                if (GradingPeriodIds != null && GradingPeriodIds.Count > 0)
                    return GradingPeriodIds.First();
                return base.GradingPeriodId;
            }
            set
            {
                base.GradingPeriodId = value;
            }
        }
    }

    public enum ComprehensiveProgressOrderByMethod
    {
        StudentDisplayName = 1,
        StudentIdentifier = 2,
        GradeLevel = 3,
        Homeroom = 4,
        PostalCode = 5,
        DistributionPeriod = 6,
    }

    public enum ProgressAttendanceDisplayMethod
    {
        None,
        Both,
        GradingPeriod,
        YearToDate,
    }
}
