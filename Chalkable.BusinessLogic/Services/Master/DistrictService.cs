using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDistrictService
    {
        District GetById(Guid id);
        void Delete(Guid id);
        District Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, ImportSystemTypeEnum sisSystemType);
        PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue);
        void Update(District district);
    }

    public class DistrictService : MasterServiceBase, IDistrictService
    {
        public DistrictService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }

        public District Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, ImportSystemTypeEnum sisSystemType)
        {
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var res = new District
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        DbName = dbName,
                        SisUrl = sisUrl,
                        SisUserName = sisUserName,
                        SisPassword = sisPassword,
                        SisSystemType = (int)sisSystemType
                    };
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }

        public PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                return new DistrictDataAccess(uow).GetPage(start, count);
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

        public void Delete(Guid id)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DistrictDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public District GetById(Guid id)
        {
            //TODO: check 
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetById(id);
            }
        }
    }
}