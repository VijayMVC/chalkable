namespace Chalkable.StiConnector.SyncModel
{
    public class GradingScale
    {
        public int GradingScaleID { get; set; }
        public int SchoolID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? HomeGradeToDisplay { get; set; }
    }
}