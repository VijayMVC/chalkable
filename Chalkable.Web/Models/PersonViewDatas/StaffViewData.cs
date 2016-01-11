using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StaffViewData : ShortPersonViewData
    {
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