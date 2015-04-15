using System;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class AttendanceProfileReportParams
    {
        public int AcadSessionId { get; set; }
        public int[] Terms { get; set; }
        public int? StudentFilterId { get; set; }
        public int? UserId { get; set; }

        public int[] AbsenceReasons { get; set; }

        public bool DisplayPeriodAbsences { get; set; }
        public bool DisplayNote { get; set; }
        public bool DisplayReasonTotals { get; set; }
        public bool IncludeUnlisted { get; set; }


        ///<summary>
        /// enum GroupByMethod
        ///    BlankColumn = 0,
        ///    GradeLevelSequence = 1,
        ///    HomeroomName = 3
        ///</summary>
        public int GroupBy { get; set; }
        public int? StudentId { get; set; }
        public string Header { get; set; }

        public int IdToPrint { get; set; }
        public bool DisplayWithdrawnStudents { get; set; }
        public string StudentIdList { get; set; }
        public int? SectionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int[] StudentIds { get; set; }        

    }
}
