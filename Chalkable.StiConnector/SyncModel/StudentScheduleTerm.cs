using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentScheduleTerm : SyncModel
    {
        public int StudentID { get; set; }
        public int SectionID { get; set; }
        public int TermID { get; set; }
        public bool IsEnrolled { get; set; }
        public Guid DistrictGuid { get; set; }
        public override int DefaultOrder => 34;
    }
}
