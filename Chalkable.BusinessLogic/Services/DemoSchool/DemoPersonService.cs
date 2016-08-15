using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class PersonQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int? RoleId { get; set; }
        public int? ClassId { get; set; }
        public int? TeacherId { get; set; }
        public int? PersonId { get; set; }
        public int? CallerId { get; set; }
        public int CallerRoleId { get; set; }
        public int? CallerGradeLevelId { get; set; }

        public IList<int> RoleIds { get; set; }

        public string StartFrom { get; set; }
        public string Filter { get; set; }
        public SortTypeEnum SortType { get; set; }

        public int? MarkingPeriodId { get; set; }
        public int? SchoolYearId { get; set; }

        public bool? IsEnrolled { get; set; }
        public bool OnlyMyTeachers { get; set; }

        public PersonQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            SortType = SortTypeEnum.ByLastName;
            OnlyMyTeachers = false;
        }
    }

    public class PersonQueryResult
    {
        public List<Person> Persons { get; set; }
        public int SourceCount { get; set; }
        public PersonQuery Query { get; set; }
    }

    public enum SortTypeEnum
    {
        ByFirstName = 0,
        ByLastName = 1
    }

    public class DemoPersonStorage : BaseDemoIntStorage<Person>
    {
        public DemoPersonStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoPersonService : DemoSchoolServiceBase, IPersonService
    {
        private DemoPersonStorage PersonStorage { get; set; }
        public DemoPersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            PersonStorage = new DemoPersonStorage();
        }
        
        public PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count)
        {
            var persons = PersonStorage.GetAll().AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLowerInvariant();
                persons = persons.Where(x => x.FullName().ToLowerInvariant().Contains(filter));
            }
            persons = orderByFirstName ? persons.OrderBy(x => x.FirstName) : persons.OrderBy(x => x.LastName);
            return new PaginatedList<Person>(persons.ToList(), start / count, count);
        }

        public CoreRole GetPersonRole(int personId)
        {
            throw new NotImplementedException();
        }

        public void PrepareToDelete(IList<Person> persons)
        {
            throw new NotImplementedException();
        }

        public PersonQueryResult GetPersons(PersonQuery query)
        {
            query.CallerId = Context.PersonId;
            query.CallerRoleId = Context.Role.Id;

            var persons = PersonStorage.GetData().Select(x => x.Value);

            if (query.PersonId.HasValue)
                persons = persons.Where(x => x.Id == query.PersonId);

            if (query.RoleId.HasValue)
                persons = persons.Where(x => x.RoleRef == query.RoleId);

            if (!string.IsNullOrWhiteSpace(query.StartFrom))
                persons = persons.Where(x => String.Compare(x.LastName, query.StartFrom, true, CultureInfo.InvariantCulture) >= 0);

            if (query.TeacherId.HasValue)
            {
                var classPersons = ((DemoClassService)ServiceLocator.ClassService).GetClassPersons();
                var classes = classPersons.Select(x => ServiceLocator.ClassService.GetById(x.ClassRef)).ToList();
                var clsIds = classes.Where(x => x.PrimaryTeacherRef == query.TeacherId).Select(x => x.Id).ToList();
                var personIds = classPersons.Where(x => clsIds.Contains(x.ClassRef)).Select(x => x.PersonRef).ToList();
                persons = persons.Where(x => personIds.Contains(x.Id));
            }

            if (query.ClassId.HasValue)
            {
                if (query.RoleId.HasValue)
                {
                    if (query.RoleId == CoreRoles.TEACHER_ROLE.Id)
                    {
                        var teacherRef = ServiceLocator.ClassService.GetClassDetailsById(query.ClassId.Value).PrimaryTeacherRef;
                        persons = persons.Where(x => x.Id == teacherRef);
                    }

                    if (query.RoleId == CoreRoles.STUDENT_ROLE.Id)
                    {
                        var personIds = ((DemoClassService)ServiceLocator.ClassService)
                            .GetClassPersons(query.ClassId.Value).Select(x => x.PersonRef).ToList();
                        persons = persons.Where(x => personIds.Contains(x.Id));
                    }
                }
            }

            if (query.CallerRoleId == CoreRoles.STUDENT_ROLE.Id)
            {

                var studentGradeLevelId =
                    ((DemoSchoolYearService) ServiceLocator.SchoolYearService).GetStudentGradeLevel(query.CallerId.Value);
                persons = persons.Where(x => x.Id == query.CallerId ||
                                             (x.RoleRef == CoreRoles.TEACHER_ROLE.Id ||
                                              x.RoleRef == CoreRoles.DISTRICT_ADMIN_ROLE.Id)
                                             ||
                                             (x.RoleRef == CoreRoles.STUDENT_ROLE.Id &&
                                              ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GradeLevelExists(studentGradeLevelId, x.Id)));
            }

            if (query.CallerRoleId == CoreRoles.CHECKIN_ROLE.Id)
            {
                persons = persons.Where(x => x.Id == query.CallerId || x.RoleRef == CoreRoles.STUDENT_ROLE.Id);
            }



            if (!string.IsNullOrEmpty(query.Filter))
            {
                var filter = query.Filter.ToLowerInvariant();
                persons = persons.Where(x => x.FullName().ToLowerInvariant().Contains(filter));
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
                SourceCount = enumerable.Count
            };
        }

        public void Add(IList<Person> persons)
        {
            PersonStorage.Add(persons);
        }

        public void Add(IList<Person> persons, IList<SchoolPerson> assignments)
        {
            throw new NotImplementedException();
        }

        public void UpdateForImport(IList<Person> persons)
        {
            throw new NotImplementedException();
        }
        
        public void Delete(IList<Person> persons)
        {
            PersonStorage.Delete(persons);
        }

        public void DeleteSchoolPersons(IList<SchoolPerson> schoolPersons)
        {
            throw new NotImplementedException();
        }

        public static int GetPersonDataForLogin(User user, out int roleId)
        {
            var prefix = user.DistrictRef.ToString();
            var localIds = new Dictionary<string, KeyValuePair<int, int>>
            {
                {
                    DemoUserService.BuildDemoUserName(CoreRoles.TEACHER_ROLE.LoweredName, prefix), 
                    new KeyValuePair<int, int>(DemoSchoolConstants.TeacherId, CoreRoles.TEACHER_ROLE.Id)
                },
                {
                    DemoUserService.BuildDemoUserName(CoreRoles.STUDENT_ROLE.LoweredName, prefix), 
                    new KeyValuePair<int, int>(DemoSchoolConstants.Student1, CoreRoles.STUDENT_ROLE.Id)
                }
            };
            var res = localIds[user.Login];
            roleId = res.Value;
            return res.Key;
        }

        public PaginatedList<Person> GetPaginatedPersons(PersonQuery query)
        {
            var res = GetPersons(query);
            return new PaginatedList<Person>(res.Persons, query.Start/query.Count, query.Count, res.SourceCount);
        }

        public Person GetPerson(int id)
        {
            return PersonStorage.GetById(id);
        }

        public PersonDetails GetPersonDetails(int personId, int? schoolId)
        {
            var person = GetPersons(new PersonQuery()
            {
                PersonId = personId,
                CallerId = Context.PersonId ?? 0,
                CallerRoleId = Context.Role.Id,
                Count = 1
            }).Persons.First();

            var personDetails = new PersonDetails
            {
                Active = true,
                AddressRef = person.AddressRef,
                BirthDate = person.BirthDate,
                FirstLoginDate = person.FirstLoginDate,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Gender = person.Gender,
                Id = person.Id,

                LastMailNotification = person.LastMailNotification,
                RoleRef = person.RoleRef,
                Salutation = person.Salutation,
                PersonEmails = new List<PersonEmail>()
            };

            personDetails.PersonEmails.Add(new PersonEmail
            {
                Description = "default demo email",
                EmailAddress = DemoPersonEmailService.BuildDemoEmail(person.Id, Context.DistrictId.ToString()),
                IsListed = true,
                IsPrimary = true,
                PersonRef = personDetails.Id
            });

            if (personDetails.AddressRef.HasValue)
                personDetails.Address = ((DemoAddressService)ServiceLocator.AddressService).GetAddress(personDetails.AddressRef.Value);

            personDetails.Phones = ServiceLocator.PhoneService.GetPhones(personDetails.Id);

            personDetails.StudentSchoolYears = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetStudentAssignments(personDetails.Id);

            return personDetails;
        }


        public Person Edit(int localId, string email, string firstName,
            string lastName, string gender, string salutation, DateTime? birthDate, int? addressId)
        {
            throw new NotImplementedException();
        }


        public void ActivatePerson(int id)
        {
            if (BaseSecurity.IsDistrictAdminOrCurrentPerson(id, Context))
                throw new ChalkableSecurityException();
            var person = GetPerson(id);
            person.Active = true;
            PersonStorage.Update(person);
        }

        public void EditEmailForCurrentUser(int personId, string email, out string error)
        {
            throw new NotImplementedException();
        }

        public IList<Person> GetAll()
        {
            return PersonStorage.GetAll();
        }
        
        public void ProcessPersonFirstLogin(int id)
        {
            if (BaseSecurity.IsDistrictAdminOrCurrentPerson(id, Context))
                throw new ChalkableSecurityException();
            var person = GetPerson(id);
            if (person.FirstLoginDate.HasValue) return;
            person.FirstLoginDate = Context.NowSchoolTime;
            PersonStorage.Update(person);
        }

        public Person GetPerson(PersonQuery personQuery)
        {
            return GetPersons(personQuery).Persons.First();
        }

        public IList<Person> GetPersonsByPhone(string phone)
        {
            throw new NotImplementedException();
        }

        public List<Person> GetByClassId(int classId)
        {
            return GetPersons(new PersonQuery
            {
                ClassId = classId
            }).Persons;
        }

        public List<Person> GetTeacherStudents(int teacherId)
        {
            return GetPersons(new PersonQuery
            {
                TeacherId = teacherId
            }).Persons;
        }
    }
}
