using System;
using Chalkable.StiConnector.Attributes;

namespace Chalkable.StiConnector.SyncModel
{
    [SisMinVersion("7.3.11.21298")]
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
