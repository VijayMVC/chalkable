using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ISchoolService
    {
        Data.Master.Model.School GetById(Guid id);
        Data.Master.Model.School Create(Guid districtId, string name, IList<UserInfo> principals);
        void CreateEmpty();
        District CreateDistrict(string name);
        IList<Data.Master.Model.School> GetSchools(bool empty);
    }

    public class SchoolService : MasterServiceBase, ISchoolService
    {
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
                da.CreateSchoolDataBase(school.Id.ToString());
            }
        }
        
        public District CreateDistrict(string name)
        {
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var res = new District {Id = new Guid(), Name = name};
                da.Create(res);
                uow.Commit();
                return res;
            }
        }

        public IList<Data.Master.Model.School> GetSchools(bool empty)
        {
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetSchools(empty);
            }
        }

        public Data.Master.Model.School Create(Guid districtId, string name, IList<UserInfo> principals)
        {
            Data.Master.Model.School school = GetEmpty();
            if (school == null)
                return null;
            var schoolSl = ServiceLocator.SchoolServiceLocator(school.Id, school.Name, school.TimeZone, school.ServerUrl);
            foreach (var principal in principals)
            {
                schoolSl.PersonService.Add(principal.Login, principal.Password, principal.FirstName, principal.LastName, CoreRoles.ADMIN_GRADE_ROLE.Name, principal.Gender, principal.Salutation, principal.BirthDate);
            }
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
    }
}