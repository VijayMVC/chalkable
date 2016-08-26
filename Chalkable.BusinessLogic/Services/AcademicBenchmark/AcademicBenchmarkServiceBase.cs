using System;
using System.Collections.Generic;
using System.Data;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IAcademicBenchmarkServiceBase<TModel, TParam> where TModel : new()
    {
        void Add(IList<TModel> models);
        void Delete(IList<TModel> models);
        void Edit(IList<TModel> models);
        IList<TModel> GetAll();
        TModel GetByIdOrNull(TParam id);
        IList<TModel> GetByIds(IList<TParam> ids);
    }

    public class AcademicBenchmarkServiceBase<TModel, TParam> : IAcademicBenchmarkServiceBase<TModel, TParam> where TModel : new()
    {
        protected IAcademicBenchmarkServiceLocator ServiceLocator { get; }
        protected UserContext Context => ServiceLocator.Context;

        public AcademicBenchmarkServiceBase(IAcademicBenchmarkServiceLocator locator)
        {
            ServiceLocator = locator;
        }

        public virtual IList<TModel> GetAll()
        {
            return DoRead(u => new DataAccessBase<TModel>(u).GetAll());
        }

        public virtual TModel GetByIdOrNull(TParam id)
        {
            return DoRead(u => new DataAccessBase<TModel, TParam>(u).GetByIdOrNull(id));
        }

        public virtual IList<TModel> GetByIds(IList<TParam> ids)
        {
            return DoRead(u => new DataAccessBase<TModel, TParam>(u).GetByIds(ids));
        }

        public virtual void Add(IList<TModel> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<TModel>(u).Insert(models));
        }

        public virtual void Delete(IList<TModel> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<TModel>(u).Delete(models));
        }

        public virtual void Edit(IList<TModel> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<TModel>(u).Update(models));
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
            using (var uow = Read())
            {
                return func(uow);
            }
        }
    }
}