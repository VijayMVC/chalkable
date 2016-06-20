using System;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentDetailsViewData : ShortPersonViewData
    {
        public bool IsHispanic { get; set; }
        public bool IsRetainedFromPrevSchoolYear { get; set; }
        public bool IsIEPActive { get; set; }
        public EthnicityViewData Ethnicity { get; set; }
        public decimal? GradeAvg { get; set; }
        public decimal? Absences { get; set; }
        public int? Discipline { get; set; }

        public static StudentDetailsViewData Create(StudentDetails student, Ethnicity ethnicity, 
            decimal? gradeAvg, decimal? absences, int? infractions, DateTime currentSchoolTime)
        {
            return new StudentDetailsViewData
            {
                Id = student.Id,
                DisplayName = student.DisplayName(),
                FullName = student.FullName(),
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gender = student.Gender,
                Role = RoleViewData.Create(CoreRoles.STUDENT_ROLE),
                IsHispanic = student.IsHispanic,
                Ethnicity = ethnicity != null ? EthnicityViewData.Create(ethnicity) : null,
                Absences = absences,
                Discipline = infractions,
                GradeAvg = gradeAvg,
                IsIEPActive = student.IsIEPActive(currentSchoolTime),
                IsRetainedFromPrevSchoolYear = false
            };
        }
    }
}