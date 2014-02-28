using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class GradingPeriod
    {
        public const string ID_FIELD = "Id";
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }        
        public int MarkingPeriodRef { get; set; }
        public int SchoolYearRef { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public string SchoolAnnouncement { get; set; }
        public bool AllowGradePosting { get; set; } 
    }

    public class GradingPeriodDetails : GradingPeriod
    {
        [DataEntityAttr]
        public MarkingPeriod MarkingPeriod { get; set; }
    }
}
