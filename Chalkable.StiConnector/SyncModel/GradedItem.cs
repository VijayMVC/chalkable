namespace Chalkable.StiConnector.SyncModel
{
    public class GradedItem : SyncModel
    {
       public int GradedItemID { get; set; }
       public int GradingPeriodID { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public bool AlphaOnly { get; set; }
       public bool AppearsOnReportCard { get; set; }
       public bool DetGradePoints { get; set; }
       public bool DetGradCredit { get; set; }
       public bool PostToTranscript { get; set; }
       public bool AllowExemption { get; set; }
       public bool DisplayAsAvgInGradebook { get; set; }
       public bool PostRoundedAverage { get; set; }
       public short Sequence { get; set; }
       public char AveragingRule { get; set; }
       public override int DefaultOrder => 46;
    }
}
