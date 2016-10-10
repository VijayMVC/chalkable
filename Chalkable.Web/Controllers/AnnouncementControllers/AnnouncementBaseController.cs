﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    public class AnnouncementBaseController : ChalkableController
    {

        protected void EnsureAnnouncementExsists(int announcementId, int? announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum?)announcementType).GetAnnouncementById(announcementId);
        }

        protected AnnouncementViewData PrepareFullAnnouncementViewData(int announcementId, int? announcementType, bool forRead = false)
        {
            var type = (AnnouncementTypeEnum?) announcementType ?? SchoolLocator.AnnouncementFetchService.GetAnnouncementType(announcementId);
            return PrepareFullAnnouncementViewData(announcementId, type, forRead);
        }

        //TODO: refactor
        protected AnnouncementViewData PrepareFullAnnouncementViewData(int announcementId, AnnouncementTypeEnum? announcementType, bool forRead = false)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var annDetails = SchoolLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            if (forRead)
                return PrepareFullAnnouncementViewDataForRead(annDetails);

            return PrepareAnnouncmentViewDataForEdit(annDetails);
        }

        protected AnnouncementViewData PrepareFullAnnouncementViewDataForRead(AnnouncementDetails ann)
        {
            var ownersIds = GetAnnouncementOwnersIds(ann);
            var annAttsInfo = SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(ann.AnnouncementAttachments, ownersIds);
            var attrAttachmentsInfo = ann.AnnouncementAttributes.Where(x=>x.Attachment != null)
                                                                .Select(x => SchoolLocator.AttachementService.TransformToAttachmentInfo(x.Attachment)).ToList(); 
            
            var annView = (AnnouncementDetailedViewData)PrepareAnnouncmentViewData(ann, annAttsInfo, attrAttachmentsInfo);
            if (ann.State == AnnouncementState.Created)
            {
                var stAnnouncements = ann.StudentAnnouncements;
                if (SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                {
                    //annView.Dropped = stAnnouncements.Count > 0 && stAnnouncements[0].ScoreDropped;
                    annView.Exempt = stAnnouncements.Count > 0 && stAnnouncements[0].Exempt;
                }
                if (stAnnouncements.Count > 0 && ann.GradableType)
                {
                    annView.StudentAnnouncements = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, ann, annAttsInfo);
                    var autoGrades = SchoolLocator.StudentAnnouncementService.GetAutoGradesByAnnouncementId(ann.Id);
                    annView.AutoGradeApps = AutoGradeViewData.Create(autoGrades);
                    //var studentAttachmentsIds = annView.StudentAnnouncements.Items.Select(x => x.Attachments.Select(y => y.Id).ToList()).SelectMany(x => x).ToList();
                    //annView.AnnouncementAttachments = annView.AnnouncementAttachments.Where(x => !studentAttachmentsIds.Contains(x.Id)).ToList();

                }
                var studentAnnouncementApplicationMeta = SchoolLocator.ApplicationSchoolService.GetStudentAnnouncementApplicationMetaByAnnouncementId(ann.Id);
                annView.StudentsAnnouncementApplicationMeta = StudentAnnouncementApplicationMetaViewData.Create(studentAnnouncementApplicationMeta);
                var comments = SchoolLocator.AnnouncementCommentService.GetList(ann.Id);
                annView.AnnouncementComments = AnnouncementCommentController.PrepareListOfCommentViewData(comments, SchoolLocator);
            }
            return annView;
        }

        protected AnnouncementViewData PrepareAnnouncmentViewDataForEdit(AnnouncementDetails ann)
        {
            Trace.Assert(Context.PersonId.HasValue);

            var annView = (AnnouncementDetailedViewData)PrepareAnnouncmentViewData(ann);
            annView.CanAddStandard = SchoolLocator.GetAnnouncementService(ann.Type).CanAddStandard(ann.Id);
            if (annView.Standards != null && annView.Standards.Count > 0)
            {
                var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(Context.NowSchoolYearTime.Date);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var abIds = ann.AnnouncementStandards.Where(x => x.Standard.AcademicBenchmarkId.HasValue)
                    .Select(x => x.Standard.AcademicBenchmarkId.Value).ToList();

                if (ann.ClassRef.HasValue)
                {
                    annView.SuggestedApps = ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator, abIds);
                    annView.AppsWithContent = ApplicationLogic.GetApplicationsWithContent(SchoolLocator, MasterLocator);
                }
            }

            if (annView.ClassAnnouncementData != null && annView.ClassId.HasValue)
            {
                var options = SchoolLocator.ClassroomOptionService.GetClassOption(annView.ClassId.Value, true);
                annView.IsAbleUseExtraCredit = options != null && options.IsAveragingMethodPoints;
            }

            //foreach (var annn in annView.AnnouncementAttachments)
            //{
            //    //annn.Attachment.IsOwner = true;
            //    annn.Attachment.IsTeacherAttachment = true;
            //}

            return annView;
        }

        protected AnnouncementViewData PrepareAnnouncmentViewData(AnnouncementDetails ann)
        {
            var ownersIds = GetAnnouncementOwnersIds(ann);
            var annAttsInfo = SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(ann.AnnouncementAttachments, ownersIds);
            var attrAttachmentsInfo = ann.AnnouncementAttributes.Where(x=>x.Attachment != null)
                .Select(x => SchoolLocator.AttachementService.TransformToAttachmentInfo(x.Attachment)).ToList(); 
            return PrepareAnnouncmentViewData(ann, annAttsInfo, attrAttachmentsInfo);
        }

        protected AnnouncementViewData PrepareAnnouncmentViewData(AnnouncementDetails ann, IList<AnnouncementAttachmentInfo> attachments, IList<AttachmentInfo> attrAttachmentInfo)
        {
            Trace.Assert(Context.PersonId.HasValue);
            if (ann.ClassAnnouncementData?.SisActivityId != null)
            {
                ann.StudentAnnouncements = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(ann.Id);
                ann.GradingStudentsCount = ann.StudentAnnouncements.Count(x => x.IsGraded);
            }
            var annViewData = AnnouncementDetailedViewData.Create(ann, Context.PersonId.Value, attachments, attrAttachmentInfo, Context.Claims);

            if (ann.StudentAnnouncements?.Count > 0)
            {
                var nonStudentsAtts = ann.AnnouncementAttachments.Where(x => ann.StudentAnnouncements.All(st => x.Attachment.PersonRef != st.StudentId)).Select(x => x.Id).ToList();
                annViewData.AnnouncementAttachments = annViewData.AnnouncementAttachments.Where(x => nonStudentsAtts.Contains(x.Id)).ToList();
            }

            annViewData.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, ann.Id);
            annViewData.ApplicationsCount = annViewData.Applications.Count;
            annViewData.AssessmentApplicationId = MasterLocator.ApplicationService.GetAssessmentId();
            if (annViewData.Applications.Count > 0)
            {
                annViewData.ApplicationName = annViewData.Applications.Count == 1
                                  ? annViewData.Applications.First().Name
                                  : annViewData.Applications.Count.ToString();
            }
            
            if (ann.AnnouncementGroups != null)
                annViewData.Recipients = AdminAnnouncementGroupViewData.Create(ann.AnnouncementGroups);

            if (ann.AdminAnnouncementStudents != null)
                annViewData.AdminAnnouncementStudents = StudentViewData.Create(ann.AdminAnnouncementStudents.Select(x => x.Student).ToList());

            return annViewData;
        }
        
        protected IList<BaseApplicationViewData> PrepareSuggestedAppsForAnnouncementViewData(AnnouncementDetails announcementDetails)
        {
            if (announcementDetails.AnnouncementStandards != null && announcementDetails.AnnouncementStandards.Count > 0 && announcementDetails.ClassRef.HasValue)
            {
                var abIds = announcementDetails.AnnouncementStandards.Where(x => x.Standard.AcademicBenchmarkId.HasValue)
                    .Select(x => x.Standard.AcademicBenchmarkId.Value).ToList();
                return ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator, abIds);
            }
            return new List<BaseApplicationViewData>();
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