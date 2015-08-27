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

        protected override void InsertInternal(IList<StudentAcadSession> entities)
        {
            var assignments = entities.Where(x => x.GradeLevelID.HasValue)
                .ToList()
                .Select(x => new StudentSchoolYear
                {
                    GradeLevelRef = x.GradeLevelID.Value,
                    SchoolYearRef = x.AcadSessionID,
                    StudentRef = x.StudentID,
                    EnrollmentStatus = StudentEnrollmentStatusEnumFromString(x.CurrentEnrollmentStatus)
                }).ToList();
            SchoolLocator.SchoolYearService.AssignStudent(assignments);
        }

        protected override void UpdateInternal(IList<StudentAcadSession> entities)
        {
            var ssy = entities.Select(x => new StudentSchoolYear
            {
                SchoolYearRef = x.AcadSessionID,
                StudentRef = x.StudentID,
                GradeLevelRef = x.GradeLevelID.Value,
                EnrollmentStatus = StudentEnrollmentStatusEnumFromString(x.CurrentEnrollmentStatus)
            }).ToList();
            SchoolLocator.SchoolYearService.EditStudentSchoolYears(ssy);
        }

        protected override void DeleteInternal(IList<StudentAcadSession> entities)
        {
            var assignments = entities.Select(x => new StudentSchoolYear
            {
                SchoolYearRef = x.AcadSessionID,
                StudentRef = x.StudentID
            }).ToList();
            SchoolLocator.SchoolYearService.UnassignStudents(assignments);
        }
    }
}