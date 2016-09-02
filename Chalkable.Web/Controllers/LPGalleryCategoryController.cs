using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class LPGalleryCategoryController : ChalkableController
    {
        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult CreateCategory(string name)
        {
            if (!SchoolLocator.LPGalleryCategoryService.Exists(name, null))
            {
                SchoolLocator.LPGalleryCategoryService.Add(name);
                return Json(true);
            }
            return Json(false);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult UpdateCategory(int categoryId, string name)
        {

            if (!SchoolLocator.LPGalleryCategoryService.Exists(name, categoryId))
            {
                SchoolLocator.LPGalleryCategoryService.Edit(categoryId, name);
                return Json(true);
            }
            return Json(false);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult DeleteCategory(int categoryId)
        {
            SchoolLocator.LPGalleryCategoryService.Delete(categoryId);
            return Json(true);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult ListCategories()
        {
            var res = SchoolLocator.LPGalleryCategoryService.GetList();
            return Json(LPGalleryCategoryViewData.Create(res));
        }
    }
}