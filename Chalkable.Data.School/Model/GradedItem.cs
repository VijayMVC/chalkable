using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class GradedItem
    {
        public const string GRADING_PERIOD_REF_FIELD = "GradingPeriodRef";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int GradingPeriodRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AlphaOnly { get; set; }
        public bool AppearsOnReportCard { get; set; }
        public bool DetGradePoints { get; set; }
        public bool DetGradeCredit { get; set; }
        public bool PostToTranscript { get; set; }
        public bool AllowExemption { get; set; }
        public bool DisplayAsAvgInGradebook { get; set; }
        public bool PostRoundedAverage { get; set; }
        public int Sequence { get; set; }
        public char AveragingRule { get; set; }
    }
}
