using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentInfoViewData : PersonInfoViewData
    {
        public bool IEP { get; set; }
        public IdNameViewData<int> GradeLevel { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string PreviousSchool { get; set; }
        public string PreviousSchoolPhone { get; set; }
        public string PreviousSchoolNote { get; set; }
        public IList<StudentParentViewData> Parents { get; set; }

        protected StudentInfoViewData(PersonDetails student):base(student)
        {
            throw new NotImplementedException();
            //IEP = student.StudentInfo.IEP;
            //GradeLevel = IdNameViewData.Create(student.StudentInfo.GradeLevelRef, student.StudentInfo.GradeLevel.Name);
            //EnrollmentDate = student.StudentInfo.EnrollmentDate ?? DateTime.MinValue;
            //PreviousSchool = student.StudentInfo.PreviousSchool;
            //PreviousSchoolNote = student.StudentInfo.PreviousSchoolNote;
            //PreviousSchoolPhone = student.StudentInfo.PreviousSchoolPhone;            
        }

        public static new StudentInfoViewData Create(PersonDetails student)
        {
            return new StudentInfoViewData(student);
        }
    }


   
}