using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Web.Models
{
    public class CCStandardCategoryViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CCStandardCategoryViewData Create(CommonCoreStandardCategory standardCategory)
        {
            return new CCStandardCategoryViewData
                {
                    Id = standardCategory.Id,
                    Name = standardCategory.Name
                };
        }

        public static IList<CCStandardCategoryViewData> Create(IList<CommonCoreStandardCategory> standardCategories)
        {
            return standardCategories.Select(Create).ToList();
        }
    }
}