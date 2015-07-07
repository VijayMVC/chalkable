using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class LPGalleryCategoryController : ChalkableController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult CreateCategory(string name)
        {
            var res = SchoolLocator.LPGalleryCategoryService.Add(name);
            return Json(LPGalleryCategoryViewData.Create(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult UpdateCategory(int categoryId, string name)
        {
            var res = SchoolLocator.LPGalleryCategoryService.Edit(categoryId, name);
            return Json(LPGalleryCategoryViewData.Create(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult DeleteCategory(int categoryId)
        {
            SchoolLocator.LPGalleryCategoryService.Delete(categoryId);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ListCategories()
        {
            var res = SchoolLocator.LPGalleryCategoryService.GetList();
            return Json(LPGalleryCategoryViewData.Create(res));
        }
    }
}