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
 
    public class ClassPersonShortGrading : ClassPerson
    {
        public Guid CourseId { get; set; }
        public string ClassName { get; set; }
        public int? StudentAvg { get; set; }
        public int? ClassAvg { get; set; }
   
    }

    public class AnnouncementTypeGrading
    {
        public Guid ClassPersonId { get; set; }
        public Guid MarkingPeriodClassId { get; set; }
        public int AnnouncementTypeId { get; set; }
        public string AnnouncementTypeName { get; set; }
        public int? StudentItemTypeAvg { get; set; }
        public int? ClassItemTypeAvg { get; set; }
        public Guid AnnouncementId { get; set; }
        public int AnnouncementOrder { get; set; }
        public bool AnnouncementDropped { get; set; }
        public int? ItemAvg { get; set; }
        public int? StudentGrade { get; set; }     
    }

    public class ClassPersonGradingStats : ClassPersonShortGrading
    {
        public IList<AnnouncementTypeGrading> GradingsByAnnType { get; set; }

        public ClassPersonGradingStats()
        {
            GradingsByAnnType = new List<AnnouncementTypeGrading>();
        }
    }
}
