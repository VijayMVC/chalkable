using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoCategoryStorage:BaseDemoGuidStorage<Category>
    {
        public DemoCategoryStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public PaginatedList<Category> GetPage(int start, int count)
        {
            var categories = data.Select(x => x.Value).ToList();
            return new PaginatedList<Category>(categories, start / count, count, categories.Count);
        }

        public override void Setup()
        {
            
        }
    }
}
