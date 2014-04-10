using System;
using System.Collections.Generic;
using System.Linq;
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

        public PaginatedList<Category> GetPage(int start, int count)
        {
            var categories = data.Select(x => x.Value).ToList();
            return new PaginatedList<Category>(categories, start / count, count, categories.Count);
        }

        public void Setup()
        {
            
        }
    }
}
