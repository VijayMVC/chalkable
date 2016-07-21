using System;

namespace Chalkable.StiConnector.Connectors.Model.StudentPanorama
{
    public class StudentPeriodAbsence
    {
        public int AcadSessionId { get; set; }
        public DateTime Date { get; set; }
        public int StudentId { get; set; }
        public int TimeSlotId { get; set; }
        public string TimeSlotName { get; set; }
    }
}
