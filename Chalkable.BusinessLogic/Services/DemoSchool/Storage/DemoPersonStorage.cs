using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPersonStorage
    {
        public PersonQueryResult GetPersons(PersonQuery query)
        {
            var res = new PersonQueryResult();
            res.Persons = GetPersonsList();
            res.Query = query;
            return res;
        }

        private List<Person> GetPersonsList()
        {
            var res = new List<Person>();
            res.Add(new Person
            {
                BirthDate = null,
                Active = false,
                AddressRef = null,
                Email = "e96ef526fe974703bec2592d977b2115user1195_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com",
                Id = 1195,
                FirstName = "ROCKY",
                LastName = "STEIN",
                Gender = "F",
                RoleRef = 2
            });

            res.Add(new Person
            {
                BirthDate = new DateTime(1998, 11, 27),
                Active = false,
                AddressRef = null,
                Email = "e96ef526fe974703bec2592d977b2115user19_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com",
                Id = 19,
                FirstName = "KAYE",
                LastName = "BURGESS",
                Gender = "F",
                RoleRef = 3
            });

            res.Add(new Person
            {
                BirthDate = null,
                Active = true,
                AddressRef = null,
                Salutation = "Mr.",
                Email = "e96ef526fe974703bec2592d977b2115user2735_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com",
                Id = 2375,
                FirstName = "rosteradmin",
                LastName = "rosteradmin",
                Gender = null,
                RoleRef = 5
            });

            return res;
        }

        public PersonDetails GetPersonDetails(int id, int i, int id1)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string email, int id)
        {
            throw new NotImplementedException();
        }

        public IList<Person> Update(List<Person> res)
        {
            throw new NotImplementedException();
        }

        public void Add(Person person)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<Person> persons)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Person GetPerson(PersonQuery personQuery)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            throw new NotImplementedException();
        }

        public void Update(Person res)
        {
            throw new NotImplementedException();
        }

        public IList<Person> GetPersonsByPhone(string phone)
        {
            throw new NotImplementedException();
        }
    }
}
