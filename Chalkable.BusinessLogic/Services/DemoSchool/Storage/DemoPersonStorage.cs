using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPersonStorage:BaseDemoStorage<int ,Person>
    {
        public DemoPersonStorage(DemoStorage storage) : base(storage)
        {
        }

        public PersonQueryResult GetPersons(PersonQuery query)
        {
            var res = new PersonQueryResult();


            ///filter persons
            res.Persons = GetAll().ToList();
            res.Query = query;
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
            if (!data.ContainsKey(person.Id))
                data[person.Id] = person;
        }

        public void Add(IList<Person> persons)
        {
            foreach (var person in persons)
            {
                Add(person);
            }
        }

        public Person GetPerson(PersonQuery personQuery)
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
