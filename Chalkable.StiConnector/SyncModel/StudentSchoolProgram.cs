using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class StudentSchoolProgram : SyncModel
    {
        public int StudentSchoolProgramID { get; set; }
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public int SchoolProgramID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? DecimalValue { get; set; }
        public override int DefaultOrder => 57;
    }
}
