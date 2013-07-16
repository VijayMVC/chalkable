using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class CategoryVeiwData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private CategoryVeiwData() { }

        public static CategoryVeiwData Create(Category category)
        {
           return new CategoryVeiwData
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
        public static IList<CategoryVeiwData> Create(IList<Category> categories)
        {
            return categories.Select(Create).ToList();
        }
    }
}