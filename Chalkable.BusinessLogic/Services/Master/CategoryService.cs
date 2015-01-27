using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
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
    
    public class CategoryService : MasterServiceBase, ICategoryService
    {
        public CategoryService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public PaginatedList<Category> ListCategories(int start = 0, int count = Int32.MaxValue)
        {
            return DoRead(u => new CategoryDataAccess(u).GetPage(start, count));
        }

        public Category GetById(Guid id)
        {
            return DoRead(u => new CategoryDataAccess(u).GetById(id));
        }

        public Category Add(string name, string description)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var res = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };
            DoUpdate(u => new CategoryDataAccess(u).Insert(res));
            return res;
        }

        public Category Edit(Guid id, string name, string description)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var res = GetById(id);
            res.Description = description;
            res.Name = name;
            DoUpdate(u => new CategoryDataAccess(u).Update(res));
            return res;
        }

        public void Delete(Guid id)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new CategoryDataAccess(u).Delete(id));
        }

    }
}
