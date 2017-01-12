using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Common
{
    public class CoreRole
    {
        public string Name { get; private set; }
        public string LoweredName { get; private set; }
        public string Description { get; private set; }
        public int Id { get; private set; }
        public CoreRole(int id, string name, string description)
        {
            Name = name;
            LoweredName = name.ToLower();
            Id = id;
            Description = description;
        }
    }

    public class CoreRoles
    {
        public static CoreRole SUPER_ADMIN_ROLE = new CoreRole(1, "SysAdmin", "system admin");
        public static CoreRole TEACHER_ROLE = new CoreRole(2, "Teacher", "Teacher");
        public static CoreRole STUDENT_ROLE = new CoreRole(3, "Student", "Student");
        public static CoreRole PARENT_ROLE = new CoreRole(4, "Parent", "Parent");
        public static CoreRole CHECKIN_ROLE = new CoreRole(6, "Checkin", "Checkin");
        public static CoreRole DEVELOPER_ROLE = new CoreRole(9, "Developer", "Chalkable Developer");
        public static CoreRole DISTRICT_ADMIN_ROLE = new CoreRole(10, "DistrictAdmin", "DistrictAdmin");
        public static CoreRole APP_TESTER_ROLE = new CoreRole(11, "AppTester", "Application Tester");
        public static CoreRole DISTRICT_REGISTRATOR_ROLE = new CoreRole(12, "DistrictRegistrator", "District Registrator");
        public static CoreRole ASSESSMENT_ADMIN_ROLE = new CoreRole(13, "AssessmentAdmin", "Assessment Admin");

        private static Dictionary<string, CoreRole> roles = new Dictionary<string, CoreRole>
                                                                 {
                                                                     {SUPER_ADMIN_ROLE.LoweredName, SUPER_ADMIN_ROLE},
                                                                     {TEACHER_ROLE.LoweredName, TEACHER_ROLE},
                                                                     {STUDENT_ROLE.LoweredName, STUDENT_ROLE},
                                                                     {PARENT_ROLE.LoweredName, PARENT_ROLE},
                                                                     {CHECKIN_ROLE.LoweredName, CHECKIN_ROLE},
                                                                     {DEVELOPER_ROLE.LoweredName, DEVELOPER_ROLE},
                                                                     {APP_TESTER_ROLE.LoweredName, APP_TESTER_ROLE},
                                                                     {DISTRICT_ADMIN_ROLE.LoweredName, DISTRICT_ADMIN_ROLE},
                                                                     {DISTRICT_REGISTRATOR_ROLE.LoweredName, DISTRICT_REGISTRATOR_ROLE},
                                                                     {ASSESSMENT_ADMIN_ROLE.LoweredName, ASSESSMENT_ADMIN_ROLE}
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
