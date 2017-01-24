using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class ShortPersonViewData
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int SchoolId { get; set; }
        public RoleViewData Role { get; set; }

        protected ShortPersonViewData(){}

        protected ShortPersonViewData(Person person)
        {
            Id = person.Id;
            DisplayName = person.DisplayName();
            FullName = person.FullName();
            FirstName = person.FirstName;
            LastName = person.LastName;
            Gender = person.Gender;
            SchoolId = person.SchoolRef;
            if (person.RoleRef > 0)
            {
                Role = RoleViewData.Create(CoreRoles.GetById(person.RoleRef));
            }
        }

        protected ShortPersonViewData(Student student)
        {
            Id = student.Id;
            DisplayName = student.DisplayName();
            FullName = student.FullName();
            FirstName = student.FirstName;
            LastName = student.LastName;
            Gender = student.Gender;
            Role = RoleViewData.Create(CoreRoles.STUDENT_ROLE);
        }

        public static ShortPersonViewData Create(Person person)
        {
            return new ShortPersonViewData(person);
        }
        public static IList<ShortPersonViewData> Create(IList<Person> persons)
        {
            return persons.Select(Create).ToList();
        }

        public static ShortPersonViewData Create(Student student)
        {
            return new ShortPersonViewData(student);
        }

        public static IList<ShortPersonViewData> Create(IList<Student> students)
        {
            return students.Select(Create).ToList();
        }
    }
}