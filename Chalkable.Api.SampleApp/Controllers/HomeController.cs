using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.API;
using Chalkable.API.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class HomeController : API.Controllers.HomeController
    {
        protected override async Task<ActionResult> ResolveAction(string mode
            , int? announcementApplicationId
            , int? studentId
            , int? announcementId
            , int? announcementType
            , int? attributeId
            , int? applicationInstallId
            , IEnumerable<StandardInfo> standards)
        {
            if (mode == Settings.SYSADMIN_MODE)
            {
                CurrentUser = SchoolPerson.SYSADMIN;
                return RedirectToAction("Index", "SysAdmin");
            }

            switch (mode)
            {
                case Settings.EDIT_MODE:
                    return RedirectToAction("Attach", "Teacher", new RouteValueDictionary
                    {
                        {"announcementApplicationId", announcementApplicationId}
                    });

                case Settings.VIEW_MODE:
                    if (CurrentUser.IsTeacher || CurrentUser.IsDistrictAdmin)
                        return RedirectToAction("ViewMode", "Teacher", new RouteValueDictionary
                        {
                            {"announcementApplicationId", announcementApplicationId}
                        });

                    if (CurrentUser.IsStudent)
                        return RedirectToAction("ViewMode", "Student", new RouteValueDictionary
                        {
                            {"announcementApplicationId", announcementApplicationId}
                        });

                    break;

                case Settings.GRADING_VIEW_MODE:
                    return RedirectToAction("GradingViewMode", "Teacher", new RouteValueDictionary
                    {
                        {"announcementApplicationId", announcementApplicationId},
                        {"studentId", studentId}
                    });

                case Settings.MY_VIEW_MODE:
                    if (CurrentUser.IsTeacher || CurrentUser.IsDistrictAdmin)
                        return RedirectToAction("Index", "Teacher");

                    return RedirectToAction("Index", CurrentUser.Role.Name);

                //special private mode for miniquiz practice
                case "practice":

                    var standard = standards.FirstOrDefault();
                    if (standard != null)
                    {
                        return RedirectToAction("Practice", "Student", new RouteValueDictionary
                        {
                            {"standardId", standard.StandardId},
                            {"commonCoreStandard", standard.CommonCoreStandard},
                            {"standardName", standard.StandardName}
                        });
                    }
                    break;
            }

            return View("NotSupported");
        }
    }
}