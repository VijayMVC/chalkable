using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model.StudentPanorama;

namespace Chalkable.BusinessLogic.Model.StudentPanorama
{
    public class StudentInfractionInfo
    {
        public int StudentId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public string InfractionName { get; set; }
        public string InfractionStateCode { get; set; }
        public byte DemeritsEarned { get; set; }
        public bool IsPrimary { get; set; }
        public string DispositionName { get; set; }
        public string DispositionNote { get; set; }
        public DateTime? DispositionStartDate { get; set; }

        public static StudentInfractionInfo Create(StudentInfraction infraction)
        {
            return new StudentInfractionInfo
            {
                StudentId = infraction.StudentId,
                OccurrenceDate = infraction.OccurrenceDate,
                InfractionName = infraction.InfractionName,
                InfractionStateCode = infraction.InfractionStateCode,
                DemeritsEarned = infraction.DemeritsEarned,
                IsPrimary = true,
                DispositionName = infraction.DispositionName,
                DispositionNote = infraction.DispositionNote,
                DispositionStartDate = infraction.DispositionStartDateTime
            };
        }
    }
}
