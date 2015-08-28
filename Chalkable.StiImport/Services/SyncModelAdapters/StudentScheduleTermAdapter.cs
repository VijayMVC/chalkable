using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentScheduleTermAdapter : SyncModelAdapter<StudentScheduleTerm>
    {
        public StudentScheduleTermAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private ClassPerson Selector(StudentScheduleTerm x)
        {
            return new ClassPerson
            {
                ClassRef = x.SectionID,
                PersonRef = x.StudentID,
                MarkingPeriodRef = x.TermID,
                IsEnrolled = x.IsEnrolled
            };
        }

        protected override void InsertInternal(IList<StudentScheduleTerm> entities)
        {
            var studentSchedules = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassService.AddStudents(studentSchedules);
        }

        protected override void UpdateInternal(IList<StudentScheduleTerm> entities)
        {
            var studentSchedules = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassService.EditStudents(studentSchedules);
        }

        protected override void DeleteInternal(IList<StudentScheduleTerm> entities)
        {
            var students = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassService.DeleteStudent(students);
        }
    }
}