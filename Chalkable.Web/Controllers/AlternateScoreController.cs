using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class AlternateScoreController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List()
        {
            var scores = AlternateScoreViewData.Create(SchoolLocator.AlternateScoreService.GetAlternateScores());
            return Json(scores, 3);
        }

        
    }
}