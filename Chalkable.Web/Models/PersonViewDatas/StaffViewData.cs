using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StaffViewData
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public RoleViewData Role { get; set; }

        protected StaffViewData(Staff staff)
        {
            Id = staff.Id;
            DisplayName = staff.DisplayName();
            FullName = staff.DisplayName();
            FirstName = staff.FirstName;
            LastName = staff.LastName;
            Gender = staff.Gender;
            Role = RoleViewData.Create(CoreRoles.TEACHER_ROLE);
        }

        public static StaffViewData Create(Staff staff)
        {
            return new StaffViewData(staff);
        }
    }
}