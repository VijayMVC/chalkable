using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentInfoViewData : PersonInfoViewData
    {
        public IdNameViewData<int> GradeLevel { get; set; } //todo remove this later 
        public IList<IdNameViewData<int>> GradeLevels { get; set; }
        public IList<StudentParentViewData> Parents { get; set; }
        public IList<StudentHealthConditionViewData> HealthConditions { get; set; } 
        protected StudentInfoViewData(PersonDetails student):base(student)
        {
            GradeLevels = student.StudentSchoolYears.OrderBy(x=>x.SchoolYearRef).Select(x => IdNameViewData<int>.Create(x.GradeLevelRef, x.GradeLevel.Name)).ToList();
            GradeLevel = GradeLevels.LastOrDefault();
        }

        public static new StudentInfoViewData Create(PersonDetails student)
        {
            return new StudentInfoViewData(student);
        }

        public static StudentInfoViewData Create(PersonDetails student, int currentSchoolYearId)
        {
            var res = Create(student);
            var currentStudentSchoolYear = student.StudentSchoolYears.FirstOrDefault(x => x.SchoolYearRef == currentSchoolYearId);
            if (currentStudentSchoolYear != null)
                res.GradeLevel = res.GradeLevels.First(x => x.Id == currentStudentSchoolYear.GradeLevelRef);
            return res;
        }
    }


   
}