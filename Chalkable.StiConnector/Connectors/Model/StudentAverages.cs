
namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentAverages
    {
        /// <summary>
        /// Id of the Alpha Grade for the Average
        /// </summary>
        public int? AlphaGradeId { get; set; }

        /// <summary>
        /// Name of the Alpha Grade.  Examples: "A", "A+", "A-"
        /// </summary>
        public string AlphaGradeName { get; set; }

        /// <summary>
        /// Id of the average
        /// </summary>
        public int AverageId { get; set; }

        /// <summary>
        /// Name of the average
        /// </summary>
        public string AverageName { get; set; }

        /// <summary>
        /// Indicates whether or not the average is exempt for the student
        /// </summary>
        public bool Exempt { get; set; }

        /// <summary>
        /// Id of the grading period that the average occurs in.  For averages setup by a teacher, a grading period will not be set.
        /// </summary>
        public int? GradingPeriodId { get; set; }

        /// <summary>
        /// Indicates whether or not this is the average that is assocaited with the graded item marked as 
        /// DisplayAvgInGradebook.  This should be the main average column in the gradebook.
        /// </summary>
        public bool IsGradingPeriodAverage { get; set; }

        /// <summary>
        /// Indicates whether or not the teacher has overriden the calculated average and manually entered in a score for the average.
        /// </summary>
        public bool ManuallyEntered { get; set; }

        /// <summary>
        /// The numeric value of the average.
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}
