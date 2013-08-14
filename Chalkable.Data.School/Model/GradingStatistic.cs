using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentGradeAvg
    {
        [DataEntityAttr]
        public Person Student { get; set; }
        public int? Avg { get; set; }
    }

    public class StudentGradeAvgPerMPC : StudentGradeAvg
    {
        [DataEntityAttr]
        public MarkingPeriodClass MarkingPeriodClass { get; set; }
    }

    public class StudentGradeAvgPerClass : StudentGradeAvg
    {
        public Guid ClassRef { get; set; }
    }

    public class MarkingPeriodClassGradeAvg : MarkingPeriodClass
    {
        private Class _class;
        [DataEntityAttr]
        public Class Class
        {
            get { return _class; }
            set
            {
                _class = value;
                if (value != null && value.Id != Guid.Empty)
                    ClassRef = value.Id;
            }
        }
        private MarkingPeriod markingPeriod;
        [DataEntityAttr]
        public MarkingPeriod MarkingPeriod
        {
            get { return markingPeriod; }
            set
            {
                markingPeriod = value;
                if (value != null && value.Id != Guid.Empty)
                    MarkingPeriodRef = value.Id;
            }
        }
        public int? Avg { get; set; }
    }

    public class DepartmentGradeAvg
    {
        public Guid ChalkableDepartmentRef { get; set; }
        public int? Avg { get; set; }
    }


    public class GradeAvgPerDate
    {
        public int? Avg { get; set; }
        public const string DATE_FIELD = "Date";
        public DateTime Date { get; set; }

    }
    public class StudentGradeAvgPerDate : GradeAvgPerDate
    {
        public Guid StudentId { get; set; }
        public int? PeersAvg { get; set; } 
    }

    public class StudentClassGradeStats
    {
        public const string STUDENT_ID_FEILD = "StudentId";
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
        public IList<GradeAvgPerDate> GradeAvgPerDates { get; set; }
        public IList<AnnTypeGradeStats> AnnTypesGradeStats { get; set; }
    }
    public class AnnTypeGradeStats
    {
        public const string ANNOUNCEMENT_TYPE_ID_FIELD = "AnnouncementTypeId";
        public int AnnouncementTypeId { get; set; }
        public IList<GradeAvgPerDate> GradeAvgPerDates { get; set; } 
    } 
}
