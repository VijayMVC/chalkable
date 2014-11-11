using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICommonCoreStandardService
    {
        IList<CommonCoreStandard> GetStandards();
    }

    public class CommonCoreStandardService : MasterServiceBase, ICommonCoreStandardService
    {
        public CommonCoreStandardService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<CommonCoreStandard> GetStandards()
        {
            using (var uow = Read())
            {
                return new CommonCoreStandardDataAccess(uow).GetAll();
            }
        }
    }
}
