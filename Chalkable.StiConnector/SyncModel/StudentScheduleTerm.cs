using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentScheduleTerm
    {
        public int StudentID { get; set; }
        public int SectionID { get; set; }
        public int TermID { get; set; }
        public bool IsEnrolled { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
