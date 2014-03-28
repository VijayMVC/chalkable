using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        District Create(string name, string sisUrl, string sisUserName, string sisPassword, string timeZone, Guid? sisDistrictId);
        PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue);
        IList<District> GetDistricts(bool? demo, bool? usedDemo = null);
        void Update(District district);
        District UseDemoDistrict();
        void DeleteDistrict(Guid id);
        bool IsOnline(Guid id);
    }

    public class DistrictService : MasterServiceBase, IDistrictService
    {
        public const int DEMO_EXPIRE_HOURS = 3;

        public DistrictService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }

        public District Create(string name, string sisUrl, string sisUserName, string sisPassword, string timeZone, Guid? sisDistrictId)
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
                        DbName = string.Empty,//TODO: remove from DB?
                        SisUrl = sisUrl,
                        SisUserName = sisUserName,
                        SisPassword = sisPassword,
                        TimeZone = timeZone,
                        SisDistrictId = sisDistrictId
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
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetByIdOrNull(id);
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
            var id = Guid.NewGuid();
            return new District()
            {
                DbName = "DemoDB",
                DemoPrefix = id.ToString(),
                Id = id,
                Name = "Demo district"
            };
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

        public bool IsOnline(Guid id)
        {
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