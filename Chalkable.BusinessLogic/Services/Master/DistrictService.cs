using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDistrictService
    {
        District GetByIdOrNull(Guid id);
        District Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, string timeZone);
        PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue);
        IList<District> GetDistricts(bool? empty, bool? demo, bool? usedDemo = null);
        void Update(District district);
        District UseDemoDistrict();
        void CreateDemo();
        IList<District> GetDemoDistrictsToDelete();
        void DeleteDistrict(Guid id);
    }

    public class DistrictService : MasterServiceBase, IDistrictService
    {
        public const int DEMO_EXPIRE_HOURS = 3;

        public DistrictService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }

        public District Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, string timeZone)
        {
            string server;
            District res;
            using (var uow = Update())
            {
                server = FindServer(uow);
                var da = new DistrictDataAccess(uow);
                res = new District
                    {
                        ServerUrl = server,
                        Id = Guid.NewGuid(),
                        Name = name,
                        DbName = dbName,
                        SisUrl = sisUrl,
                        SisUserName = sisUserName,
                        SisPassword = sisPassword,
                        TimeZone = timeZone,
                        IsEmpty = false
                    };
                da.Insert(res);
                uow.Commit();
            }
            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                da.CreateDistrictDataBase(res.Id.ToString(), Settings.Configuration.SchoolTemplateDataBase);
            }
            return res;
        }

        public PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                return new DistrictDataAccess(uow).GetPage(start, count);
            }
        }

        public IList<District> GetDistricts(bool? empty, bool? demo, bool? usedDemo = null)
        {
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetDistricts(empty, demo, usedDemo);
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
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException(); 
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetByIdOrNull(id);
            }
        }
        
        public void CreateDemo()
        {
            District district;
            var prefix = Guid.NewGuid().ToString().Replace("-", "");
            var prototypeId = Guid.Parse(PreferenceService.Get(Preference.DEMO_DISTRICT_ID).Value);
            var prototype = GetByIdOrNull(prototypeId);
            var server = prototype.ServerUrl;
            using (var uow = Update())
            {

                district = new District
                {
                    Id = Guid.NewGuid(),
                    Name = prototype.Name + prefix,
                    ServerUrl = server,
                    IsEmpty = false,
                    TimeZone = prototype.TimeZone,
                    DemoPrefix = prefix
                };
                var da = new DistrictDataAccess(uow);
                da.Insert(district);
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
                using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, "Master"), false))
                {
                    var da = new DistrictDataAccess(unitOfWork);
                    var l = da.GetOnline(new[] { district.Id });
                    if (l.Count > 0)
                        break;
                }
                Thread.Sleep(1000);
            }


            IList<Person> users;
            IList<SchoolPerson> schoolPersons;
            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, district.Id.ToString()), false))
            {
                var da = new PersonDataAccess(unitOfWork, Context.SchoolLocalId);
                users = da.GetAll();
                var spDa = new SchoolPersonDataAccess(unitOfWork);
                schoolPersons = spDa.GetSchoolPersons(null, null, null);
            }

            foreach (var person in users)
            {
                var u = ServiceLocator.UserService.CreateSchoolUser(person.Email, "tester", district.Id, person.Id);
                var p = person;
                var sps = schoolPersons.Where(x => x.PersonRef == p.Id);
                foreach (var schoolPerson in sps)
                {
                    ServiceLocator.UserService.AssignUserToSchool(u.Id, schoolPerson.SchoolRef, schoolPerson.RoleRef);
                }
            }
        }

        public IList<District> GetDemoDistrictsToDelete()
        {
            var expires = DateTime.UtcNow.AddHours(-DEMO_EXPIRE_HOURS);
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetDistrictsToDelete(expires);
            }
        }
        
        private static string FindServer(UnitOfWork uow)
        {
            var da = new DistrictDataAccess(uow);
            var serverLoading = da.CalcServersLoading();
            string s = null;
            int cnt = int.MaxValue;
            foreach (var sl in serverLoading)
            {
                if (sl.Value >= cnt) continue;
                cnt = sl.Value;
                s = sl.Key;
            }
            if (s == null)
                throw new NullReferenceException();
            return s;
        }
        
        public District UseDemoDistrict()
        {
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var notUsedDemo = da.GetDistricts(null, true, false).FirstOrDefault();
                if (notUsedDemo == null) return null;
                notUsedDemo.LastUseDemo = DateTime.UtcNow.ConvertFromUtc(notUsedDemo.TimeZone);
                da.Update(notUsedDemo);
                uow.Commit();
                return notUsedDemo;
            }
        }

        public void DeleteDistrict(Guid id)
        {
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
    }
}