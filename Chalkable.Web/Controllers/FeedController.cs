using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FeedController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_FEED_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult List(int? start, int? count, bool? starredOnly, Guid? classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();

            var list = SchoolLocator.AnnouncementService.GetAnnouncements(starredOnly ?? false, start ?? 0, count ?? 10, 
                classId, null, BaseSecurity.IsAdminViewer(SchoolLocator.Context));
            return Json(list.Transform(x => AnnouncementViewData.Create(x)));
        }
    }
}