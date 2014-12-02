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
            if (person.RoleRef > 0)
            {
                Role = RoleViewData.Create(CoreRoles.GetById(person.RoleRef));
            }
        }
        public static ShortPersonViewData Create(Person person)
        {
            return new ShortPersonViewData(person);
        }
        public static IList<ShortPersonViewData> Create(IList<Person> persons)
        {
            return persons.Select(Create).ToList();
        }
    }
}