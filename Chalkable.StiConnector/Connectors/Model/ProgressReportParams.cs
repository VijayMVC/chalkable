using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ProgressReportParams
    {
        public int[] AbsenceReasonIds { get; set; }

        public int AcadSessionId { get; set; }

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

        public int GradingPeriodId { get; set; }

        public int IdToPrint { get; set; }

        public decimal? MaxCategoryClassAverage { get; set; }

        public decimal? MaxStandardAverage { get; set; }

        public decimal? MinCategoryClassAverage { get; set; }

        public decimal? MinStandardAverage { get; set; }

        public bool PrintFromHomePortal { get; set; }

        public string SectionComment { get; set; }

        public int SectionId { get; set; }

        public int[] StudentIds { get; set; }

    }
}
