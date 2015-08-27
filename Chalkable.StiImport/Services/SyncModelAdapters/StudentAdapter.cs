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

        protected override void InsertInternal(IList<Student> entities)
        {
            var students = entities.Select(x => new Data.School.Model.Student
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
                SpEdStatus = x.SpEdStatusID.HasValue ? Locator.SpEdStatusMapping[x.SpEdStatusID.Value] : ""
            }).ToList();
            SchoolLocator.StudentService.AddStudents(students);
        }

        protected override void UpdateInternal(IList<Student> entities)
        {
            var students = entities.Select(x => new Data.School.Model.Student
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
                SpEdStatus = x.SpEdStatusID.HasValue ? Locator.SpEdStatusMapping[x.SpEdStatusID.Value] : ""
            }).ToList();
            SchoolLocator.StudentService.EditStudents(students);
        }

        protected override void DeleteInternal(IList<Student> entities)
        {
            var students = entities.Select(x => new Data.School.Model.Student { Id = x.StudentID }).ToList();
            SchoolLocator.StudentService.DeleteStudents(students);
        }
    }
}