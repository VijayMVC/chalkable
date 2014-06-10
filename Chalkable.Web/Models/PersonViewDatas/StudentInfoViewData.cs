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
            GradeLevels = student.StudentSchoolYears.Select(x => IdNameViewData<int>.Create(x.GradeLevelRef, x.GradeLevel.Name)).ToList();
            GradeLevel = GradeLevels.FirstOrDefault();
        }

        public static new StudentInfoViewData Create(PersonDetails student)
        {
            return new StudentInfoViewData(student);
        }
    }


   
}