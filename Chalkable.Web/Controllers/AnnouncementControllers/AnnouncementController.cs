﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.UserTracking;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementController : AnnouncementBaseController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Create(int? classId)
        {
            if (BaseSecurity.IsDistrictAdmin(Context))
                return Redirect<AdminAnnouncementController>(c => c.CreateAdminAnnouncement(null));
            
            var draft = SchoolLocator.AnnouncementFetchService.GetLastDraft();
            if (draft != null)
            {
                if (draft is ClassAnnouncement)
                {
                    var classAnn = draft as ClassAnnouncement;
                    var classAnnType = classId.HasValue ? null : classAnn.ClassAnnouncementTypeRef;
                    classId = classId ?? classAnn.ClassRef;
                    var classAnnTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId.Value);
                    if(classAnnTypes.Count > 0)
                        return Redirect<ClassAnnouncementController>(c => c.CreateClassAnnouncement(classAnnType, classId.Value, null));
                }
                if (draft is LessonPlan)
                {
                    classId = classId ?? (draft as LessonPlan).ClassRef;
                    return Redirect<LessonPlanController>(x => x.CreateLessonPlan(classId));
                }
                
                if (draft is SupplementalAnnouncement)
                {
                    var supplementedAnn = draft as SupplementalAnnouncement;
                    var classAnnType = classId.HasValue ? null : supplementedAnn.ClassAnnouncementTypeRef;
                    classId = classId ?? (draft as SupplementalAnnouncement).ClassRef;
                    
                    var classAnnTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId.Value);
                    if (classAnnTypes.Count > 0)
                        return Redirect<SupplementalAnnouncementController>(c => c.CreateSupplemental(classId.Value, null, classAnnType));
                }
            }
            if(classId.HasValue)
                return Redirect<ClassAnnouncementController>(c => c.CreateClassAnnouncement(null, classId.Value, null));
            return Json(null, 7);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Edit(int announcementId, int? announcementType)
        {
            var viewData = PrepareFullAnnouncementViewData(announcementId, announcementType);
            var res = new CreateAnnouncementViewData { Announcement = viewData };
            return Json(res, 6);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Delete(int announcementId, int? announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum?)announcementType).DeleteAnnouncement(announcementId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult DeleteDrafts(int personId, int announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum)announcementType).DeleteDrafts(personId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult Read(int announcementId, int? announcementType)
        {
            var res = PrepareFullAnnouncementViewData(announcementId, announcementType, true);
            
            //TODO: implement this later
            /*
            Currently admin has no rigths to edit lessonplans and activities even
            if he is owner. He can edit this only from teacher portal.
            --------------------------------------------------------------------------*/
            if((res.LessonPlanData != null || res.ClassAnnouncementData != null || res.SupplementalAnnouncementData != null) && BaseSecurity.IsDistrictAdmin(Context))
                res.IsOwner = false;
            //------------------------------------------------------------------------
            

            MasterLocator.UserTrackingService.OpenedAnnouncement(Context.Login, res.AnnouncementTypeName, res.Title, res.PersonName);
            return Json(res, 20);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult AttachSettings(int announcementId, int? announcementType)
        {
            Trace.Assert(Context.PersonId.HasValue);

            var assesmentId = (!ApplicationSecurity.HasStudyCenterAccess(Context) && !ApplicationSecurity.HasAssessmentEnabled(Context)) ? null : MasterLocator.ApplicationService.GetAssessmentId();
            var type = (AnnouncementTypeEnum?)announcementType ?? AnnouncementTypeEnum.Class;
            var canAddStandard = SchoolLocator.GetAnnouncementService(type).CanAddStandard(announcementId);
            var isAppEnabled = BaseSecurity.IsDistrictOrTeacher(Context) && Context.SCEnabled;
            var isFileCabinetEnabled = Context.Role == CoreRoles.TEACHER_ROLE; //only teacher can use file cabinet for now
            
            var apps = MasterLocator.ApplicationService.GetApplications(live:true).ToList();
            apps = apps.Where(app => MasterLocator.ApplicationService.HasExternalAttachMode(app)).ToList();
            return Json(AttachSettingsViewData.Create(assesmentId, canAddStandard, isAppEnabled, isFileCabinetEnabled, apps));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Complete(int announcementId, int announcementType, bool? complete)
        {
            if (!complete.HasValue)
            {
                var prev = SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum)announcementType).GetAnnouncementDetails(announcementId).Complete;
                complete = !prev;
            }
            SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum)announcementType).SetComplete(announcementId, complete.Value);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Done(int? classId, int option, int? annType)
        {
            SetCompleteByOptions(classId, option, annType, true);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult UnDone(int? classId, int option, int? annType)
        {
            SetCompleteByOptions(classId, option, annType, false);
            return Json(true);
        }

        private void SetCompleteByOptions(int? classId, int option, int? annType, bool complete)
        {
            MarkDoneOptions mdo = (MarkDoneOptions)option;
            if (!annType.HasValue)
            {
                SchoolLocator.AdminAnnouncementService.SetComplete(classId, mdo, complete);
                SchoolLocator.LessonPlanService.SetComplete(classId, mdo, complete);
                SchoolLocator.ClassAnnouncementService.SetComplete(classId, mdo, complete);
                SchoolLocator.SupplementalAnnouncementService.SetComplete(classId, mdo, complete);
            }
            else SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum)annType).SetComplete(classId, mdo, complete);
        }


        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult SubmitStandardsToAnnouncement(int announcementId, int? announcementType, IntList standardIds)
        {
            var service = SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum?) announcementType);
            service.SubmitStandardsToAnnouncement(announcementId, standardIds);
            return Json(PrepareFullAnnouncementViewData(announcementId, announcementType));
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult AddStandard(int announcementId, int standardId, int? announcementType)
        {
            var standard = SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum?)announcementType).AddAnnouncementStandard(announcementId, standardId);
            MasterLocator.UserTrackingService.AttachedStandard(Context.Login, standard.Name);
            return Json(PrepareFullAnnouncementViewData(announcementId, announcementType));
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult RemoveStandard(int announcementId, int standardId, int? announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementTypeEnum?)announcementType).RemoveStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId, announcementType));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AnnouncementAttributesList(bool? activeOnly)
        {
            var res = SchoolLocator.AnnouncementAttributeService.GetList(activeOnly);
            return Json(AnnouncementAttributeViewData.Create(res));
        }

        protected ActionResult EditTitle(int announcementId, AnnouncementTypeEnum announcementType, string title, Func<string, bool> existsAction)
        {
            if (!existsAction(title))
            {
                SchoolLocator.GetAnnouncementService(announcementType).EditTitle(announcementId, title);
                return Json(true);
            }
            return Json(false);
        }

        protected CreateAnnouncementViewData PrepareCreateAnnouncementViewData(AnnouncementDetails annDetails)
        {
            return new CreateAnnouncementViewData
            {
                Announcement = PrepareAnnouncmentViewDataForEdit(annDetails),
                IsDraft = annDetails.IsDraft,
            };
        }

        protected DateTime GenerateDefaultExpiresDate(DateTime? expiresDate)
        {
            return expiresDate ?? Context.NowSchoolYearTime;
        }

        protected void TrackNewItemCreate(AnnouncementDetails ann, Action<IUserTrackingService, int, int > trackNewItemCreated)
        {
            trackNewItemCreated(MasterLocator.UserTrackingService, ann.ApplicationCount, ann.AttachmentsCount);
            if (ann.ApplicationCount > 0)
            {
                var apps = ann.AnnouncementApplications.Select(x => x.Id.ToString()).ToList();
                MasterLocator.UserTrackingService.AttachedApp(Context.Login, apps);
            }
            if (ann.AttachmentsCount > 0)
            {
                var docs = ann.AnnouncementAttachments.Select(x => x.Attachment.Name).ToList();
                MasterLocator.UserTrackingService.AttachedDocument(Context.Login, docs);
            }
        }

        [AuthorizationFilter("Teacher")]
        public async Task<ActionResult> AdjustDates(IList<AnnouncementInputModel> announcements, int shift, int classId)
        {
            
            if (announcements == null || announcements.Count == 0)
                return Json(true);

            //If any conflicts occurred
            
            var caIds = announcements.Where(x => x.AnnouncementType == (int)AnnouncementTypeEnum.Class)
                .Select(x => x.AnnouncementId).ToList();

            SchoolLocator.ClassAnnouncementService.AdjustDates(caIds, shift, classId);
            
            var adjustLpsTask = Task.Factory.StartNew(() =>
            {
                var ids = announcements.Where(x => x.AnnouncementType == (int)AnnouncementTypeEnum.LessonPlan)
                    .Select(x => x.AnnouncementId).ToList();

                SchoolLocator.LessonPlanService.AdjustDates(ids, shift, classId);
            });

            var adjustSuppAnnTask = Task.Factory.StartNew(() =>
            {
                var ids = announcements.Where(x => x.AnnouncementType == (int)AnnouncementTypeEnum.Supplemental)
                    .Select(x => x.AnnouncementId).ToList();

                SchoolLocator.SupplementalAnnouncementService.AdjustDates(ids, shift, classId);
            });

            await adjustLpsTask;
            await adjustSuppAnnTask;

            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> Copy(CopyAnnouncementsInputModel inputModel)
        {
            inputModel.Announcements = inputModel.Announcements ?? new List<AnnouncementInputModel>();

            var classAnnouncementCopyTask = Task.Factory.StartNew(() => {
                var ids = inputModel.Announcements
                    .Where(x => x.AnnouncementType == (int) AnnouncementTypeEnum.Class)
                    .Select(x => x.AnnouncementId)
                    .ToList();

                return SchoolLocator.ClassAnnouncementService.Copy(ids, inputModel.FromClassId, inputModel.ToClassId, inputModel.StartDate);
            });

            var lessonPlanCopyTask = Task.Factory.StartNew(() => {
                var ids = inputModel.Announcements
                        .Where(x => x.AnnouncementType == (int)AnnouncementTypeEnum.LessonPlan)
                        .Select(x => x.AnnouncementId)
                        .ToList();

                return SchoolLocator.LessonPlanService.Copy(ids, inputModel.FromClassId, inputModel.ToClassId, inputModel.StartDate);
            });

            var res = new List<CopyAnnouncementResultViewData>();
            res.AddRange((await lessonPlanCopyTask).Select(x=> new CopyAnnouncementResultViewData
            {
                AnnouncementType = (int)AnnouncementTypeEnum.LessonPlan,
                AnnouncementId = x
            }));
            res.AddRange((await classAnnouncementCopyTask).Select(x => new CopyAnnouncementResultViewData
            {
                AnnouncementType = (int)AnnouncementTypeEnum.Class,
                AnnouncementId = x
            }));
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Announcement })]
        public ActionResult GetAnnouncementRecipients(int announcementId, int? studentFilter, int start = 0, int count = int.MaxValue)
        {
            var filterEnum = (StudentFilterEnum) (studentFilter ?? 0);
            var res = SchoolLocator.LessonPlanService.GetAnnouncementRecipientPersons(announcementId, filterEnum, start, count);
            return Json(res.Select(ShortPersonViewData.Create));
        }
    }

}