using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICategoryService
    {
        PaginatedList<Category> ListCategories(int start = 0, int count = int.MaxValue);
        Category GetById(Guid id);
        Category Add(string name, string description);
        Category Edit(Guid id, string name, string description);
        void Delete(Guid id);
    }

    //TODO: needs test
    public class CategoryService : MasterServiceBase, ICategoryService
    {
        public CategoryService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public PaginatedList<Category> ListCategories(int start = 0, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                return new CategoryDataAccess(uow).GetPage(start, count);
            }
        }
        public Category GetById(Guid id)
        {
            using (var uow = Read())
            {
                return new CategoryDataAccess(uow).GetById(id);
            }
        }

        public Category Add(string name, string description)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var res = new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Description = description
                    };
                new CategoryDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }

        public Category Edit(Guid id, string name, string description)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new CategoryDataAccess(uow);
                var res = da.GetById(id);
                res.Description = description;
                res.Name = name;
                da.Update(res);
                uow.Commit();
                return res;
            }
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new CategoryDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

    }
}
