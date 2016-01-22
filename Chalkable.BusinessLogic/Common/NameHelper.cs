using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Common
{
    public static class NameHelper
    {
        private const string GENDER_MALE = "m";
        private const string MR = "Mr. ";
        private const string MS = "Ms. ";

        private static string ResolveSalutation(string gender)
        {
            return !string.IsNullOrEmpty(gender) && gender.ToLower() == GENDER_MALE ? MR : MS;
        }

        private static string StaffDisplayName(string lastName, string gender, bool upper)
        {
            var ln = upper ? lastName.ToUpper() : lastName;
            return ResolveSalutation(gender) + ln;
        }
        
        public static string FullName(string firstName, string lastName, bool upper = true, string gender = null)
        {
            var fn = upper ? firstName.ToUpper() : firstName.CapitalizeFirstLetter();
            var ln = upper ? lastName.ToUpper() : lastName.CapitalizeFirstLetter();
            var gn = string.IsNullOrWhiteSpace(gender) ? "" : ResolveSalutation(gender);
            return gn + fn + " " + ln;
        }

        public static string TeacherDisplayName(this ScheduleItem staff, bool upper = true)
        {
            return string.IsNullOrEmpty(staff.TeacherLastName) ? null : StaffDisplayName(staff.TeacherLastName, staff.TeacherGender, upper);
        }

        public static string DisplayName(this Staff staff, bool upper = true)
        {
            return StaffDisplayName(staff.LastName, staff.Gender, upper);
        }

        public static string DisplayName(this Student student, bool upper = true)
        {
            return FullName(student.FirstName, student.LastName, upper);
        }

        public static string DisplayName(this Person person, bool upper = true)
        {
            if (person.RoleRef == CoreRoles.STUDENT_ROLE.Id)
                return FullName(person.FirstName, person.LastName, upper);
            return StaffDisplayName(person.LastName, person.Gender, upper);
        }

        public static string FullName(this Staff staff, bool upper = true, bool withSalutation = false)
        {
            return FullName(staff.FirstName, staff.LastName, upper, withSalutation ? staff.Gender : null);
        }

        public static string FullName(this Student student, bool upper = true)
        {
            return FullName(student.FirstName, student.LastName, upper);
        }

        public static string FullName(this Person person, bool upper = true, bool withSalutation = false)
        {
            return FullName(person.FirstName, person.LastName, upper, withSalutation ? person.Gender : null);
        }

    }
}
