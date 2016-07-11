using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityCategory
    {
        /// <summary>
        /// Id of the activity category 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the activity category.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The number of high scores to drop when using category averaging.  
        /// For example, if the value is set to 1 we will drop the highest score 
        /// in the category when calculating the average.
        /// </summary>
        public byte HighScoresToDrop { get; set; }
        /// <summary>
        /// Teachers should only be able to edit categories that aren't system categories.  
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// The number of low scores to drop when using category averaging.   
        /// For example, if the value is set to 2 we will drop the two lowest 
        /// scores in the category when calculating the average.
        /// </summary>
        public byte LowScoresToDrop { get; set; }
        /// <summary>
        /// The Percentage that the category counts towards the final average
        /// </summary>
        public decimal? Percentage { get; set; }
        /// <summary>
        /// Id of the section for the activity category
        /// </summary>
        public int SectionId { get; set; }
    }
}
