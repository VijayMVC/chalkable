using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class Activity
    {
        /// <summary>
        /// A list of attributes that provide additional notes about the activity
        /// </summary>
        public IEnumerable<ActivityAssignedAttribute> Attributes { get; set; }

        /// <summary>
        /// The id of the category
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// The date of the activity
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Indicates whether or not this activity should be visible in home portal.  A teacher might set this to false for a pop quiz
        /// </summary>
        public bool DisplayInHomePortal { get; set; }

        /// <summary>
        /// Id of the activity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicates whether or not this activity is an assesment
        /// </summary>
        public bool IsAssessment { get; set; }

        /// <summary>
        /// Indicates whether or not the activity is dropped.  When true, the activity is dropped for all students
        /// </summary>
        public bool IsDropped { get; set; }

        /// <summary>
        /// Indicates whether or not the activity is scored
        /// </summary>
        public bool IsScored { get; set; }

        /// <summary>
        /// The maximum number of points allowed for the activity.
        /// </summary>
        public decimal? MaxScore { get; set; }

        /// <summary>
        /// Indicates whether or not an activity can be dropped.  This flag is used when determining which high and low scores to automatically drop during the grade calculation
        /// </summary>
        public bool MayBeDropped { get; set; }

        /// <summary>
        /// Name of the activity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        public string Unit { get; set; }

        /// <summary>
        /// The weight that should be added to each score for the activity
        /// </summary>
        public decimal? WeightAddition { get; set; }


        public decimal? WeightMultiplier { get; set; }

    }

    public class ActivityAssignedAttribute
    {

        /// <summary>
        /// The Id of the activity attribute.
        /// </summary>
        public short ActivityAttributeId { get; set; }

        /// <summary>
        /// The Id of the activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// Indicates whether or not this attribute should be visible in home portal.  A teacher might want to enter private notes about an activity that students should not see.
        /// </summary>
        public bool DisplayInHomePortal { get; set; }

        /// <summary>
        /// The id of the ActivityAssignedAttribute. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text of the attribute
        /// </summary>
        public string Text { get; set; }

    }
}
