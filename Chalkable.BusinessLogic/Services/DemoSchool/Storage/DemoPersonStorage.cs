using System;
using System.Collections.Generic;
using System.Globalization;
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
            //public int? CallerId { get; set; }
            //public int CallerRoleId { get; set; }

            /*	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId   or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and exists(select * from StudentSchoolYear where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId))))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
		)*/

            var persons = data.Select(x => x.Value);


            /*if(@callerRoleId = 3)
begin
	-- todo : needs currentSchoolYearId for getting right grade level 
	set @callerGradeLevelId = (select  top 1 GradeLevelRef from StudentSchoolYear where StudentRef = @callerId)
end*/

            //var callerGradeLevelId = Storage.StudentSchoolYearStorage.GetAll(

            if (query.PersonId.HasValue)
                persons = persons.Where(x => x.Id == query.PersonId);

            if (query.RoleId.HasValue)
                persons = persons.Where(x => x.RoleRef == query.RoleId);

            if (!string.IsNullOrWhiteSpace(query.StartFrom))
                persons = persons.Where(x => String.Compare(x.LastName, query.StartFrom, true, CultureInfo.InvariantCulture) >= 0);

            if (query.TeacherId.HasValue)
            {
                var classPersons = Storage.ClassPersonStorage.GetAll();
                var classes = classPersons.Select(x => Storage.ClassStorage.GetById(x.ClassRef)).ToList();
                var clsIds = classes.Where(x => x.TeacherRef == query.TeacherId).Select(x => x.Id).ToList();
                var personIds = classPersons.Where(x => clsIds.Contains(x.ClassRef)).Select(x => x.PersonRef).ToList();
                persons = persons.Where(x => personIds.Contains(x.Id));
            }

            if (query.ClassId.HasValue)
            {
                if (query.RoleId.HasValue)
                {
                    if (query.RoleId == CoreRoles.TEACHER_ROLE.Id)
                    {
                        var teacherRef = Storage.ClassStorage.GetClassesComplex(new ClassQuery
                        {
                            ClassId = query.ClassId
                        }).Classes.Select(x => x.TeacherRef).First();

                        persons = persons.Where(x => x.Id == teacherRef);
                    }

                    if (query.RoleId == CoreRoles.STUDENT_ROLE.Id)
                    {
                        var personIds = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
                        {
                            ClassId = query.ClassId
                        }).Select(x => x.PersonRef).ToList();

                        persons = persons.Where(x => personIds.Contains(x.Id));
                    }
                }
            }

            //caller role id

            if (!string.IsNullOrEmpty(query.Filter))
                persons = persons.Where(x => x.FullName.Contains(query.Filter));
            
            if (query.GradeLevelIds != null)
            {
                var gradeLevelIds = Storage.GradeLevelStorage.GetAll().Select(x => x.Id).ToList();
                persons = persons.Where(x => x.RoleRef == CoreRoles.STUDENT_ROLE.Id && Storage.StudentSchoolYearStorage.Exists(gradeLevelIds, x.Id) || 
                    x.RoleRef == CoreRoles.TEACHER_ROLE.Id && Storage.ClassStorage.Exists(gradeLevelIds, x.Id));
            }

            if (query.RoleIds != null)
                persons = persons.Where(x => query.RoleIds.Contains(x.RoleRef));


            

            persons = persons.Skip(query.Start).Take(query.Count).ToList();

            persons = query.SortType == SortTypeEnum.ByFirstName ? persons.OrderBy(x => x.FirstName) : persons.OrderBy(x => x.LastName);


            var enumerable = persons as IList<Person> ?? persons.ToList();
            return new PersonQueryResult
            {
                Persons = enumerable.ToList(),
                Query = query,
                SourceCount = data.Count
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

            personDetails.StudentSchoolYears = Storage.StudentSchoolYearStorage.GetAll(personDetails.Id);

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

        public override void Setup()
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
