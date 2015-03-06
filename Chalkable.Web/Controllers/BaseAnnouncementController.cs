using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    public class AnnouncementBaseController : ChalkableController
    {
        protected AnnouncementViewData PrepareFullAnnouncementViewData(int announcementId, bool forRead = false)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var annDetails = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (forRead) return PrepareFullAnnouncementViewDataForRead(annDetails);
            return PrepareAnnouncmentViewDataForEdit(annDetails);
        }

        protected AnnouncementViewData PrepareFullAnnouncementViewDataForRead(AnnouncementDetails ann)
        {
            var teachersIds = SchoolLocator.ClassService.GetClassTeachers(ann.ClassRef, null).Select(x => x.PersonRef).ToList();
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(ann.AnnouncementAttachments, teachersIds);
            var annView = (AnnouncementDetailedViewData)PrepareAnnouncmentViewData(ann, attInfo);
            if (ann.State == AnnouncementState.Created)
            {
                var stAnnouncements = ann.StudentAnnouncements;
                if (SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                {
                    annView.Dropped = stAnnouncements.Count > 0 && stAnnouncements[0].ScoreDropped;
                    annView.Exempt = stAnnouncements.Count > 0 && stAnnouncements[0].Exempt;
                }
                if (stAnnouncements.Count > 0 && ann.GradableType)
                {
                    annView.StudentAnnouncements = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, ann, attInfo);
                    var autoGrades = SchoolLocator.StudentAnnouncementService.GetAutoGradesByAnnouncementId(ann.Id);
                    annView.AutoGradeApps = AutoGradeViewData.Create(autoGrades);
                }
            }
            return annView;
        }

        protected AnnouncementViewData PrepareAnnouncmentViewDataForEdit(AnnouncementDetails ann)
        {
            var annView = (AnnouncementDetailedViewData)PrepareAnnouncmentViewData(ann);
            annView.CanAddStandard = SchoolLocator.AnnouncementService.CanAddStandard(ann.Id);
            if (annView.Standards != null && annView.Standards.Count > 0)
            {
                var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(Context.NowSchoolYearTime.Date);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var abIds = ann.AnnouncementStandards.Where(x => x.Standard.AcademicBenchmarkId.HasValue)
                    .Select(x => x.Standard.AcademicBenchmarkId.Value).ToList();
                annView.SuggestedApps = ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                                                          Context.PersonId.Value, ann.ClassRef, abIds, mp.Id);
            }
            return annView;
        }

        protected AnnouncementViewData PrepareAnnouncmentViewData(AnnouncementDetails ann)
        {
            var teachersIds = SchoolLocator.ClassService.GetClassTeachers(ann.ClassRef, null).Select(x => x.PersonRef).ToList();
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(ann.AnnouncementAttachments, teachersIds);             
            return PrepareAnnouncmentViewData(ann, attInfo);
        }

        protected AnnouncementViewData PrepareAnnouncmentViewData(AnnouncementDetails ann, IList<AnnouncementAttachmentInfo> attachments)
        {
            if (ann.SisActivityId.HasValue)
            {
                ann.StudentAnnouncements = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(ann.Id);
                ann.GradingStudentsCount = ann.StudentAnnouncements.Count(x => x.IsGraded);
            }
            var annViewData = AnnouncementDetailedViewData.Create(ann, SchoolLocator.GradingStyleService.GetMapper(), Context.PersonId.Value, attachments);
            annViewData.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, ann.Id);
            annViewData.ApplicationsCount = annViewData.Applications.Count;
            annViewData.AssessmentApplicationId = Guid.Parse(PreferenceService.Get(Preference.ASSESSMENT_APLICATION_ID).Value);
            if (annViewData.Applications.Count > 0)
            {
                annViewData.ApplicationName = annViewData.Applications.Count == 1
                                  ? annViewData.Applications.First().Name
                                  : annViewData.Applications.Count.ToString();
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
                var abIds = announcementDetails.AnnouncementStandards.Where(x => x.Standard.AcademicBenchmarkId.HasValue)
                    .Select(x => x.Standard.AcademicBenchmarkId.Value).ToList();
                return ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                                                          Context.PersonId.Value, announcementDetails.ClassRef, abIds, mp.Id);
            }
            return new List<ApplicationForAttachViewData>();
        }
    }
}