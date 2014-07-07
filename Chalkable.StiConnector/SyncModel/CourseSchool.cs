using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class CourseSchool
    {
        public int CourseID { get; set; }
        public int SchoolID { get; set; }
        public Guid RowVersion { get; set; }
    }
}
