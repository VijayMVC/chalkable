using System;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.ClassPanorama;
using Chalkable.Common;

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
        public decimal? TotalOfDaysEnrolled { get; set; }

        public static StudentDetailsViewData Create(StudentDetailsInfo student, decimal? gradeAvg, ShortStudentAbsenceInfo absences, int? infractions)
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
                Ethnicity = student.Ethnicity != null ? EthnicityViewData.Create(student.Ethnicity) : null,
                Absences = absences != null ? decimal.Round(absences.NumberOfAbsences) : (decimal?)null,
                Discipline = infractions ?? 0,
                GradeAvg = gradeAvg.HasValue ? decimal.Round(gradeAvg.Value, 2) : (decimal?) null,
                IsIEPActive = student.IsIEPActive,
                IsRetainedFromPrevSchoolYear = false,
                TotalOfDaysEnrolled = absences?.NumberOfDaysEnrolled
            };
        }
    }
}