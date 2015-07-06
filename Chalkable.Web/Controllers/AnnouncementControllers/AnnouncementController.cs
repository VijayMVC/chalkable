using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.UserTracking;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;

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
                if (draft.Type == AnnouncementType.Class)
                {
                    var classAnn = draft as ClassAnnouncement;
                    var classAnnType = classId.HasValue ? null : classAnn.ClassAnnouncementTypeRef;
                    classId = classId ?? classAnn.ClassRef;
                    return Redirect<ClassAnnouncementController>(c => c.CreateClassAnnouncement(classAnnType, classId.Value, null));
                }
                if (draft.Type == AnnouncementType.LessonPlan)
                    classId = classId ?? (draft as LessonPlan).ClassRef;
            }
            if(classId.HasValue)
                return Redirect<LessonPlanController>(c => c.CreateLessonPlan(classId.Value));
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
            SchoolLocator.GetAnnouncementService((AnnouncementType?)announcementType).DeleteAnnouncement(announcementId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult DeleteDrafts(int personId, int announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementType)announcementType).DeleteAnnouncements(personId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult Read(int announcementId, int? announcementType)
        {
            var res = PrepareFullAnnouncementViewData(announcementId, announcementType, true);
            MasterLocator.UserTrackingService.OpenedAnnouncement(Context.Login, res.AnnouncementTypeName, res.Title, res.PersonName);
            return Json(res, 7);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Complete(int announcementId, int announcementType, bool? complete)
        {
            if (!complete.HasValue)
            {
                var prev = SchoolLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId).Complete;
                complete = !prev;
            }
            SchoolLocator.GetAnnouncementService((AnnouncementType)announcementType).SetComplete(announcementId, complete.Value);
            return Json(true);
        }

        
        [AuthorizationFilter("Teacher")]
        public ActionResult AddStandard(int announcementId, int standardId, int? announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementType?)announcementType).AddAnnouncementStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId, announcementType));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult RemoveStandard(int announcementId, int standardId, int? announcementType)
        {
            SchoolLocator.GetAnnouncementService((AnnouncementType?)announcementType).RemoveStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId, announcementType));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AnnouncementAttributesList(bool? activeOnly)
        {
            var res = SchoolLocator.AnnouncementAttributeService.GetList(activeOnly);
            return Json(AnnouncementAttributeViewData.Create(res));
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
            return expiresDate.HasValue ? expiresDate.Value :
                                    Context.SchoolYearEndDate.HasValue ? Context.SchoolYearEndDate.Value :
                                    DateTime.MaxValue;
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
                var docs = ann.AnnouncementAttachments.Select(x => x.Name).ToList();
                MasterLocator.UserTrackingService.AttachedDocument(Context.Login, docs);
            }
        }

    }

}