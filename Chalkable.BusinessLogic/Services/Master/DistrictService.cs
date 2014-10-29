using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDistrictService
    {
        District GetByIdOrNull(Guid id);
        District Create(Guid id, string name, string sisUrl, string sisRedirectUrl, string sisUserName, string sisPassword, string timeZone);
        PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue);
        PaginatedList<DistrictSyncStatus> GetDistrictsSyncStatus(int start = 0, int count = int.MaxValue);
        void Update(District district);
        void DeleteDistrict(Guid id);
        bool IsOnline(Guid id);
    }

    public class DistrictService : MasterServiceBase, IDistrictService
    {
        private const int MAX_SYNC_TIME_DEFAULT = 1800;
        private const int SYNC_LOG_FLUSH_SIZE_DEFAULT = 100;
        private const int SYNC_HISTORY_DAYS_DEFAULT = 15;

        public DistrictService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }

        public District Create(Guid id, string name, string sisUrl, string sisRedirectUrl, string sisUserName, string sisPassword, string timeZone)
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
                        Id = id,
                        Name = name,
                        SisUrl = sisUrl,
                        SisRedirectUrl = sisRedirectUrl,
                        SisUserName = sisUserName,
                        SisPassword = sisPassword,
                        TimeZone = timeZone,
                        MaxSyncTime = MAX_SYNC_TIME_DEFAULT,
                        SyncLogFlushSize = SYNC_LOG_FLUSH_SIZE_DEFAULT,
                        SyncHistoryDays = SYNC_HISTORY_DAYS_DEFAULT
                    };
                da.Insert(res);
                uow.Commit();
            }
            using (var unitOfWork = new UnitOfWork(Settings.GetSchoolConnectionString(server, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                da.CreateDistrictDataBase(res.Id.ToString(), Settings.SchoolTemplateDbName);
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

        public PaginatedList<DistrictSyncStatus> GetDistrictsSyncStatus(int start = 0, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                return new DistrictDataAccess(uow).GetSyncStatuses(start, count);
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

        public void DeleteDistrict(Guid id)
        {
            var district = GetByIdOrNull(id);
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                da.Delete(id);
                uow.Commit();
            }

            using (var unitOfWork = new UnitOfWork(Settings.GetSchoolConnectionString(district.ServerUrl, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                da.DeleteDistrictDataBase(district.Id.ToString());
            }
        }

        public bool IsOnline(Guid id)
        {
            var d = GetByIdOrNull(id);
            using (var unitOfWork = new UnitOfWork(Settings.GetSchoolConnectionString(d.ServerUrl, "Master"), false))
            {
                var da = new DistrictDataAccess(unitOfWork);
                var l = da.GetOnline(new[] { id });
                return (l.Count > 0) ;
            }
        }
    }
}