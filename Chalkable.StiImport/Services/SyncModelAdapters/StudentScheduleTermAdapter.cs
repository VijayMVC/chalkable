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

        protected override void InsertInternal(IList<StudentScheduleTerm> entities)
        {
            var studentSchedules = entities.Select(x => new ClassPerson
            {
                ClassRef = x.SectionID,
                PersonRef = x.StudentID,
                MarkingPeriodRef = x.TermID,
                IsEnrolled = x.IsEnrolled
            }).ToList();
            ServiceLocatorSchool.ClassService.AddStudents(studentSchedules);
        }

        protected override void UpdateInternal(IList<StudentScheduleTerm> entities)
        {
            var studentSchedules = entities.Select(x => new ClassPerson
                {
                    ClassRef = x.SectionID,
                    PersonRef = x.StudentID,
                    MarkingPeriodRef = x.TermID,
                    IsEnrolled = x.IsEnrolled
                }).ToList();
            ServiceLocatorSchool.ClassService.EditStudents(studentSchedules);
        }

        protected override void DeleteInternal(IList<StudentScheduleTerm> entities)
        {
            var students = entities.Select(x => new ClassPerson
                                  {
                                      ClassRef = x.SectionID,
                                      MarkingPeriodRef = x.TermID,
                                      PersonRef = x.StudentID
                                  })
                                  .ToList();
            ServiceLocatorSchool.ClassService.DeleteStudent(students);
        }
    }
}