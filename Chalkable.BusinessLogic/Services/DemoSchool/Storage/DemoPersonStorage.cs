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

            //public int? ClassId { get; set; }
            //public int? TeacherId { get; set; }
            //public int? PersonId { get; set; }
            //public int? CallerId { get; set; }
            //public int CallerRoleId { get; set; }

            //public string StartFrom { get; set; }
            //public SortTypeEnum SortType { get; set; }

            var persons = data.Select(x => x.Value);

            //if (query.ClassId.HasValue)
            //    persons = persons.Where(x => x.)

            if (query.RoleId.HasValue)
                persons = persons.Where(x => x.RoleRef == query.RoleId);

            if (!string.IsNullOrEmpty(query.Filter))
                persons = persons.Where(x => x.FullName.Contains(query.Filter));
            
            if (query.PersonId.HasValue)
                persons = persons.Where(x => x.Id == query.PersonId);

            persons = persons.Where(x => query.GradeLevelIds.Contains(x.Id));

            persons = persons.Where(x => query.RoleIds.Contains(x.RoleRef));

            persons = persons.Skip(query.Start).Take(query.Count).ToList();

            var enumerable = persons as IList<Person> ?? persons.ToList();
            return new PersonQueryResult
            {
                Persons = enumerable.ToList(),
                Query = query,
                SourceCount = enumerable.Count
            };
        }

        public PersonDetails GetPersonDetails(int personId, int callerId, int callerRoleId)
        {
            var person = GetPersons(new PersonQuery()
            {
                PersonId = personId,
                CallerId = callerId,
                CallerRoleId = callerRoleId,
                Count = 1
            }).Persons.First();

            var personDetails = (PersonDetails) person;

            if (personDetails.AddressRef.HasValue)
                personDetails.Address = Storage.AddressStorage.GetById(personDetails.AddressRef.Value);

            personDetails.Phones = Storage.PhoneStorage.GetAll(personDetails.Id);

        //    personDetails.StudentSchoolYears = Storage.StudentSchoolYearStorage.GetAll()

            personDetails.Address = Storage.AddressStorage.GetById(personDetails.AddressRef.Value);
            return personDetails;
        }

        public bool Exists(string email, int id)
        {
            return data.Count(x => x.Value.Email == email && x.Value.Id == id) == 1;
        }

        public IList<Person> Update(List<Person> res)
        {
            foreach (var person in res)
            {
                Update(person);
            }
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
            return GetPersons(personQuery).Persons.First();
        }

        public void Update(Person res)
        {
            if (data.ContainsKey(res.Id))
                data[res.Id] = res;
        }

        public IList<Person> GetPersonsByPhone(string phone)
        {
            return Storage.PhoneStorage.GetUsersByPhone(phone);
        }

        public void Setup()
        {
            //add 3 persons
        }
    }
}
