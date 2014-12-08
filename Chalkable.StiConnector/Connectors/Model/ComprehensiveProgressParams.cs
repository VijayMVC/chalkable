using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ComprehensiveProgressParams
    {
         public enum OrderByMethod {
            StudentDisplayName = 1,
            StudentIdentifier = 2,
            GradeLevel = 3,
            Homeroom = 4,
            PostalCode = 5,
            DistributionPeriod = 6
        }

        
        public int[] AbsenceReasonIds { get; set; }

        public int AcadSessionId { get; set; }

        public bool AdditionalMailings { get; set; }
        public bool ClassAverageOnly { get; set; }

        public bool DisplayCategoryAverages { get; set; }
        public bool DisplayClassAverage { get; set; }

        public int DailyAttendanceDisplayMethod { get; set; }

        public bool DisplayPeriodAttendance { get; set; }
        public bool DisplaySignatureLine { get; set; }
        public bool DisplayStudentComment { get; set; }
        public bool DisplayStudentMailingAddress { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public DateTime? EndDate { get; set; }
        public bool GoGreen { get; set; }

        public int[] GradingPeriodIds { get; set; }

        public int IdToPrint { get; set; }

        public decimal? MaxStandardAverage { get; set; }

        public decimal? MinStandardAverage { get; set; }
        public OrderByMethod OrderBy { get; set; }
        public int SectionId { get; set; }
        public int? StaffId { get; set; }
        public DateTime? StartDate { get; set; }
        public int? StudentFilterId { get; set; }
        public int[] StudentIds { get; set; }
        public int? UserId { get; set; }
        public bool WindowEnvelope { get; set; }
    }
}
