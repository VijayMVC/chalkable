using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentAdapter : SyncModelAdapter<Student>
    {
        public StudentAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Student Selector(Student x)
        {
            return new Data.School.Model.Student
            {
                BirthDate = x.DateOfBirth,
                Id = x.StudentID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.GenderID.HasValue ? Locator.GetnderMapping[x.GenderID.Value] : "U",
                UserId = x.UserID,
                HasMedicalAlert = x.HasMedicalAlert,
                IsAllowedInetAccess = x.IsAllowedInetAccess,
                SpecialInstructions = x.SpecialInstructions,
                SpEdStatus = x.SpEdStatusID.HasValue ? Locator.SpEdStatusMapping[x.SpEdStatusID.Value] : "",
                IsHispanic = x.IsHispanic
            };
        }

        protected override void InsertInternal(IList<Student> entities)
        {
            var students = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentService.AddStudents(students);
        }

        protected override void UpdateInternal(IList<Student> entities)
        {
            var students = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentService.EditStudents(students);
        }

        protected override void DeleteInternal(IList<Student> entities)
        {
            var students = entities.Select(x => new Data.School.Model.Student { Id = x.StudentID }).ToList();
            ServiceLocatorSchool.StudentService.DeleteStudents(students);
        }
    }
}