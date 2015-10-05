using System.Collections.Generic;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class ProgressReportInputModel : BaseReportInputModel
    {
        public IntList AbsenceReasonIds { get; set; }
        public bool AdditionalMailings { get; set; }

        public int DailyAttendanceDisplayMethod { get; set; }
        public bool DisplayCategoryAverages { get; set; }
        public bool DisplayClassAverages { get; set; }
        public bool DisplayLetterGrade { get; set; }
        public bool DisplayPeriodAttendance { get; set; }
        public bool DisplaySignatureLine { get; set; }
        public bool DisplayStudentComments { get; set; }
        public bool DisplayStudentMailingAddress { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool GoGreen { get; set; }

        public decimal? MaxCategoryClassAverage { get; set; }
        public decimal? MaxStandardAverage { get; set; }
        public decimal? MinCategoryClassAverage { get; set; }
        public decimal? MinStandardAverage { get; set; }
        public bool PrintFromHomePortal { get; set; }

        public string ClassComment { get; set; }
        public IList<StudentCommentInputModel> StudentComments { get; set; }
    }

    public class StudentCommentInputModel
    {
        public int StudentId { get; set; }
        public string Comment { get; set; }
    }

}
