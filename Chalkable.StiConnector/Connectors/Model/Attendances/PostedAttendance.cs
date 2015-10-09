using System;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class PostedAttendance
    {
        public bool AttendancePosted { get; set; }
        public TimeSpan EndTime { get; set; }
        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }
        public TimeSpan StartTime { get; set; }
        /// <summary>
        /// Id of the time slot the section meets
        /// </summary>
        public int TimeSlotId { get; set; }
        /// <summary>
        /// Id of the time slot variation, if one exists
        /// </summary>
        public int? TimeSlotVariationId { get; set; }
    }
}
