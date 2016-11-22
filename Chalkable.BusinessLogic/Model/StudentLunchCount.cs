using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentLunchCount : Student
    {
        public bool IsAbsent { get; set; }

        public static StudentLunchCount Create(Student student, bool? isAbsent)
        {
            return new StudentLunchCount
            {
                Id = student.Id,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                LastName = student.LastName,
                BirthDate = student.BirthDate,
                Gender = student.Gender,
                HasMedicalAlert = student.HasMedicalAlert,
                IsAllowedInetAccess = student.IsAllowedInetAccess,
                SpecialInstructions = student.SpecialInstructions,
                SpEdStatus = student.SpEdStatus,
                PhotoModifiedDate = student.PhotoModifiedDate,
                UserId = student.UserId,
                IsHispanic = student.IsHispanic,
                IEPBeginDate = student.IEPBeginDate,
                IEPEndDate = student.IEPEndDate,
                Section504Qualification = student.Section504Qualification,
                GenderDescriptor = student.GenderDescriptor,
                IsHomeless = student.IsHomeless,
                IsImmigrant = student.IsImmigrant,
                LimitedEnglishRef = student.LimitedEnglishRef,
                IsForeignExchange = student.IsForeignExchange,
                StateIdNumber = student.StateIdNumber,
                AltStudentNumber = student.AltStudentNumber,
                StudentNumber = student.StudentNumber,
                OriginalEnrollmentDate = student.OriginalEnrollmentDate,
                IsWithdrawn = student.IsWithdrawn,
                IsAbsent = isAbsent ?? false
            };
        }
    }
}
