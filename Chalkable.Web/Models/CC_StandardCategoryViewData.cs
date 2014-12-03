using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class CCStandardCategoryViewData
    {
        public Guid Id { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string Name { get; set; }

        public static CCStandardCategoryViewData Create(CC_StandardCategory standardCategory)
        {
            return new CCStandardCategoryViewData
                {
                    Id = standardCategory.Id,
                    ParentCategoryId = standardCategory.ParentCategoryRef,
                    Name = standardCategory.Name
                };
        }

        public static IList<CCStandardCategoryViewData> Create(IList<CC_StandardCategory> standardCategories)
        {
            return standardCategories.Select(Create).ToList();
        }
    }
}