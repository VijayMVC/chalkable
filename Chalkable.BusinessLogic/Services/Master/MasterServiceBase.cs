using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services.Master
{
    public class MasterServiceBase
    {
        protected IServiceLocatorMaster ServiceLocator { get; private set; }
        protected UserContext Context
        {
            get { return ServiceLocator.Context; }
        }

        public MasterServiceBase(IServiceLocatorMaster serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        protected UnitOfWork Read()
        {
            return ServiceLocator.DbService.GetUowForRead();
        }

        protected UnitOfWork Update()
        {
            return ServiceLocator.DbService.GetUowForUpdate();
        }
    }
}
