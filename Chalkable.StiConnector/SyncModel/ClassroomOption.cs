namespace Chalkable.StiConnector.SyncModel
{
    public class ClassroomOption
    {
        public int SectionID { get; set; }
        public string DefaultActivitySortOrder { get; set; }
        public bool GroupByCategory { get; set; }
        public string AveragingMethod { get; set; }
        public bool CategoryAveraging { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool RoundDisplayedAverages { get; set; }
        public bool DisplayAlphaGrade { get; set; }
        public bool DisplayStudentNames { get; set; }
        public bool DisplayMaximumScore { get; set; }
        public int? StandardsGradingScaleID { get; set; }
        public string StandardsCalculationMethod { get; set; }
        public string StandardsCalculationRule { get; set; }
        public bool StandardsCalculationWeightMaximumValues { get; set; }
        public string DefaultStudentSortOrder { get; set; }
        public int SeatingChartRows { get; set; }
        public int SeatingChartColumns { get; set; }
    }
}