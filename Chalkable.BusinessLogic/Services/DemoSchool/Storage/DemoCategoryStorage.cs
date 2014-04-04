using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoCategoryStorage:BaseDemoStorage<Guid, Category>
    {
        public DemoCategoryStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(Category res)
        {
            if (!data.ContainsKey(res.Id))
                data.Add(res.Id, res);
        }

        public void Update(Category res)
        {
            if (data.ContainsKey(res.Id))
            {
                data[res.Id] = res;
            }
        }

        public void Delete(Guid id)
        {
            data.Remove(id);
        }

        public PaginatedList<Category> GetPage(int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Setup()
        {
            
        }
    }
}
