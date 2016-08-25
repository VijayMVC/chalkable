using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class GradeVerificationReportParams
    {
        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }

        /// <summary>
        /// List of graded item ids that should be included in the report
        /// </summary>
        public int[] GradedItemIds { get; set; }

        /// <summary>
        /// List of grading period ids that should be included in the report
        /// </summary>
        public int[] GradingPeriodIds { get; set; }

        /// <summary>
        /// The type of grade that should be included in the report.  Options include Both = 0 (default), Alpha =1 , Numeric = 2
        /// </summary>
        public int GradeType { get; set; }

        /// <summary>
        /// The type of student identifier that should print on the report. Options include:
        ///     None = 0,
        ///     StudentNumber = 1 (default),
        ///     StateIdNumber = 2,
        ///     AltStudentNumber = 3,
        ///     SocialSecurityNumber = 4
        /// </summary>
        public int IdToPrint { get; set; }

        /// <summary>
        /// Include comments in the report
        /// </summary>
        public bool IncludeComments { get; set; }

        /// <summary>
        /// Include Notes in the report
        /// </summary>
        public bool IncludeNotes { get; set; }

        /// <summary>
        /// Include the signature line in the report
        /// </summary>
        public bool IncludeSignature { get; set; }

        /// <summary>
        /// Include withdrawn student in the report
        /// </summary>
        public bool IncludeWithdrawn { get; set; }

        /// <summary>
        /// List of ids for the student
        /// </summary>
        public int[] StudentIds { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int? SectionId { get; set; }

        /// <summary>
        /// The order students should print on the report.  Options include:
        ///     StudentName = 0,
        ///     GradeLevel = 1,
        ///     IDToPrint = 2
        /// </summary>
        public int StudentOrder { get; set; }

    }
}
