using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class CourseStandard : SyncModel
    {
        public int CourseID { get; set; }
        public int StandardID { get; set; }
        public Guid DistrictGuid { get; set; }

        public override int DefaultOrder => 27;
    }
}
