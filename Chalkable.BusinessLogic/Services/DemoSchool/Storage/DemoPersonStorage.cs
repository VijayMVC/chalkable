﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
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


            if (query.GradeLevelIds != null)
                persons = persons.Where(x => query.GradeLevelIds.Contains(x.Id));

            if (query.RoleIds != null)
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

            var personDetails = new PersonDetails
            {
                Active = true,
                AddressRef = person.AddressRef,
                BirthDate = person.BirthDate,
                Email = person.Email,
                FirstLoginDate = person.FirstLoginDate,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Gender = person.Gender,
                Id = person.Id,
                LastMailNotification = person.LastMailNotification,
                LastPasswordReset = person.LastPasswordReset,
                RoleRef = person.RoleRef,
                Salutation = person.Salutation
            };

            if (personDetails.AddressRef.HasValue)
                personDetails.Address = Storage.AddressStorage.GetById(personDetails.AddressRef.Value);

            personDetails.Phones = Storage.PhoneStorage.GetAll(personDetails.Id);

        //    personDetails.StudentSchoolYears = Storage.StudentSchoolYearStorage.GetAll()

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
            return res;
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
            Add(new Person
            {
                Active = true,
                Email = PreferenceService.Get("demoschool" + CoreRoles.TEACHER_ROLE.LoweredName).Value,
                Gender = "M",
                FirstName = "ROCKY",
                LastName = "STEIN",
                Id = 1195,
                RoleRef = 2
            });

            Add(new Person
            {
                BirthDate = new DateTime(1998, 11, 27),
                Active = true,
                Email = PreferenceService.Get("demoschool" + CoreRoles.STUDENT_ROLE.LoweredName).Value,
                Id = 1196,
                FirstName = "KAYE",
                LastName = "BURGESS",
                Gender = "F",
                RoleRef = 3
            });

            Add(new Person
            {
                Active = true,
                Salutation = "Mr.",
                Email = PreferenceService.Get("demoschool" + CoreRoles.ADMIN_GRADE_ROLE.LoweredName).Value,
                Id = 1197,
                FirstName = "rosteradmin",
                LastName = "rosteradmin",
                Gender = null,
                RoleRef = 5
            });
            
        }
    }
}
