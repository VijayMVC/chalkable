using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDistrictService
    {
        District GetById(Guid id);
        District Create(string name);
        PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue);
        void Update(District district);
    }

    public class DistrictService : MasterServiceBase, IDistrictService
    {
        public DistrictService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }

        public District Create(string name)
        {
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var res = new District { Id = new Guid(), Name = name };
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
            using (var uow = Update())
            {
                new DistrictDataAccess(uow).Update(district);
                uow.Commit();
            }
        }

        public District GetById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new DistrictDataAccess(uow);
                return da.GetById(id);
            }
        }
    }
}