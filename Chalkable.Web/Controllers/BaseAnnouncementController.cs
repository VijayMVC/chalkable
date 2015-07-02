﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
            var ownersIds = GetAnnouncementOwnersIds(ann);
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(ann.AnnouncementAttachments, MasterLocator.CrocodocService, ownersIds);
            var annView = (AnnouncementDetailedViewData)PrepareAnnouncementViewData(ann, attInfo);
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
            var annView = (AnnouncementDetailedViewData)PrepareAnnouncementViewData(ann);
            annView.CanAddStandard = SchoolLocator.AnnouncementService.CanAddStandard(ann.Id);
            if (annView.Standards != null && annView.Standards.Count > 0)
            {
                var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(Context.NowSchoolYearTime.Date);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var abIds = ann.AnnouncementStandards.Where(x => x.Standard.AcademicBenchmarkId.HasValue)
                    .Select(x => x.Standard.AcademicBenchmarkId.Value).ToList();
                if(ann.ClassRef.HasValue)
                    annView.SuggestedApps = ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                                                              Context.PersonId.Value, ann.ClassRef.Value, abIds, mp.Id);
            }
            return annView;
        }

        protected AnnouncementViewData PrepareAnnouncementViewData(AnnouncementDetails ann)
        {
            var ownersIds = GetAnnouncementOwnersIds(ann);
            var attInfo = AttachmentLogic.PrepareAttachmentsInfo(ann.AnnouncementAttachments, MasterLocator.CrocodocService, ownersIds);
            return PrepareAnnouncementViewData(ann, attInfo);
        }

        protected AnnouncementViewData PrepareAnnouncementViewData(AnnouncementDetails ann, IList<AnnouncementAttachmentInfo> attachments)
        {
            if (ann.SisActivityId.HasValue)
            {
                ann.StudentAnnouncements = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(ann.Id);
                ann.GradingStudentsCount = ann.StudentAnnouncements.Count(x => x.IsGraded);
            }
            var annViewData = AnnouncementDetailedViewData.Create(ann, Context.PersonId.Value, attachments);
            annViewData.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, ann.Id);
            annViewData.ApplicationsCount = annViewData.Applications.Count;
            annViewData.AssessmentApplicationId = MasterLocator.ApplicationService.GetAssessmentId();
            if (annViewData.Applications.Count > 0)
            {
                annViewData.ApplicationName = annViewData.Applications.Count == 1
                                  ? annViewData.Applications.First().Name
                                  : annViewData.Applications.Count.ToString();
            }
            
            if (ann.AnnouncementRecipients != null)
                annViewData.Recipients = AdminAnnouncementRecipientViewData.Create(ann.AnnouncementRecipients);
            return annViewData;
        }
        
        protected IList<ApplicationForAttachViewData> PrepareSuggestedAppsForAnnouncementViewData(AnnouncementDetails announcementDetails)
        {
            if (announcementDetails.AnnouncementStandards != null && announcementDetails.AnnouncementStandards.Count > 0 && announcementDetails.ClassRef.HasValue)
            {
                var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(Context.NowSchoolYearTime.Date);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var abIds = announcementDetails.AnnouncementStandards.Where(x => x.Standard.AcademicBenchmarkId.HasValue)
                    .Select(x => x.Standard.AcademicBenchmarkId.Value).ToList();
                return ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                                                          Context.PersonId.Value, announcementDetails.ClassRef.Value, abIds, mp.Id);
            }
            return new List<ApplicationForAttachViewData>();
        }

        private IList<int> GetAnnouncementOwnersIds(AnnouncementDetails ann)
        {
            var ownersIds = new List<int>();
            if (ann.AdminRef.HasValue)
                ownersIds.Add(ann.AdminRef.Value);
            if (ann.ClassRef.HasValue)
                ownersIds = SchoolLocator.ClassService.GetClassTeachers(ann.ClassRef.Value, null).Select(x => x.PersonRef).ToList();
            return ownersIds;
        } 
    }
}