using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoCategoryService : DemoMasterServiceBase, ICategoryService
    {
        public DemoCategoryService(IServiceLocatorMaster serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public PaginatedList<Category> ListCategories(int start = 0, int count = Int32.MaxValue)
        {
            return Storage.CategoryStorage.GetPage(start, count);
            
        }
        public Category GetById(Guid id)
        {
            return Storage.CategoryStorage.GetById(id);
        }

        public Category Add(string name, string description)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var res = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };

            Storage.CategoryStorage.Add(res);
            return res;
        }

        public Category Edit(Guid id, string name, string description)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var res = Storage.CategoryStorage.GetById(id);
            res.Description = description;
            res.Name = name;
            Storage.CategoryStorage.Update(res);
            return res;
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.CategoryStorage.Delete(id);
        }

    }
}
