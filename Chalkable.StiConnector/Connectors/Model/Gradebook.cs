using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class Gradebook
    {
        public IEnumerable<Activity> Activities { get; set; }

        /// <summary>
        /// The options a teacher has selected for their gradebook
        /// </summary>
        public ClassroomOption Options { get; set; }

        /// <summary>
        /// A list of scores for each student and activity combo 
        /// </summary>
        public IEnumerable<Score> Scores { get; set; }

        /// <summary>
        /// The id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// A list of averages that should be displayed in the gradebook
        /// </summary>
        public IEnumerable<StudentAverages> StudentAverages { get; set; }

    }
}
