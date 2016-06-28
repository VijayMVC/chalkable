using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentAcadSessionAdapter : SyncModelAdapter<StudentAcadSession>
    {
        public StudentAcadSessionAdapter(AdapterLocator locator) : base(locator)
        {
        }


        private StudentEnrollmentStatusEnum StudentEnrollmentStatusEnumFromString(string value)
        {
            return value == "C"
                ? StudentEnrollmentStatusEnum.CurrentlyEnrolled
                : value == "G"
                    ? StudentEnrollmentStatusEnum.Registered
                    : StudentEnrollmentStatusEnum.PreviouslyEnrolled;
        }

        private StudentSchoolYear Selector(StudentAcadSession x)
        {
            return new StudentSchoolYear
            {
                GradeLevelRef = x.GradeLevelID ?? 0,
                SchoolYearRef = x.AcadSessionID,
                StudentRef = x.StudentID,
                EnrollmentStatus = StudentEnrollmentStatusEnumFromString(x.CurrentEnrollmentStatus),
                IsRetained = x.IsRetained,
                HomeroomRef = x.HomeroomID
            };
        }

        protected override void InsertInternal(IList<StudentAcadSession> entities)
        {
            var assignments = entities.Where(x => x.GradeLevelID.HasValue).Select(Selector).ToList();
            ServiceLocatorSchool.SchoolYearService.AssignStudent(assignments);
        }

        protected override void UpdateInternal(IList<StudentAcadSession> entities)
        {
            var ssy = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SchoolYearService.EditStudentSchoolYears(ssy);
        }

        protected override void DeleteInternal(IList<StudentAcadSession> entities)
        {
            var assignments = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SchoolYearService.UnassignStudents(assignments);
        }
    }
}