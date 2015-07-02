using System;
using System.Diagnostics;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    [RequireHttps, TraceControllerFilter]
    public class AdminAnnouncementController : AnnouncementController
    {
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult CreateAdminAnnouncement(DateTime? expiresDate)
        {
            var annDetails = SchoolLocator.AdminAnnouncementService.Create(GenerateDefaultExpiresDate(expiresDate));
            return Json(PrepareCreateAnnouncementViewData(annDetails));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Save(int adminAnnouncementId, string title, string content, DateTime? expiresDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var res = SchoolLocator.AdminAnnouncementService.Edit(adminAnnouncementId, title, content, expiresDate);
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Submit(int adminAnnouncementId, string title, string content, DateTime? expiresDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            if (string.IsNullOrEmpty(title))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Announcement Title "));

            var res = SchoolLocator.AdminAnnouncementService.Edit(adminAnnouncementId, title, content, expiresDate);
            SchoolLocator.AdminAnnouncementService.Submit(adminAnnouncementId);
            SchoolLocator.AdminAnnouncementService.DeleteAnnouncements(Context.PersonId.Value);
            var adminAnn = SchoolLocator.AdminAnnouncementService.GetAdminAnnouncementById(adminAnnouncementId);
            TrackNewItemCreate(res, (s, appsCount, doscCount) => s.CreateNewAdminItem(Context.Login, adminAnn.AdminName, appsCount, doscCount));
            return Json(true, 5);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult SubmitGroupsToAnnouncement(int adminAnnouncementId, IntList groupsIds)
        {
            SchoolLocator.AdminAnnouncementService.SubmitGroupsToAnnouncement(adminAnnouncementId, groupsIds);
            return Json(true, 5);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Exists(string title, int? excludeAdminAnnouncementId)
        {
           return Json(SchoolLocator.AdminAnnouncementService.Exists(title, excludeAdminAnnouncementId));
        }
    }
}