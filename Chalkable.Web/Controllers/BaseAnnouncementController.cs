using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    public class AnnouncementBaseController : ChalkableController
    {
        protected AnnouncementViewData PrepareFullAnnouncementViewData(int announcementId, bool needsAllAttachments = true, bool isRead = false)
        {
            var annDetails = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (annDetails.SisActivityId.HasValue)
            {
                annDetails.StudentAnnouncements = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(announcementId);
                annDetails.GradingStudentsCount = annDetails.StudentAnnouncements.Count(x=>x.IsGraded);
            }
            //if(CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
            //    annDetails.AnnouncementAttachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(annDetails.Id);
            
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(annDetails.AnnouncementAttachments);
            var annViewData = AnnouncementDetailedViewData.Create(annDetails, SchoolLocator.GradingStyleService.GetMapper(), Context.UserLocalId.Value, attInfo);
            annViewData.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, announcementId);
            if (isRead && annDetails.State == AnnouncementState.Created)
            {
                var ids = new HashSet<Guid>();
                IList<string> appNames = new List<string>();  
                //TODO : think about appNames 
                //foreach (var sa in annDetails.StudentAnnouncements)
                //{
                //    var appRef = sa.ApplicationRef;
                //    if (appRef.HasValue && !ids.Contains(appRef.Value))
                //    {
                //        var app = applications.First(x => x.Id == appRef);
                //        ids.Add(appRef.Value);
                //        appNames.Add(app.Name);
                //    }
                //}
                var stAnnouncements = annDetails.StudentAnnouncements;
                annViewData.autoGradeApps = appNames;
                if (SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                    annViewData.Dropped = stAnnouncements.Count > 0 && stAnnouncements[0].Dropped;

                if (stAnnouncements.Count > 0 && annDetails.GradableType)
                    annViewData.StudentAnnouncements = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, annDetails, attInfo);
            }
            return annViewData;
        }
    }
}