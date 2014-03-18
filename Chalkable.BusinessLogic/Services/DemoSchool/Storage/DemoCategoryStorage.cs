using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoCategoryStorage
    {
        private Dictionary<Guid, Category> categoryData = new Dictionary<Guid, Category>(); 
        public Category GetById(Guid id)
        {
            
            if (categoryData.ContainsKey(id))
            {
                return categoryData[id];
            }
            return null;
        }

        public void Add(Category res)
        {
            if (!categoryData.ContainsKey(res.Id))
                categoryData.Add(res.Id, res);
        }

        public void Update(Category res)
        {
            if (categoryData.ContainsKey(res.Id))
            {
                categoryData[res.Id] = res;
            }
        }

        public void Delete(Guid id)
        {
            categoryData.Remove(id);
        }
    }
}
