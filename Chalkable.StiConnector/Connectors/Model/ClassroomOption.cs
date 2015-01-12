using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ClassroomOption
    {
        /// <summary>
        /// Indicates what Averaging method the teacher is using for the class.  Values will be "Average" or "Points"
        /// </summary>
        public string AveragingMethod { get; set; }

        /// <summary>
        /// Indicates whether or not the teacher is using category averaging for averages
        /// </summary>
        public bool CategoryAveraging { get; set; }

        /// <summary>
        /// Indicates whether or not the Alpha grade average column should be shown in the gradebook
        /// </summary>
        public bool DisplayAlphaGrades { get; set; }

        /// <summary>
        /// Indicates whether or not averages should be displayed as a column in the gradebook
        /// </summary>
        public bool DisplayStudentAverage { get; set; }

        /// <summary>
        /// Indicates whether or not total points should be displayed as a column in the gradebook
        /// </summary>
        public bool DisplayTotalPoints { get; set; }

        /// <summary>
        /// Indicates whether or not withdrawn students should be displayed in the gradebook
        /// </summary>
        public bool IncludeWithdrawnStudents { get; set; }

        /// <summary>
        /// Indicates whether or not the Averages in the gradebook should be rounded
        /// </summary>
        public bool RoundAverages { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// The grading scale to use for displaying standard alpha grades
        /// </summary>
        public int? StandardsGradingScaleId { get; set; }

        /// <summary>
        ///  The method being used to calculate standards grades.  Values: "Averaging" or "Points"
        /// </summary>
        public string StandardsCalculationMethod { get; set; }

        /// <summary>
        /// Indicates the time period for calculating standards.  Values: "CurrentGradingPeriod", "RunningAverage"
        /// </summary>
        public string StandardsCalculationRule { get; set; }

        /// <summary>
        /// Indicates whether or not to weight the max value when calculating standards
        /// </summary>
        public bool StandardsCalculationWeightMaximumValues { get; set; }
    }
}
