using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    public class AnnouncementBaseController : ChalkableController
    {
        protected AnnouncementViewData PrepareFullAnnouncementViewData(Guid announcementId, bool needsAllAttachments = true, bool isRead = false)
        {
            var annDetails = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(annDetails.AnnouncementAttachments);
            var annViewData = AnnouncementDetailedViewData.Create(annDetails, SchoolLocator.GradingStyleService.GetMapper(), SchoolLocator.Context.UserId, attInfo);
            if (isRead && annDetails.State == AnnouncementState.Created)
            {
                //var ids = new HashSet<int>();
                //IList<string> appNames = new List<string>();
                
                //TODO: application
                //foreach (var sa in studentAnnInfo)
                //{
                //    var appRef = sa.StudentAnnouncement.ApplicationRef;
                //    //if (appRef.HasValue && !ids.Contains(appRef.Value))
                //    //{
                //    //    appNames.Add(SchoolLocator.ApplicationService.GetApplicationById(appRef.Value, false).Name);
                //    //}
                //}
                var stAnnouncements = annDetails.StudentAnnouncements;
                //annViewData.autoGradeApps = appNames;
                if (SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                    annViewData.Dropped = stAnnouncements.Count > 0 && stAnnouncements[0].Dropped;

                if (stAnnouncements.Count > 0 && annDetails.GradableType)
                    annViewData.StudentAnnouncements = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, annDetails, attInfo);
            }
            return annViewData;
        }
    }
}