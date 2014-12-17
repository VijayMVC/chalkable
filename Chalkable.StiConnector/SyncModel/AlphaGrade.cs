namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class AlphaGrade
    {
        public int AlphaGradeID { get; set; }
        public int SchoolID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RowVersion { get; set; }
        public int? HonorRollEquivalentId { get; set; }
        public Guid DistrictGuid { get; set; }
        public string StateCode { get; set; }
    }
}
