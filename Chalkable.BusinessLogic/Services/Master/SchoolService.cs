using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ISchoolService
    {
        Data.Master.Model.School GetById(Guid id);
        Data.Master.Model.School GetByIdOrNull(Guid id);
        Data.Master.Model.School Create(Guid districtId, string name, IList<UserInfo> principals);
        void CreateEmpty();
        PaginatedList<Data.Master.Model.School> GetSchools(Guid? districtId, int start = 0, int count = int.MaxValue);
        IList<Data.Master.Model.School> GetSchools(bool? empty, bool? demo, bool? usedDemo = null);
        Data.Master.Model.School UseDemoSchool();
        SisSync GetSyncData(Guid schoolId);
        void SetSyncData(SisSync sisSync);
        void Update(Data.Master.Model.School school);
        void CreateDemo();
        IList<Data.Master.Model.School> GetDemoSchoolsToDelete();
        void DeleteSchool(Guid id);
    }

    public class SchoolService : MasterServiceBase, ISchoolService
    {
        public const int DEMO_EXPIRE_HOURS = 3;

        public SchoolService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void CreateEmpty()
        {
            string server;
            Data.Master.Model.School school;
            using (var uow = Update())
            {
                server = FindServer(uow);
                school = new Data.Master.Model.School
                {
                    DistrictRef = null,
                    Id = Guid.NewGuid(),
                    Name = "Empty",
                    ServerUrl = server,
                    IsEmpty = true,
                    TimeZone = "UTC"
                };
                var da = new SchoolDataAccess(uow);
                da.Create(school);
                uow.Commit();
            }

            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, "Master"), false))
            {
                var da = new SchoolDataAccess(unitOfWork);
                da.CreateSchoolDataBase(school.Id.ToString(), Settings.Configuration.SchoolTemplateDataBase);
            }
        }
        
        public District CreateDistrict(string name)
        {
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var res = new District {Id = new Guid(), Name = name};
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid? districtId, int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetSchools(districtId, start, count);
            }
        }


        public IList<Data.Master.Model.School> GetSchools(bool? empty, bool? demo, bool? usedDemo = null)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetSchools(empty, demo, usedDemo);
            }
        }

        public SisSync GetSyncData(Guid schoolId)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetSyncData(schoolId);
            }
        }

        public void SetSyncData(SisSync sisSync)
        {
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                da.SetSyncData(sisSync);
                uow.Commit();
            }
        }

        public void Update(Data.Master.Model.School school)
        {
            using (var uow = Update())
            {
                new SchoolDataAccess(uow).Update(school);
                uow.Commit();
            }
        }

        public void CreateDemo()
        {
            Data.Master.Model.School school;
            var prefix = Guid.NewGuid().ToString().Replace("-", "");
            var prototypeId = Guid.Parse(PreferenceService.Get(Preference.DEMO_SCHOOL_ID).Value);
            var prototype = GetById(prototypeId);
            var server = prototype.ServerUrl;
            using (var uow = Update())
            {
                
                school = new Data.Master.Model.School
                {
                    DistrictRef = prototype.DistrictRef,
                    Id = Guid.NewGuid(),
                    Name = prototype.Name + prefix,
                    ServerUrl = server,
                    IsEmpty = false,
                    TimeZone = prototype.TimeZone,
                    DemoPrefix = prefix
                };
                var da = new SchoolDataAccess(uow);
                da.Create(school);
                uow.Commit();
            }

            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, "Master"), false))
            {
                var da = new SchoolDataAccess(unitOfWork);
                da.CreateSchoolDataBase(school.Id.ToString(), prototype.Id.ToString());
            }

            //wait for online for an hour
            for (int i = 0; i < 3600; i++)
            {
                using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, "Master"), false))
                {
                    var da = new SchoolDataAccess(unitOfWork);
                    var l = da.GetOnline(new[] { school.Id });
                    if (l.Count > 0)
                        break;
                }
                Thread.Sleep(1000);
            }
                        
            
            IList<Person> users;
            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, server, school.Id.ToString()), false))
            {
                var da = new PersonDataAccess(unitOfWork);
                da.RepopulateDemoIds(prefix);
                users = da.GetAll();
            }
            foreach (var person in users)
            {
                ServiceLocator.UserService.CreateSchoolUser(person.Email, "tester", school.Id, CoreRoles.GetById(person.RoleRef).Name, person.Id);
            }
        }
        
        public IList<Data.Master.Model.School> GetDemoSchoolsToDelete()
        {
            var expires = DateTime.UtcNow.AddHours(-DEMO_EXPIRE_HOURS);
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetSchoolsToDelete(expires);
                
            }
        }

        public void DeleteSchool(Guid id)
        {
            var school = GetById(id);
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                da.Delete(id);
                uow.Commit();
            }

            using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, school.ServerUrl, "Master"), false))
            {
                var da = new SchoolDataAccess(unitOfWork);
                da.DeleteSchoolDataBase(school.Id.ToString());
            }
        }

        public Data.Master.Model.School Create(Guid districtId, string name, IList<UserInfo> principals)
        {
            Data.Master.Model.School school = GetEmpty();
            if (school == null)
                return null;
            var schoolSl = ServiceLocator.SchoolServiceLocator(school.Id);
            foreach (var principal in principals)
            {
                schoolSl.PersonService.Add(principal.Login, principal.Password, principal.FirstName, principal.LastName, CoreRoles.ADMIN_GRADE_ROLE.Name, principal.Gender, principal.Salutation, principal.BirthDate, null);
            }
            school.IsEmpty = false;
            school.DistrictRef = districtId;
            Update(school);
            return school;
        }

        public Data.Master.Model.School GetById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetById(id);
            }
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetByIdOrNull(id);
            }
        }

        private static string FindServer(UnitOfWork uow)
        {
            var da = new SchoolDataAccess(uow);
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

        private Data.Master.Model.School GetEmpty()
        {
            var allEmpty = new Dictionary<string, List<Guid>>();
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                var candidats = da.GetEmpty();
                foreach (var candidat in candidats)
                {
                    if (allEmpty.ContainsKey(candidat.ServerUrl))
                        allEmpty[candidat.ServerUrl].Add(candidat.Id);
                    else
                        allEmpty.Add(candidat.ServerUrl, new List<Guid> { candidat.Id });
                }
            }

            foreach (var serversEmpty in allEmpty)
            {
                using (var unitOfWork = new UnitOfWork(string.Format(Settings.SchoolConnectionStringTemplate, serversEmpty.Key, "Master"), false))
                {
                    var da = new SchoolDataAccess(unitOfWork);
                    var online = da.GetOnline(serversEmpty.Value);
                    if (online.Count > 0)
                        return GetById(Guid.Parse(online[0]));
                }
            }
            return null;
        }


        public Data.Master.Model.School UseDemoSchool()
        {
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                var notUsedDemo = da.GetSchools(null, true, false).FirstOrDefault();
                if (notUsedDemo == null) return null;
                notUsedDemo.LastUseDemo = DateTime.UtcNow.ConvertFromUtc(notUsedDemo.TimeZone);
                da.Update(notUsedDemo);
                uow.Commit();
                return notUsedDemo;
            }
        }
    }
}