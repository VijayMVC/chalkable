using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class CategoryController : ChalkableController
    {
        [Authorize]
        public ActionResult List(int? start, int? count)
        {
            var categories = MasterLocator.CategoryService.ListCategories(start ?? 0, count ?? 10);
            return Json(categories.Transform(CategoryVeiwData.Create));
        }
        [Authorize]
        public ActionResult GetInfo(Guid categoryId)
        {
            var category = MasterLocator.CategoryService.GetById(categoryId);
            return Json(CategoryVeiwData.Create(category));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Add(string name, string description)
        {
            var category = MasterLocator.CategoryService.Add(name, description);
            return Json(CategoryVeiwData.Create(category));
        }
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Edit(Guid categoryId, string name, string description)
        {
            var category = MasterLocator.CategoryService.Edit(categoryId, name, description);
            return Json(CategoryVeiwData.Create(category));
        }
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid categoryId)
        {
            MasterLocator.CategoryService.Delete(categoryId);
            return List(null, null);
        }

    }
}