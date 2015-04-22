using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class GradeVerificationReportParams
    {
        public int AcadSessionId { get; set; }
        public string Header { get; set; }
        public string DistrictName { get; set; }
        public int? TeacherId { get; set; }
        public int? StaffFilterId { get; set; }
        public string StartSectionNumber { get; set; }
        public string EndSectionNumber { get; set; }

        /// <summary>
        /// SectionSortOptions : 
        /// FullSectionNumber = 0,
        ///PeriodSectionNumber = 1,
        ///PeriodTeacher = 2,
        ///TeacherSectionNumber = 3,
        ///TeacherPeriod = 4
        /// </summary>
        public int SectionOrder { get; set; }
        ///<summary>
        /// StudentSortOptionsEnum
        ///StudentName = 0,
        ///GradeLevel = 1,
        ///IDToPrint = 2
        ///</summary>
        public int StudentOrder { get; set; }

        public int IdToPrint { get; set; }
        public int[] GradingPeriodId { get; set; }
        public int[] GradedItemId { get; set; }

        ///<summary>
        /// GradeTypeOptions :
        ///both = 0,
        ///alpha = 1,
        ///numeric = 2
        ///</summary>
        public int GradeType { get; set; }
        public bool IncludeNotes { get; set; }
        public bool IncludeComments { get; set; }
        public bool IncludeLegend { get; set; }
        public bool IncludeSignature { get; set; }
        public bool IncludeWithdrawn { get; set; }
        public int NumberToDisplay { get; set; }
        public int[] StudentIds { get; set; }
        public int? SectionId { get; set; }
        public int? StaffId { get; set; }
    }
}
