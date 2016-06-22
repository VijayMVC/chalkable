using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Api.SampleApp.Logic;
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
            , IEnumerable<StandardInfo> standards
            , string contentId)
        {
            await Task.Delay(0);

            if (mode == Settings.SYSADMIN_MODE)
            {
                CurrentUser = SchoolPerson.SYSADMIN;
                return RedirectToAction("Index", "SysAdmin");
            }

            switch (mode)
            {
                case Settings.ATTACH_MODE:
                    if (announcementId.HasValue && announcementType.HasValue)
                    {
                        return RedirectToAction("Index", "Document", new RouteValueDictionary
                        {
                            {"announcementId", announcementId.Value},
                            {"announcementType", announcementType.Value},
                            {"attributeId", attributeId }
                        });
                    }
                    break;

                case Settings.EDIT_MODE:
                    return RedirectToAction("Attach", "Teacher", new RouteValueDictionary
                    {
                        {"announcementApplicationId", announcementApplicationId},
                        {"contentId", contentId }
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


        protected override PaginatedList<ApplicationContent> GetApplicationContents(IList<StandardInfo> standardInfos, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;

            var res = ContentStorage.GetStorage().GetContents().OrderBy(x=>x.ContentId).ToList();
            return new PaginatedList<ApplicationContent>(res, start.Value, count.Value);
        }
      

    }
}