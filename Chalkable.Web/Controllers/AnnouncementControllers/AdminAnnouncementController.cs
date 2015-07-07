using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
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
        public ActionResult Save(int adminAnnouncementId, string title, string content, DateTime? expiresDate,  IList<AnnouncementAssignedAttribute> attributes)
        {
            Trace.Assert(Context.PersonId.HasValue);
            SchoolLocator.AnnouncementAssignedAttributeService.Edit((int)AnnouncementType.Admin, adminAnnouncementId, attributes);
            var res = SchoolLocator.AdminAnnouncementService.Edit(adminAnnouncementId, title, content, expiresDate);
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Submit(int adminAnnouncementId, string title, string content, DateTime? expiresDate,  IList<AnnouncementAssignedAttribute> attributes)
        {

            Trace.Assert(Context.PersonId.HasValue);
            if (string.IsNullOrEmpty(title))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Announcement Title "));


            SchoolLocator.AnnouncementAssignedAttributeService.Edit((int)AnnouncementType.Admin, adminAnnouncementId, attributes);
            var res = SchoolLocator.AdminAnnouncementService.Edit(adminAnnouncementId, title, content, expiresDate);
            SchoolLocator.AdminAnnouncementService.Submit(adminAnnouncementId);
            SchoolLocator.AdminAnnouncementService.DeleteDrafts(Context.PersonId.Value);
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
        
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult EditTitle(int announcementId, string title)
        {
            return EditTitle(announcementId, AnnouncementType.Admin, title, t => SchoolLocator.AdminAnnouncementService.Exists(t, announcementId));
        }
    }
}