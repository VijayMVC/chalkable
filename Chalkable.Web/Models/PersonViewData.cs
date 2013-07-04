using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PersonViewData
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public int RoleRef { get; set; }

        public static PersonViewData Create(Person person)
        {
            return new PersonViewData
                {
                    Active = person.Active,
                    BirthDate = person.BirthDate,
                    Email = person.Email,
                    FirstName = person.FirstName,
                    Gender = person.Gender,
                    Id = person.Id,
                    LastName = person.LastName,
                    RoleRef = person.RoleRef,
                    Salutation = person.Salutation,
                };
        }

        public static IList<PersonViewData> Create(IList<Person> person)
        {
            return person.Select(Create).ToList();
        }
    }
}