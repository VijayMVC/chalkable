using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class CourseController : ChalkableController
    {
        public ActionResult List(int? start, int? count)
        {
            var res = SchoolLocator.CourseService.GetCourses(start ?? 0, count ?? DEFAULT_PAGE_SIZE);
            return Json(res.Transform(CourseViewData.Create));
        }
    }
}