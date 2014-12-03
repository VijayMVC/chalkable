using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class CategoryViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private CategoryViewData() { }

        public static CategoryViewData Create(Category category)
        {
           return new CategoryViewData
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
        public static IList<CategoryViewData> Create(IList<Category> categories)
        {
            return categories.Select(Create).ToList();
        }
    }
}