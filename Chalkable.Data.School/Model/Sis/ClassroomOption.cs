using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class ClassroomOption
    {
        public const string ID_FIELD = "Id";
        public const string STANDARD_GRADING_SCALE_REF_FIELD = "StandardsGradingScaleRef";

        public const string AVERAGE_METHOD_POINTS = "Points";
        public const string AVERAGE_METHOD_AVERAGE = "Average";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string DefaultActivitySortOrder { get; set; }
        public bool GroupByCategory { get; set; }
        public string AveragingMethod { get; set; }
        public bool CategoryAveraging { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool	DisplayTotalPoints { get; set; }
        public bool RoundDisplayedAverages { get; set; }
        public bool DisplayAlphaGrade { get; set; }
        public bool DisplayStudentNames { get; set; }
        public bool DisplayMaximumScore { get; set; }
        public int? StandardsGradingScaleRef { get; set; }
	    public string StandardsCalculationMethod { get; set; }
	    public string StandardsCalculationRule { get; set; }
	    public bool StandardsCalculationWeightMaximumValues { get; set; }
	    public string DefaultStudentSortOrder { get; set; }
	    public int SeatingChartRows { get; set; }
	    public int SeatingChartColumns { get; set; }

        [NotDbFieldAttr]
        public AveragingMethodTypeEnum AveragingMethodType
        {
            get
            {
                if (CategoryAveraging)
                    return AVERAGE_METHOD_AVERAGE == AveragingMethod
                               ? AveragingMethodTypeEnum.CategoryAverage
                               : AveragingMethodTypeEnum.CategoryPoints;
                return AVERAGE_METHOD_AVERAGE == AveragingMethod
                           ? AveragingMethodTypeEnum.Average
                           : AveragingMethodTypeEnum.Points;

            }
            set
            {
                CategoryAveraging = value == AveragingMethodTypeEnum.CategoryAverage
                                    || value == AveragingMethodTypeEnum.CategoryPoints;
                AveragingMethod = value == AveragingMethodTypeEnum.CategoryPoints
                                  || value == AveragingMethodTypeEnum.Points
                                      ? AVERAGE_METHOD_POINTS 
                                      : AVERAGE_METHOD_AVERAGE;
            }
        }
    }

    public enum AveragingMethodTypeEnum
    {
        Points = 0,
        Average = 1,
        CategoryPoints = 2,
        CategoryAverage = 3
    }

}
