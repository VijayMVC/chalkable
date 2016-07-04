using System;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentViewData : ShortPersonViewData
    {
        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public bool? IsWithDrawn { get; set; }

        protected StudentViewData(Student student)
        {
            Id = student.Id;
            DisplayName = student.DisplayName();
            FullName = student.FullName();
            FirstName = student.FirstName;
            LastName = student.LastName;
            Gender = student.Gender;
            HasMedicalAlert = student.HasMedicalAlert;
            IsAllowedInetAccess = student.IsAllowedInetAccess;
            SpecialInstructions = student.SpecialInstructions;
            SpEdStatus = student.SpEdStatus;
            Role = RoleViewData.Create(CoreRoles.STUDENT_ROLE);
            IsWithDrawn = student.IsWithdrawn;
        }

        public static StudentViewData Create(Student student)
        {
            return new StudentViewData(student);
        }
        
    }
}