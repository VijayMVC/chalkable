using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    public class AnnouncementBaseController : ChalkableController
    {
        protected AnnouncementViewData PrepareFullAnnouncementViewData(int announcementId, bool needsAllAttachments = true, bool isRead = false)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var annDetails = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (annDetails.SisActivityId.HasValue)
            {
                annDetails.StudentAnnouncements = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(announcementId);
                annDetails.GradingStudentsCount = annDetails.StudentAnnouncements.Count(x=>x.IsGraded);
            }
            var teachersIds = SchoolLocator.ClassService.GetClassTeachers(annDetails.ClassRef, null).Select(x=>x.PersonRef).ToList();
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(annDetails.AnnouncementAttachments, teachersIds);
            var annViewData = AnnouncementDetailedViewData.Create(annDetails, SchoolLocator.GradingStyleService.GetMapper(), Context.PersonId.Value, attInfo);
            annViewData.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, announcementId);
            annViewData.ApplicationsCount = annViewData.Applications.Count;
            if (annViewData.Applications.Count > 0)
            {
                annViewData.ApplicationName = annViewData.Applications.Count == 1
                                  ? annViewData.Applications.First().Name
                                  : annViewData.Applications.Count.ToString();
            }

            if (isRead && annDetails.State == AnnouncementState.Created)
            {
                IList<string> appNames = new List<string>();  
                var stAnnouncements = annDetails.StudentAnnouncements;
                annViewData.AutoGradeApps = appNames;
                if (SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                {
                    annViewData.Dropped = stAnnouncements.Count > 0 && stAnnouncements[0].Dropped;
                    annViewData.Exempt = stAnnouncements.Count > 0 && stAnnouncements[0].Exempt;
                }
                
                if (stAnnouncements.Count > 0 && annDetails.GradableType)
                    annViewData.StudentAnnouncements = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, annDetails, attInfo);
            }
            if (!isRead)
            {
                annViewData.CanAddStandard = SchoolLocator.AnnouncementService.CanAddStandard(announcementId);
                if (annViewData.Standards != null && annViewData.Standards.Count > 0)
                {
                    var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(Context.NowSchoolYearTime.Date);
                    if(mp == null)
                        throw new NoMarkingPeriodException();
                    var codes = annDetails.AnnouncementStandards.Where(x=>!string.IsNullOrEmpty(x.Standard.CCStandardCode))
                        .Select(x => x.Standard.CCStandardCode).ToList();
                    annViewData.SuggestedApps = AppMarketController.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                                                              Context.PersonId.Value, annDetails.ClassRef, codes, mp.Id);
                }

            }

            return annViewData;
        }

        protected IList<ApplicationForAttachViewData> PrepareSuggestedAppsForAnnouncementViewData(AnnouncementDetails announcementDetails)
        {
            if (announcementDetails.AnnouncementStandards != null && announcementDetails.AnnouncementStandards.Count > 0)
            {
                var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(Context.NowSchoolYearTime.Date);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var codes = announcementDetails.AnnouncementStandards.Where(x => !string.IsNullOrEmpty(x.Standard.CCStandardCode))
                    .Select(x => x.Standard.CCStandardCode).ToList();
                return AppMarketController.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                                                          Context.PersonId.Value, announcementDetails.ClassRef, codes, mp.Id);
            }
            return new List<ApplicationForAttachViewData>();
        }
    }
}