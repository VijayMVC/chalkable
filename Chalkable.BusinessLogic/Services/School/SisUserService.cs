using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISisUserService
    {
        void Add(IList<SisUser> users);
        void Edit(IList<SisUser> users);
        void Delete(IList<int> ids);
        IList<SisUser> GetAll();
        SisUser GetById(int id);
    }

    public class SisUserService : SchoolServiceBase, ISisUserService
    {
        public SisUserService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<SisUser> users)
        {
            using (var uow = Update())
            {
                var da = new SisUserDataAccess(uow);
                da.Insert(users);
                uow.Commit();
            }
        }

        public void Edit(IList<SisUser> users)
        {
            using (var uow = Update())
            {
                var da = new SisUserDataAccess(uow);
                da.Update(users);
                uow.Commit();
            }
        }

        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {
                var da = new SisUserDataAccess(uow);
                da.Delete(ids);
                uow.Commit();
            }
        }

        public IList<SisUser> GetAll()
        {
            using (var uow = Update())
            {
                var da = new SisUserDataAccess(uow);
                return da.GetAll();
            }
        }

        public SisUser GetById(int id)
        {
            using (var uow = Update())
            {
                var da = new SisUserDataAccess(uow);
                return da.GetById(id);
            }
        }
    }
}