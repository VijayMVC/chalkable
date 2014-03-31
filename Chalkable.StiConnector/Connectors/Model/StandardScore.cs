using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StandardScore
    {
        /// <summary>
        /// The average score computed from activity standard scores.  A user can never set this property.
        /// It can only be set by the Standards Calculator that calculates standard grades.  If a user 
        /// needs to override this score, they should set the EnteredScoreAlphaGradeId property
        /// </summary>        
        public decimal? ComputedScore { get; set; }

        /// <summary>
        /// Id of the alpha grade equivalent of the numeric computed score. A user can never set this property.
        /// </summary>
        public int? ComputedScoreAlphaGradeId { get; set; }

        /// <summary>
        /// Name of the alpha grade equivalent of the numeric computed score. A user can never set this property.
        /// </summary>
        public string ComputedScoreAlphaGradeName { get; set; }

        /// <summary>
        /// Id of the grading period the during which the standard score occurs
        /// </summary>
        public int GradingPeriodId { get; set; }

        /// <summary>
        /// Indicates whether or not the student is enrolled in the section
        /// </summary>
        public bool Enrolled { get; set; }

        /// <summary>
        /// Id of the Alpha grade.  A user can set this property in order to override the calculated 
        /// standard score, or to manually score a standard.
        /// </summary>
        public int? EnteredScoreAlphaGradeId { get; set; }

        /// <summary>
        /// Name of the Alpha grade for the manually overridden score.  You do not need to set this property on update.  
        /// This property automatically gets populated from the EnteredScoreAlphaGradeId property
        /// </summary>
        public string EnteredScoreAlphaGradeName { get; set; }

        /// <summary>
        /// A note field for entering comments about the standard score.  There is a 500 character limit on this field
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// Id of the standard
        /// </summary>
        public int StandardId { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }

    }
}
