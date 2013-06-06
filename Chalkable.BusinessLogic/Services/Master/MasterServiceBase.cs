using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services.Master
{
    public class MasterServiceBase
    {
        protected IServiceLocatorMaster ServiceLocator { get; private set; }
        public MasterServiceBase(IServiceLocatorMaster serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        protected UnitOfWork Read()
        {
            return new UnitOfWork(ServiceLocator.Context.ConnectionString, false);
        }

        protected UnitOfWork Update()
        {
            return new UnitOfWork(ServiceLocator.Context.ConnectionString, true);
        }
    }
}
