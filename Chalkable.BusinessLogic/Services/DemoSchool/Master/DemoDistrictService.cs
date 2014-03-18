using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
  
    public class DemoDistrictService : DemoMasterServiceBase, IDistrictService
    {
        public const int DEMO_EXPIRE_HOURS = 3;

        public DemoDistrictService(IServiceLocatorMaster serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public District Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, string timeZone)
        {
            throw new NotImplementedException();
        }

        public District Create(string name, string sisUrl, string sisUserName, string sisPassword, string timeZone, Guid? sisDistrictId)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                return new DistrictDataAccess(uow).GetPage(start, count);
            }
        }

        public IList<District> GetDistricts(bool? demo, bool? usedDemo = null)
        {
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetDistricts(demo, usedDemo);
            }
        }

        public void Update(District district)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DistrictDataAccess(uow).Update(district);
                uow.Commit();
            }
        }
        
        public District GetByIdOrNull(Guid id)
        {
            throw new NotImplementedException();
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetByIdOrNull(id);
            }
        }
        
        public void CreateDemo()
        {
            throw new NotImplementedException();
            District district;
            var prefix = Guid.NewGuid().ToString().Replace("-", "");
            var prototypeId = Guid.Parse(PreferenceService.Get(Preference.DEMO_DISTRICT_ID).Value);
            var prototype = GetByIdOrNull(prototypeId);
            var server = prototype.ServerUrl;
            IList<Data.Master.Model.School> schools;
            IList<User> oldUsers;
            using (var uow = Update())
            {
                district = new District
                {
                    Id = Guid.NewGuid(),
                    Name = prototype.Name + prefix,
                    ServerUrl = server,
                    TimeZone = prototype.TimeZone,
                    DemoPrefix = prefix
                };
                var da = new DistrictDataAccess(uow);
                da.Insert(district);
                var schoolDa = new Data.Master.DataAccess.SchoolDataAccess(uow);
                var  oldSchools = schoolDa.GetSchools(prototypeId, 0, int.MaxValue);
                schools = oldSchools.Select(x => new Data.Master.Model.School
                    {
                        Id = Guid.NewGuid(),
                        DistrictRef = district.Id,
                        LocalId = x.LocalId,
                        District = district,
                        Name = x.Name
                    }).ToList();
                schoolDa.Insert(schools);
                oldUsers = new UserDataAccess(uow).GetUsers(prototypeId);
                uow.Commit();
            }

            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                da.CreateDistrictDataBase(district.Id.ToString(), prototype.Id.ToString());
            }

            //wait for online for an hour
            for (int i = 0; i < 3600; i++)
            {
                if (IsOnline(district.Id))
                    break;
                Thread.Sleep(10000);
            }


            IList<Person> persons;
            IList<SchoolPerson> schoolPersons;
            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, district.Id.ToString()), true))
            {
                var da = new PersonDataAccess(unitOfWork, Context.SchoolLocalId);
                persons = da.GetAll();
                foreach (var person in persons)
                {
                    person.Email = district.DemoPrefix + person.Email;
                }
                da.Update(persons);
                var spDa = new SchoolPersonDataAccess(unitOfWork);
                schoolPersons = spDa.GetSchoolPersons(null, null, null);
                unitOfWork.Commit();
            }
            IList<User> users = new List<User>();
            IList<SchoolUser> schoolUsers = new List<SchoolUser>();
            var demoUserPassword = PreferenceService.Get(Preference.DEMO_USER_PASSWORD).Value;
            foreach (var person in persons)
            {
                var oldUser = oldUsers.First(x => x.LocalId == person.Id);
                var u = ServiceLocator.UserService.CreateSchoolUser(person.Email, demoUserPassword, district.Id, person.Id, oldUser.SisUserName);
                users.Add(u);
                var sps = schoolPersons.Where(x => x.PersonRef == person.Id).ToList();
                foreach (var schoolPerson in sps)
                {
                    schoolUsers.Add(new SchoolUser
                    {
                        Id = Guid.NewGuid(),
                        SchoolRef = schools.First(x => x.LocalId == schoolPerson.SchoolRef).Id,
                        Role = schoolPerson.RoleRef,
                        UserRef = u.Id,
                    });
                }
            }
            ServiceLocator.UserService.AssignUserToSchool(schoolUsers);
        }

        public IList<District> GetDemoDistrictsToDelete()
        {
            return new List<District>();
        }

        
        public District UseDemoDistrict()
        {
           
            var district = new District
            {
                DemoPrefix = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid()
            };
            return district;
            throw new NotImplementedException();
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var notUsedDemo = da.GetDistricts(true, false).FirstOrDefault();
                if (notUsedDemo == null) return null;
                notUsedDemo.LastUseDemo = DateTime.UtcNow.ConvertFromUtc(notUsedDemo.TimeZone);
                da.Update(notUsedDemo);
                uow.Commit();
                return notUsedDemo;
            }
        }

        public void DeleteDistrict(Guid id)
        {
            throw new NotImplementedException();
            var district = GetByIdOrNull(id);
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                da.Delete(id);
                uow.Commit();
            }

            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, district.ServerUrl, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                da.DeleteDistrictDataBase(district.Id.ToString());
            }
        }

        public bool IsOnline(Guid id)
        {
            throw new NotImplementedException();
            var d = GetByIdOrNull(id);
            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, d.ServerUrl, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                var l = da.GetOnline(new[] { id });
                return (l.Count > 0) ;
            }
        }
    }
}