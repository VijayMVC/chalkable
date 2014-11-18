using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPersonStorage:BaseDemoIntStorage<Person>
    {
        public DemoPersonStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public PersonQueryResult GetPersons(PersonQuery query)
        {
            var persons = data.Select(x => x.Value);

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
                        var teacherRef = Storage.ClassStorage.GetClassesComplex(new ClassQuery
                        {
                            ClassId = query.ClassId
                        }).Classes.Select(x => x.PrimaryTeacherRef).First();

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

            if (query.CallerRoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                var studentGradeLevelId =
                    Storage.StudentSchoolYearStorage.GetAll(query.CallerId.Value).Select(x => x.GradeLevelRef);
                persons = persons.Where(x => x.Id == query.CallerId ||
                                             (x.RoleRef == CoreRoles.TEACHER_ROLE.Id ||
                                              x.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id ||
                                              x.RoleRef == CoreRoles.ADMIN_EDIT_ROLE.Id ||
                                              x.RoleRef == CoreRoles.ADMIN_VIEW_ROLE.Id)
                                             ||
                                             (x.RoleRef == CoreRoles.STUDENT_ROLE.Id &&
                                              Storage.StudentSchoolYearStorage.Exists(
                                                  new List<int>(studentGradeLevelId), x.Id)));
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
                FirstLoginDate = person.FirstLoginDate,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Gender = person.Gender,
                Id = person.Id,
                Email = DemoStorage.BuildDemoEmail(person.Id, Storage.Context.DistrictId.ToString()),
                LastMailNotification = person.LastMailNotification,
                RoleRef = person.RoleRef,
                Salutation = person.Salutation
            };

            if (personDetails.AddressRef.HasValue)
                personDetails.Address = Storage.AddressStorage.GetById(personDetails.AddressRef.Value);

            personDetails.Phones = Storage.PhoneStorage.GetAll(personDetails.Id);

            personDetails.StudentSchoolYears = Storage.StudentSchoolYearStorage.GetAll(personDetails.Id);

            return personDetails;
        }

        public Person GetPerson(PersonQuery personQuery)
        {
            return GetPersons(personQuery).Persons.First();
        }

        public IList<Person> GetPersonsByPhone(string phone)
        {
            return Storage.PhoneStorage.GetUsersByPhone(phone);
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
                },
                {
                    DemoUserService.BuildDemoUserName(CoreRoles.ADMIN_GRADE_ROLE.LoweredName, prefix), 
                    new KeyValuePair<int, int>(DemoSchoolConstants.AdminGradeId, CoreRoles.ADMIN_GRADE_ROLE.Id)
                },
                {
                    DemoUserService.BuildDemoUserName(CoreRoles.ADMIN_EDIT_ROLE.LoweredName, prefix), 
                    new KeyValuePair<int, int>(DemoSchoolConstants.AdminEditId, CoreRoles.ADMIN_EDIT_ROLE.Id)
                },
                {
                    DemoUserService.BuildDemoUserName(CoreRoles.ADMIN_VIEW_ROLE.LoweredName, prefix), 
                    new KeyValuePair<int, int>(DemoSchoolConstants.AdminViewId, CoreRoles.ADMIN_VIEW_ROLE.Id)
                }
            };
            var res = localIds[user.Login];
            roleId = res.Value;
            return res.Key;
        }
    }


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
        public IEnumerable<int> GradeLevelIds { get; set; }
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
}
