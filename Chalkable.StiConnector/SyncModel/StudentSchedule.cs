using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentSchedule
    {
        public int StudentID { get; set; }
        public int SectionID { get; set; }
        public int? InclusionSectionID { get; set; }
        public int? GradingScaleID { get; set; }
        public bool IsEnrolled { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public decimal? CareerTechAverage { get; set; }
        public short? CareerTechStatusId { get; set; }
    }
}
