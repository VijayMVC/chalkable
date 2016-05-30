using System;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class DailySectionAbsenceSummary
    {
        /// <summary>
        /// The total absences that occurred on a specific date for a section
        /// </summary>
        public decimal Absences { get; set; }

        /// <summary>
        /// The date of the attendance summary
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// The number of tardies that occurred on a specific date
        /// </summary>
        public int Tardies { get; set; }
    }
}
