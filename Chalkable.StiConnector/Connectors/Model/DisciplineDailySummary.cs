using System;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class DisciplineDailySummary
    {
        /// <summary>
        /// The date for which discipline data is summarized
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The number of discipline referrals for the date
        /// </summary>
        public int Infractions { get; set; }
    }
}
