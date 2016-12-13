using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentSchoolsInfoViewData : StudentViewData
    {
        public IList<School> StudentSchools { get; set; }
        public GradeLevel GradeLevel { get; set; }

        public bool IsClassmate { get; set; }
        public bool IsMyStudent { get; set; }

        protected StudentSchoolsInfoViewData(StudentSchoolsInfo studentsSchoolsInfo) : base(studentsSchoolsInfo)
        {
            StudentSchools = studentsSchoolsInfo.StudentSchools;
            GradeLevel = studentsSchoolsInfo.GradeLevel;
            IsClassmate = studentsSchoolsInfo.IsClassmate;
            IsMyStudent = studentsSchoolsInfo.IsMyStudent;
        }

        public static StudentSchoolsInfoViewData Create(StudentSchoolsInfo studentsSchoolsInfo)
        {
            return new StudentSchoolsInfoViewData(studentsSchoolsInfo);
        }

        public static IList<StudentSchoolsInfoViewData> Create(IList<StudentSchoolsInfo> studentsSchoolsInfos)
        {
            return studentsSchoolsInfos.Select(Create).ToList();
        }
    }
}