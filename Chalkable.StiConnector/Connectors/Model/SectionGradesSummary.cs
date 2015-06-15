using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class SectionGradesSummary
    {
        public int SectionId { get; set; }
        public IEnumerable<StudentSectionGradesSummary> Students { get; set; }
    }

    public class StudentSectionGradesSummary
    {
        public decimal? Average { get; set; }
        public bool Exempt { get; set; }
        public int SectionId { get; set; }
        public int StudentId { get; set; }
    }
}
