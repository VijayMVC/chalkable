using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentInfoViewData : PersonInfoViewData
    {
        public IdNameViewData<int> GradeLevel { get; set; }
        public IList<StudentHealthConditionViewData> HealthConditions { get; set; }

        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public IList<StudentCustomAlertDetailViewData> StudentCustomAlertDetails { get; set; }

        

        public IList<StudentContactViewData> StudentContacts { get; set; }
        
        protected StudentInfoViewData(PersonDetails student):base(student)
        {
            var gradeLevels = student.StudentSchoolYears.OrderBy(x=>x.SchoolYearRef).Select(x => IdNameViewData<int>.Create(x.GradeLevelRef, x.GradeLevel.Name)).ToList();
            GradeLevel = gradeLevels.LastOrDefault();
        }

        public static new StudentInfoViewData Create(PersonDetails student)
        {
            return new StudentInfoViewData(student);
        }

        public static StudentInfoViewData Create(PersonDetails student, int currentSchoolYearId)
        {
            var res = Create(student);
            var gradeLevels = student.StudentSchoolYears.OrderBy(x => x.SchoolYearRef).Select(x => IdNameViewData<int>.Create(x.GradeLevelRef, x.GradeLevel.Name)).ToList();
            var currentStudentSchoolYear = student.StudentSchoolYears.FirstOrDefault(x => x.SchoolYearRef == currentSchoolYearId);
            if (currentStudentSchoolYear != null)
                res.GradeLevel = gradeLevels.First(x => x.Id == currentStudentSchoolYear.GradeLevelRef);
            return res;
        }
    }


   
}