namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class ActivityCategory
    {
        public int ActivityCategoryID { get; set; }
        public int SectionID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Percentage { get; set; }
        public byte HighScoresToDrop { get; set; }
        public byte LowScoresToDrop { get; set; }
        public Guid DistrictGuid { get; set; }
        public int? GradeBookCategoryId { get; set; }
    
    }
}
