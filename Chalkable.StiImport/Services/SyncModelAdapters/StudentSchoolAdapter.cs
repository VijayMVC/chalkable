using System.Collections.Generic;
using System.Linq;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentSchoolAdapter : SyncModelAdapter<StudentSchool>
    {
        public StudentSchoolAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private static Data.School.Model.StudentSchool Selector(StudentSchool x)
        {
            return new Data.School.Model.StudentSchool
            {
                SchoolRef = x.SchoolID,
                StudentRef = x.StudentID,
                CounselorRef = x.CounselorID,
                IsTitle1Eligible = x.IsTitle1Eligible
            };
        }

        protected override void InsertInternal(IList<StudentSchool> entities)
        {
            var studentSchools = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentService.AddStudentSchools(studentSchools);
        }

        protected override void UpdateInternal(IList<StudentSchool> entities)
        {
            //needed for resync
            ServiceLocatorSchool.StudentService.EditStudentSchools(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<StudentSchool> entities)
        {
            var ss = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentService.DeleteStudentSchools(ss);
        }
    }
}