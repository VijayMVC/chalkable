using System;
using System.Data;
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

        protected UnitOfWork Update(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return ServiceLocator.DbService.GetUowForUpdate(isolationLevel);
        }

        public void DoUpdate(Action<UnitOfWork> action)
        {
            using (var uow = Update())
            {
                action(uow);
                uow.Commit();
            }
        }

        public T DoRead<T>(Func<UnitOfWork, T> func)
        {
            using (var uow = Update())
            {
                return func(uow);
            }
        }
    }
}
