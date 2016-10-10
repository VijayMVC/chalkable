using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class DisciplineSummaryDashboard
    {
        /// <summary>
        /// A list of infractions and their counts for the student
        /// </summary>
        public IEnumerable<InfractionSummary> Infractions { get; set; }
    }
}
