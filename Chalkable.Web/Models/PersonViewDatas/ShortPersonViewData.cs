using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class ShortPersonViewData
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        protected ShortPersonViewData(Person person)
        {
            Id = person.Id;
            DisplayName = person.FullName;
            FullName = person.FullName;
            FirstName = person.FirstName;
            LastName = person.LastName;
            Gender = person.Gender;
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