
using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentAverage
    {

        /// <summary>
        /// Id of the average
        /// </summary>
        public int AverageId { get; set; }

        /// <summary>
        /// Name of the average
        /// </summary>
        public string AverageName { get; set; }

        /// <summary>
        /// Id of the Alpha Grade for the calculated average.  This value cannot be updated.
        /// </summary>
        public int? CalculatedAlphaGradeId { get; set; }

        /// <summary>
        /// Name of the Alpha Grade for the calculated average.  Examples: "A", "A+", "A-".  This value cannot be updated.
        /// </summary>
        public string CalculatedAlphaGradeName { get; set; }

        /// <summary>
        /// The numeric value of the calculated average.  This value cannot be updated.
        /// </summary>
        public decimal? CalculatedNumericAverage { get; set; }

        /// <summary>
        /// A list of comments for the average
        /// </summary>
        public IList<StudentAverageComment> Comments { get; set; }

        /// <summary>
        /// Id of the Alpha Grade for the manually entered average.  This value cannot be updated.
        /// </summary>
        public int? EnteredAlphaGradeId { get; set; }

        /// <summary>
        /// Name of the Alpha Grade for the manually entered average.  Examples: "A", "A+", "A-".  This value cannot be updated.
        /// </summary>
        public string EnteredAlphaGradeName { get; set; }

        /// <summary>
        /// The numeric value of the manually entered average.  This value cannot be updated.
        /// </summary>
        public decimal? EnteredNumericAverage { get; set; }

        /// <summary>
        /// The value a user entered to manually override the calculated average.  This can be numeric or alpha.  
        /// Examples: "A", "98.76".  This is the value that will used for updates.
        /// </summary>
        public string EnteredAverageValue { get; set; }

        /// <summary>
        /// Indicates whether or not the average is exempt for the student.
        /// </summary>
        public bool Exempt { get; set; }

        /// <summary>
        /// Id of the grading period that the average occurs in.  For averages setup by a teacher, a grading period will not be set.
        /// </summary>
        public int? GradingPeriodId { get; set; }

        /// <summary>
        /// Indicates whether or not this is the average that is associated with the graded item marked as 
        /// DisplayAvgInGradebook.  This should be the main average column in the gradebook.
        /// </summary>
        public bool IsGradingPeriodAverage { get; set; }

        /// <summary>
        /// Indicates whether or not the average can be exempt
        /// </summary>
        public bool MayBeExempt { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }

    }
}
