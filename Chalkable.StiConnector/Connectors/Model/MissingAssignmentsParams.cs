using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class MissingAssignmentsParams
    {
        public int AcadSessionId { get; set; }
        public int[] AlternateScoreIds { get; set; }
        public bool AlternateScoresOnly { get; set; }
        public bool ConsiderZerosAsMissingGrades { get; set; }
        public DateTime EndDate { get; set; }
        public int IdToPrint { get; set; }
        public bool IncludeWithdrawn { get; set; }
        public bool OnePerPage { get; set; }
        public int OrderBy { get; set; }
        public int? SectionId { get; set; }
        public DateTime StartDate { get; set; }
        public int? StudentId { get; set; }
        public bool SuppressStudentName { get; set; }
        public int UserId { get; set; }
    }
}
