namespace Chalkable.StiConnector.SyncModel
{
    public class GradingScaleRange : SyncModel
    {
        public int GradingScaleID { get; set; }
        public int AlphaGradeID { get; set; }
        public decimal LowValue { get; set; }
        public decimal HighValue { get; set; }
        public int AveragingEquivalent { get; set; }
        public bool AwardGradCredit { get; set; }
        public bool IsPassing { get; set; }
        public override int DefaultOrder => 40;
    }
}