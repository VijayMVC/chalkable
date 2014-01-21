using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class Activity
    {
        public IEnumerable<ActivityAttachment> Attachments { get; set; }
        public int? CategoryId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool DisplayInHomePortal { get; set; }
        public int Id { get; set; }
        public bool IsAssessment { get; set; }
        public bool IsDropped { get; set; }
        public bool IsScored { get; set; }
        public decimal? MaxScore { get; set; }
        public bool MayBeDropped { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public string Unit { get; set; }
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
