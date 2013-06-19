using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static CoreRole SUPER_ADMIN_ROLE = new CoreRole(1, "System Admin", "system admin");
        public static CoreRole TEACHER_ROLE = new CoreRole(2, "Teacher", "Teacher");
        public static CoreRole STUDENT_ROLE = new CoreRole(3, "Student", "Student");
        public static CoreRole PARENT_ROLE = new CoreRole(4, "Parent", "Parent");
        public static CoreRole ADMIN_GRADE_ROLE = new CoreRole(5, "AdminGrade", "School Administrator");
        public static CoreRole CHECKIN_ROLE = new CoreRole(6, "Checkin", "Checkin");
        public static CoreRole ADMIN_EDIT_ROLE = new CoreRole(7, "AdminEdit", "School Administrator for Edit");
        public static CoreRole ADMIN_VIEW_ROLE = new CoreRole(8, "AdminView", "Administrator for View");
        public static CoreRole DEVELOPER_ROLE = new CoreRole(9, "Developer", "Chalkable Developer");


        private static Dictionary<string, CoreRole> roles = new Dictionary<string, CoreRole>
                                                                 {
                                                                     {SUPER_ADMIN_ROLE.LoweredName, SUPER_ADMIN_ROLE},
                                                                     {TEACHER_ROLE.LoweredName, TEACHER_ROLE},
                                                                     {STUDENT_ROLE.LoweredName, STUDENT_ROLE},
                                                                     {PARENT_ROLE.LoweredName, PARENT_ROLE},
                                                                     {ADMIN_GRADE_ROLE.LoweredName, ADMIN_GRADE_ROLE},
                                                                     {CHECKIN_ROLE.LoweredName, CHECKIN_ROLE},
                                                                     {ADMIN_EDIT_ROLE.LoweredName, ADMIN_EDIT_ROLE},
                                                                     {ADMIN_VIEW_ROLE.LoweredName, ADMIN_VIEW_ROLE},
                                                                     {DEVELOPER_ROLE.LoweredName, DEVELOPER_ROLE},
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
