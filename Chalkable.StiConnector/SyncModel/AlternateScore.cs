namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class AlternateScore
    {
        public int AlternateScoreID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IncludeInAverage { get; set; }
        public decimal? PercentOfMaximumScore { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
