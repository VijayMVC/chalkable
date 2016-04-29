using System.Collections.Generic;
using System.Linq;

namespace Chalkable.API.Models
{
    public class CoreRoles
    {
        public static readonly CoreRole SUPER_ADMIN_ROLE = new CoreRole(1, "SysAdmin", "system admin");
        public static readonly CoreRole TEACHER_ROLE = new CoreRole(2, "Teacher", "Teacher");
        public static readonly CoreRole STUDENT_ROLE = new CoreRole(3, "Student", "Student");
        public static readonly CoreRole PARENT_ROLE = new CoreRole(4, "Parent", "Parent");
        public static readonly CoreRole CHECKIN_ROLE = new CoreRole(6, "Checkin", "Checkin");
        public static readonly CoreRole DEVELOPER_ROLE = new CoreRole(9, "Developer", "Chalkable Developer");
        public static readonly CoreRole DISTRICT_ADMIN_ROLE = new CoreRole(10, "DistrictAdmin", "DistrictAdmin");
        public static readonly CoreRole APP_TESTER_ROLE = new CoreRole(11, "AppTester", "Application Tester");
        public static readonly CoreRole DISTRICT_REGISTRATOR_ROLE = new CoreRole(12, "DistrictRegistrator", "District Registrator");

        private static readonly Dictionary<string, CoreRole> roles = new Dictionary<string, CoreRole>
        {
            {SUPER_ADMIN_ROLE.LoweredName, SUPER_ADMIN_ROLE},
            {TEACHER_ROLE.LoweredName, TEACHER_ROLE},
            {STUDENT_ROLE.LoweredName, STUDENT_ROLE},
            {PARENT_ROLE.LoweredName, PARENT_ROLE},
            {CHECKIN_ROLE.LoweredName, CHECKIN_ROLE},
            {DEVELOPER_ROLE.LoweredName, DEVELOPER_ROLE},
            {APP_TESTER_ROLE.LoweredName, APP_TESTER_ROLE},
            {DISTRICT_ADMIN_ROLE.LoweredName, DISTRICT_ADMIN_ROLE},
            {DISTRICT_REGISTRATOR_ROLE.LoweredName, DISTRICT_REGISTRATOR_ROLE}
        };

        public static CoreRole GetByName(string name)
        {
            return roles[name.ToLower()];
        }

        public static CoreRole GetById(int id)
        {
            return roles.Values.First(x => x.Id == id);
        }
    }
}