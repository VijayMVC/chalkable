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

        private Data.School.Model.StudentSchool Selector(StudentSchool x)
        {
            return new Data.School.Model.StudentSchool
            {
                SchoolRef = x.SchoolID,
                StudentRef = x.StudentID
            };
        }

        protected override void InsertInternal(IList<StudentSchool> entities)
        {
            var studentSchools = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentService.AddStudentSchools(studentSchools);
        }

        protected override void UpdateInternal(IList<StudentSchool> entities)
        {
            //No update here
        }

        protected override void DeleteInternal(IList<StudentSchool> entities)
        {
            var ss = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentService.DeleteStudentSchools(ss);
        }
    }
}