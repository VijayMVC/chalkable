using System;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoCategoryStorage : BaseDemoGuidStorage<Category>
    {
        public DemoCategoryStorage()
            : base(x => x.Id)
        {
        }

        public PaginatedList<Category> GetPage(int start, int count)
        {
            var categories = data.Select(x => x.Value).ToList();
            return new PaginatedList<Category>(categories, start / count, count, categories.Count);
        }
    }

    public class DemoCategoryService : DemoMasterServiceBase, ICategoryService
    {
        private DemoCategoryStorage CategoryStorage { get; set; }
        public DemoCategoryService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            CategoryStorage = new DemoCategoryStorage();
        }

        public PaginatedList<Category> ListCategories(int start = 0, int count = Int32.MaxValue)
        {
            return CategoryStorage.GetPage(start, count);
            
        }
        public Category GetById(Guid id)
        {
            return CategoryStorage.GetById(id);
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

            CategoryStorage.Add(res);
            return res;
        }

        public Category Edit(Guid id, string name, string description)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var res = CategoryStorage.GetById(id);
            res.Description = description;
            res.Name = name;
            CategoryStorage.Update(res);
            return res;
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            CategoryStorage.Delete(id);
        }

    }
}
