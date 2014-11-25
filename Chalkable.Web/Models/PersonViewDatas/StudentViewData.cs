using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentViewData
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public RoleViewData Role { get; set; }
        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public bool? IsWithDrawn { get; set; }

        protected StudentViewData(StudentDetails student)
        {
            Id = student.Id;
            DisplayName = student.DisplayName();
            FullName = student.FullName();
            FirstName = student.LastName;
            LastName = student.FirstName;
            Gender = student.Gender;
            HasMedicalAlert = student.HasMedicalAlert;
            IsAllowedInetAccess = student.IsAllowedInetAccess;
            SpecialInstructions = student.SpecialInstructions;
            SpEdStatus = student.SpEdStatus;
            Role = RoleViewData.Create(CoreRoles.STUDENT_ROLE);
            IsWithDrawn = student.IsWithdrawn;
        }

        public static StudentViewData Create(StudentDetails student)
        {
            return new StudentViewData(student);
        }
    }
}