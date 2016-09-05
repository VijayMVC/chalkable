using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface ILessonPlanService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(int? classId, DateTime? startDate, DateTime? endDate);
        AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int classId);
        AnnouncementDetails Edit(int lessonPlanId, int? classId, int? lpGalleryCategoryId, string title, string content, DateTime? startDate, DateTime? endDate, bool visibleForStudent
            , bool inGallery, bool discussionEnabled, bool previewCommentsEnabled, bool requireCommentsEnabled);
        PaginatedList<LessonPlan> GetLessonPlansTemplates(int? lpGalleryCategoryId, string title, int? classId, AttachmentSortTypeEnum sortType, int start, int count, AnnouncementState? state = AnnouncementState.Created); 
        IList<string> GetLastFieldValues(int classId);
        bool Exists(string title, int? excludedLessonPlaId);
        bool ExistsInGallery(string title, int? exceludedLessonPlanId);
        void SetVisibleForStudent(int lessonPlanId, bool visible);
        LessonPlan GetLessonPlanById(int lessonPlanId);

        IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? studentId, int? teacherId, bool filterByStartDate = true);
        IList<LessonPlan> GetLessonPlansbyFilter(string filter);

        IList<AnnouncementComplex> GetLessonPlansForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetLessonPlansSortedByDate(DateTime? fromDate, DateTime? toDate, bool includeFromDate, bool includeToDate, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetLessonPlansSortedByTitle(DateTime? fromDate, DateTime? toDate, string fromTitle, string toTitle, bool includeFromTitle, bool includeToTitle, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetLessonPlansSortedByClassName(DateTime? fromDate, DateTime? toDate, string fromClassName, string toClassName, bool includeFromClassName, bool includeToClassName, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null);

        LessonPlan GetLastDraft();

        void DuplicateLessonPlan(int lessonPlanId, IList<int> classIds);
        void ReplaceLessonPlanInGallery(int oldLessonPlanId, int newLessonPlanId);
        void RemoveFromGallery(int lessonPlanId);
        void CopyToGallery(int fromAnnouncementId, int toAnnouncementId);
    }

    public class LessonPlanService : BaseAnnouncementService<LessonPlan>, ILessonPlanService
    {
        public LessonPlanService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        protected override BaseAnnouncementDataAccess<LessonPlan> CreateDataAccess(UnitOfWork unitOfWork)
        {
            return CreateLessonPlanDataAccess(unitOfWork);
        }

        protected LessonPlanDataAccess CreateLessonPlanDataAccess(UnitOfWork unitOfWork, bool? ownedOnly = null)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);        
            if (BaseSecurity.IsDistrictOrTeacher(Context))
            {
                if (Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN) || Context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN))
                    return new LessonPlanForAdminDataAccess(unitOfWork, Context.SchoolYearId.Value, ownedOnly);
                if (Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM) || Context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM))
                    return new LessonPlanForTeacherDataAccess(unitOfWork, Context.SchoolYearId.Value);
            }
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return new LessonPlanForStudentDataAccess(unitOfWork, Context.SchoolYearId.Value);

            throw new ChalkableException("Not supported role for lesson plan");
        }

        public AnnouncementDetails Create(int? classId, DateTime? startDate, DateTime? endDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            BaseSecurity.EnsureAdminOrTeacher(Context);
            using (var u = Update())
            {
                var res = CreateLessonPlanDataAccess(u).Create(classId, Context.NowSchoolTime, startDate, endDate, Context.PersonId.Value, Context.SchoolYearId.Value, Context.RoleId);
                u.Commit();
                return res;
            }
        }

        public AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureStudyCenterEnabled(Context);
            BaseSecurity.EnsureAdminOrTeacher(Context);

            AnnouncementDetails res;
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(lessonPlanTemplateId, true);
            var appIds = annApps.Select(aa => aa.ApplicationRef).ToList();
            //get only simple apps
            var apps = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(appIds).Where(a=>!a.IsAdvanced).ToList();
            annApps = annApps.Where(aa => apps.Any(a => a.Id == aa.ApplicationRef)).ToList();
            
            using (var u = Update())
            {
                var da = CreateLessonPlanDataAccess(u);
                var lp = da.GetLessonPlanTemplate(lessonPlanTemplateId, Context.PersonId.Value);
                if (lp.IsDraft)
                    throw new ChalkableException("Current lesson plan in gallery is not submitted yet. You can't create lesson plan from not submitted template");

                res = da.CreateFromTemplate(lessonPlanTemplateId, Context.PersonId.Value, classId);
                var teachers = new ClassTeacherDataAccess(u).GetClassTeachers(lp.ClassRef, null).Select(x=>x.PersonRef).ToList();
                res.AnnouncementAttachments = AnnouncementAttachmentService.CopyAnnouncementAttachments(lessonPlanTemplateId, teachers, new List<int> { res.Id }, u, ServiceLocator, ConnectorLocator);
                res.AnnouncementAttributes = AnnouncementAssignedAttributeService.CopyNonStiAttributes(lessonPlanTemplateId, new List<int>{res.Id}, u, ServiceLocator, ConnectorLocator);
                res.AnnouncementApplications = ApplicationSchoolService.CopyAnnApplications(annApps, new List<int> { res.Id }, u);
                u.Commit();
            }
            return res;
        }

        public void DuplicateLessonPlan(int lessonPlanId, IList<int> classIds)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            var lessonPlan = GetLessonPlanById(lessonPlanId); // security check
            BaseSecurity.EnsureTeacher(Context);
            if (lessonPlan.IsDraft)
                throw new ChalkableException("Only submited lesson plan can be duplicated");
            
            //get announcementApplications for copying
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(lessonPlanId, true);
            var appIds = annApps.Select(aa => aa.ApplicationRef).ToList();
            //get only simple apps
            var apps = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(appIds).Where(a => !a.IsAdvanced).ToList();
            annApps = annApps.Where(aa => apps.Any(a => a.Id == aa.ApplicationRef)).ToList();
            
            using (var u = Update())
            {
                var teachers = new ClassTeacherDataAccess(u).GetClassTeachers(lessonPlan.ClassRef, null).Select(x => x.PersonRef).ToList();
                var resIds = CreateLessonPlanDataAccess(u).DuplicateLessonPlan(lessonPlanId, classIds, Context.NowSchoolYearTime);
                AnnouncementAttachmentService.CopyAnnouncementAttachments(lessonPlanId, teachers, resIds, u, ServiceLocator, ConnectorLocator);
                AnnouncementAssignedAttributeService.CopyNonStiAttributes(lessonPlanId, resIds, u, ServiceLocator, ConnectorLocator);
                ApplicationSchoolService.CopyAnnApplications(annApps, resIds, u);
                u.Commit();
            }
        }

        /// <summary>
        /// Copies lesson plans. Its attachments, attributes and 
        /// announcement applications for simple apps
        /// </summary>
        /// <returns>Copied ids. Not new!</returns>
        public override IList<int> Copy(IList<int> lessonPlanIds, int fromClassId, int toClassId, DateTime? startDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureAdminOrTeacher(Context);

            if (!ServiceLocator.ClassService.IsTeacherClasses(Context.PersonId.Value, fromClassId, toClassId))
                throw new ChalkableSecurityException("You can copy announcements only between your classes");

            if (lessonPlanIds == null || lessonPlanIds.Count == 0)
                return new List<int>();

            startDate = startDate ?? CalculateStartDateForCopying(toClassId);

            var announcements = DoRead(u => CreateLessonPlanDataAccess(u, true).GetByIds(lessonPlanIds));
            var announcementIdsToCopy = announcements.Where(x => !x.IsDraft).Select(x => x.Id).ToList();
            
            var announcementApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(announcementIdsToCopy, true);          
            var applicationIds = announcementApps.Select(x => x.ApplicationRef).ToList();
            var applications = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(applicationIds)
                .Where(x => !x.IsAdvanced).ToList();
            
            //Filter simple apps
            announcementApps = announcementApps.Where(x => applications.Any(y => y.Id == x.ApplicationRef)).ToList();

            IDictionary<int, int> fromToAnnouncementIds;
            IList<Pair<AnnouncementAttachment, AnnouncementAttachment>> annAttachmentsCopyResult;
            IList<Pair<AnnouncementAssignedAttribute, AnnouncementAssignedAttribute>> annAttributesCopyResult;
            
            using (var unitOfWork = Update())
            {
                var teachers = new ClassTeacherDataAccess(unitOfWork).GetClassTeachers(fromClassId, null).Select(x => x.PersonRef).ToList();

                fromToAnnouncementIds = CreateLessonPlanDataAccess(unitOfWork, true).CopyLessonPlansToClass(lessonPlanIds, toClassId, startDate.Value, Context.NowSchoolTime);

                annAttachmentsCopyResult = AnnouncementAttachmentService.CopyAnnouncementAttachments(fromToAnnouncementIds, teachers, unitOfWork, ServiceLocator, ConnectorLocator);
                annAttributesCopyResult  = AnnouncementAssignedAttributeService.CopyNonStiAttributes(fromToAnnouncementIds, unitOfWork, ServiceLocator, ConnectorLocator);
 
                ApplicationSchoolService.CopyAnnApplications(announcementApps, fromToAnnouncementIds.Select(x => x.Value).ToList(), unitOfWork);

                unitOfWork.Commit();
            }

            //Here we will copy all contents.
            //var attachmentsToCopy = annAttachmentsCopyResult.Transform(x => x.Attachment).ToList();
            //attachmentsToCopy.AddRange(annAttributesCopyResult.Where(x=>x.Second.Attachment != null).Transform(x=>x.Attachment));

            //ServiceLocator.AttachementService.CopyContent(attachmentsToCopy);

            return fromToAnnouncementIds.Select(x => x.Value).ToList();
        }

        public override IList<AnnouncementComplex> GetAnnouncementsByIds(IList<int> announcementIds)
        {
            return DoRead(u => InternalGetDetailses(CreateLessonPlanDataAccess(u), announcementIds))
                .Cast<AnnouncementComplex>().ToList();
        }

        public AnnouncementDetails Edit(int lessonPlanId, int? classId, int? lpGalleryCategoryId, string title, string content, DateTime? startDate, 
            DateTime? endDate, bool visibleForStudent, bool inGallery, bool discussionEnabled, bool previewCommentsEnabled, bool requireCommentsEnabled)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var lessonPlan = GetLessonPlanById(lessonPlanId); // security check

            if (inGallery && !lessonPlan.IsDraft)
            {
                if (!Context.SCEnabled)
                    throw new ChalkableException("Cannot create lesson plan template, Study Center disabled!");
                if (lpGalleryCategoryId == null)
                    throw new ChalkableException("Cannot create lesson plan template without category!");
            }

            using (var uow = Update())
            {
                var da = CreateLessonPlanDataAccess(uow);
                AnnouncementSecurity.EnsureInModifyAccess(lessonPlan, Context);
                
                if (classId != null && lessonPlan.ClassRef != classId)
                {
                    if(!lessonPlan.IsDraft)
                        throw new ChalkableException("Class can't be changed for submited lesson plan");

                    lessonPlan.ClassRef = classId;
                    //clear old data befor swiching 
                    new AnnouncementApplicationDataAccess(uow).DeleteByAnnouncementId(lessonPlan.Id);
                    new AnnouncementStandardDataAccess(uow).DeleteNotAssignedToClass(lessonPlan.Id, classId.Value);
                }
                
                lessonPlan.Title = title;
                lessonPlan.Content = content;
                lessonPlan.StartDate = startDate;
                lessonPlan.EndDate = endDate;
                lessonPlan.VisibleForStudent = visibleForStudent;
                lessonPlan.InGallery = inGallery;
                lessonPlan.GalleryOwnerRef = Context.PersonId;

                if (previewCommentsEnabled && discussionEnabled && !lessonPlan.PreviewCommentsEnabled)
                {
                    if(lessonPlan.IsSubmitted)
                        new AnnouncementCommentDataAccess(uow).HideAll(lessonPlan.Id);
                }
                lessonPlan.DiscussionEnabled = discussionEnabled;
                lessonPlan.RequireCommentsEnabled = requireCommentsEnabled;
                lessonPlan.PreviewCommentsEnabled = previewCommentsEnabled;


                lessonPlan.LpGalleryCategoryRef = lpGalleryCategoryId;

                if (lessonPlan.IsSubmitted)
                    ValidateLessonPlan(lessonPlan, da);
                da.Update(lessonPlan);
                uow.Commit();
                return InternalGetDetails(da, lessonPlanId);
            }                
        }


        public override Announcement EditTitle(int announcementId, string title)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var announcement = GetLessonPlanById(announcementId);
            if (announcement.Title != title)
            {
                using (var uow = Update())
                {
                    if (!announcement.IsOwner && !BaseSecurity.IsDistrictAdmin(Context) && announcement.GalleryOwnerRef != Context.PersonId)
                        throw new ChalkableSecurityException();
                    var da = CreateLessonPlanDataAccess(uow);
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    if (da.ExistsInGallery(title, announcement.Id) && announcement.InGallery)
                        throw new ChalkableException("The item with current title already exists in the gallery");
                    announcement.Title = title;
                    da.Update(announcement);
                    uow.Commit();
                }
            }
            return announcement;
        }

        public override void Submit(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            
            using (var u = Update())
            {
                var da = CreateLessonPlanDataAccess(u);
                var res = InternalGetDetails(da, announcementId); // da.GetDetails(announcementId, Context.PersonId.Value, Context.RoleId);
                var ln = res.LessonPlanData;

                if (ln.InGallery)
                {
                    if (!Context.SCEnabled)
                        throw new ChalkableException("Cannot create lesson plan template, Study Center disabled!");
                    if (ln.LpGalleryCategoryRef == null)
                        throw new ChalkableException("Cannot create lesson plan template without category!");
                }

                AnnouncementSecurity.EnsureInModifyAccess(res, Context);
                ValidateLessonPlan(ln, da);
                if (ln.IsDraft)
                {
                    ln.State = AnnouncementState.Created;
                    ln.Created = Context.NowSchoolTime.Date;
                    da.Update(ln);
                }
                ServiceLocator.AnnouncementAssignedAttributeService.ValidateAttributes(res.AnnouncementAttributes);
                u.Commit();
            }
        }

        private static void ValidateLessonPlan(LessonPlan lessonPlan, LessonPlanDataAccess da)
        {
            if(!lessonPlan.StartDate.HasValue)
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "LessonPlan start date "));
            if(!lessonPlan.EndDate.HasValue)
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "LessonPlan end date "));
            if(lessonPlan.StartDate > lessonPlan.EndDate)
                throw new ChalkableException("Lesson Plan is not valid. Start date is greater than end date");

            if (string.IsNullOrEmpty(lessonPlan.Title))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "LessonPlan Title "));
            if (da.ExistsInGallery(lessonPlan.Title, lessonPlan.Id) && lessonPlan.InGallery)
                throw new ChalkableException("Lesson Plan with current title already exists in the gallery");
                    
        }

        

        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateLessonPlanDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableException("You can delete only your own announcement!");
                da.Delete(announcementId);
                uow.Commit();
            }
        }

        public IList<string> GetLastFieldValues(int classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLastFields(Context.PersonId.Value, classId));
        }
        
        public bool Exists(string title, int? excludedLessonPlaId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).Exists(title, excludedLessonPlaId));
        }

        public void SetVisibleForStudent(int lessonPlanId, bool visible)
        {
            var lessonPlan = GetLessonPlanById(lessonPlanId);
            AnnouncementSecurity.EnsureInModifyAccess(lessonPlan, Context);
            if(lessonPlan.VisibleForStudent == visible) return;
            lessonPlan.VisibleForStudent = visible;
            DoUpdate(u => CreateLessonPlanDataAccess(u).Update(lessonPlan));
        }

        public LessonPlan GetLessonPlanById(int lessonPlanId)
        {
            return InternalGetAnnouncementById(lessonPlanId);
        }

        protected override AnnouncementDetails InternalGetDetails(BaseAnnouncementDataAccess<LessonPlan> dataAccess, int announcementId)
        {
            var onlyOwner = !Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);
            return InternalGetDetails(dataAccess, announcementId, onlyOwner);
        }

        public override IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete, bool ownerOnly = false)
        {
            var lps = GetLessonPlansForFeed(startDate, toDate, classId, complete);
            return  DoRead(u => InternalGetDetailses(CreateLessonPlanDataAccess(u), lps.Select(lp=>lp.Id).ToList(), ownerOnly));
        }

        //TODO: old method

        protected override void SetComplete(Announcement announcement, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            DoUpdate( u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(announcement.Id, AnnouncementTypeEnum.LessonPlan,
                Context.SchoolYearId.Value, Context.PersonId.Value, Context.RoleId, complete, null, null, null, false));
        }

        public override void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            DoUpdate(u =>
            {
                SetAnnouncementsAsComplete(u, ServiceLocator, toDate, complete);
            });
        }


        public static void SetAnnouncementsAsComplete(UnitOfWork unitOfWork, IServiceLocatorSchool locator, DateTime? toDate, bool complete)
        {

            //TODO: remove this get method later 
            //var anns = CreateLessonPlanDataAccess(unitOfWork, locator).GetLessonPlansOrderedByDate(new LessonPlansQuery
            //{
            //    RoleId = locator.Context.RoleId,
            //    ToDate = toDate,
            //    PersonId = locator.Context.PersonId,
            //}).Announcements;

            var da = new AnnouncementRecipientDataDataAccess(unitOfWork);
            da.UpdateAnnouncementRecipientData(null, AnnouncementTypeEnum.LessonPlan, locator.Context.SchoolYearId, locator.Context.PersonId, 
                locator.Context.RoleId, complete, null, null, toDate, false);
            //foreach (var ann in anns)
            //    da.UpdateAnnouncementRecipientData(ann.Id, (int)AnnouncementTypeEnum.LessonPlan, null, locator.Context.PersonId, null, complete, null, null);
        }
        
        public override bool CanAddStandard(int announcementId)
        {
            return DoRead(u => BaseSecurity.IsTeacher(Context) && CreateLessonPlanDataAccess(u).CanAddStandard(announcementId));
        }

        
        public PaginatedList<LessonPlan> GetLessonPlansTemplates(int? lpGalleryCategoryId, string title, int? classId, AttachmentSortTypeEnum sortType, int start, int count, AnnouncementState? state = AnnouncementState.Created)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureStudyCenterEnabled(Context);

            var lessonPlans = DoRead(u => CreateLessonPlanDataAccess(u).GetLessonPlanTemplates(lpGalleryCategoryId, title, classId, state, Context.PersonId.Value));

            switch (sortType)
            {
                case AttachmentSortTypeEnum.NewestUploaded:
                        lessonPlans = lessonPlans.OrderByDescending(x => x.Created).ToList();
                    break;

                case AttachmentSortTypeEnum.RecentlySent:
                    lessonPlans = lessonPlans.OrderByDescending(x => x.Created).ToList();
                    break;

                case AttachmentSortTypeEnum.OldestUploaded:
                    lessonPlans = lessonPlans.OrderBy(x => x.Created).ToList();
                    break;
            }

            var totalCount = lessonPlans.Count;
            var res = lessonPlans.Skip(start).Take(count).ToList();
            return new PaginatedList<LessonPlan>(res, start / count, count, totalCount);
        }
        
        public IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? studentId, int? teacherId, bool filterByStartDate = true)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLessonPlans(fromDate, toDate, classId, null, Context.PersonId.Value, studentId, teacherId, filterByStartDate));
        }

        public IList<LessonPlan> GetLessonPlansbyFilter(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLessonPlansByFilter(filter, Context.PersonId.Value));
        }

        public IList<AnnouncementComplex> GetLessonPlansForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool? ownedOnly = null)
        {
            return GetLessonPlansSortedByDate(fromDate, toDate, true, true, classId, complete, start, count, ownedOnly: ownedOnly);
        }

        public IList<AnnouncementComplex> GetLessonPlansSortedByDate(DateTime? fromDate, DateTime? toDate, bool includeFromDate, bool includeToDate,
            int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u, ownedOnly).GetLessonPlansOrderedByDate(new LessonPlansQuery
            {
                ClassId = classId,
                Complete = complete,
                Start = start,
                Count = count,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = includeFromDate,
                IncludeTo = includeToDate,
                Sort = sortDesc
            })).Announcements;
        }

        public IList<AnnouncementComplex> GetLessonPlansSortedByTitle(DateTime? fromDate, DateTime? toDate, string fromTitle, string toTitle,
            bool includeFromTitle, bool includeToTitle, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u, ownedOnly).GetLesonPlansOrderedByTitle(new LessonPlansQuery
            {
                ClassId = classId,
                Complete = complete,
                Start = start,
                Count = count,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = includeFromTitle,
                IncludeTo = includeToTitle,
                Sort = sortDesc
            })).Announcements;
        }

        public IList<AnnouncementComplex> GetLessonPlansSortedByClassName(DateTime? fromDate, DateTime? toDate, string fromClassName, string toClassName,
            bool includeFromClassName, bool includeToClassName, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u, ownedOnly).GetLesonPlansOrderedByClassName(new LessonPlansQuery
            {
                ClassId = classId,
                Complete = complete,
                Start = start,
                Count = count,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = includeFromClassName,
                IncludeTo = includeToClassName,
                FromClassName = fromClassName,
                ToClassName = toClassName,
                Sort = sortDesc
            })).Announcements;
        }
        
        public LessonPlan GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        public bool ExistsInGallery(string title, int? exceludedLessonPlanId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).ExistsInGallery(title, exceludedLessonPlanId));
        }

        protected override void SetComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId, bool filterByExpiryDate, bool complete)
        {
            DoUpdate( u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(null, AnnouncementTypeEnum.LessonPlan,
               schoolYearId, personId, roleId, complete, classId, startDate, endDate, filterByExpiryDate));
        }
        public void ReplaceLessonPlanInGallery(int oldLessonPlanId, int newLessonPlanId)
        {
            BaseSecurity.EnsureStudyCenterEnabled(Context); // only study center customers can use lesson plan gallery 

            var newLessonPlan = GetLessonPlanById(newLessonPlanId);
            DoUpdate(u =>
            {
                var da = CreateLessonPlanDataAccess(u);
                var oldLessonPlan = da.GetLessonPlanTemplate(oldLessonPlanId, Context.PersonId.Value);

                if (!oldLessonPlan.LpGalleryCategoryRef.HasValue)
                    throw new ChalkableException($@"'{oldLessonPlan.Title}' was deleted from Gallery.");

                if (!BaseSecurity.IsDistrictAdmin(Context) && oldLessonPlan.GalleryOwnerRef != Context.PersonId)
                    throw new ChalkableException("Current user has no access to replace lesson plan in gallery!");

                newLessonPlan.LpGalleryCategoryRef = oldLessonPlan.LpGalleryCategoryRef;
                oldLessonPlan.LpGalleryCategoryRef = null;
                CreateLessonPlanDataAccess(u).Update(new[] {oldLessonPlan, newLessonPlan});
            });
        }

        public void RemoveFromGallery(int lessonPlanId)
        {
            BaseSecurity.EnsureStudyCenterEnabled(Context); // only study center custumers can use lesson plan gallery 

            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateLessonPlanDataAccess(uow);
                var lessonPlan = da.GetAnnouncement(lessonPlanId, Context.PersonId.Value);
                if (!BaseSecurity.IsDistrictAdmin(Context) && lessonPlan.GalleryOwnerRef != Context.PersonId)
                    throw new ChalkableException("Current user has no access to remove lesson plan from gallery!");
                da.Delete(lessonPlanId);
                uow.Commit();
            }
        }

        public void CopyToGallery(int fromAnnouncementId, int toAnnouncementId)
        {
            

            //AnnouncementAttachmentService.CopyAttachments(fromAnnouncementId, new List<int>(), lpGalleryId);
            //var annApp = ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(new List<int> { fromAnnouncementId }, true);
            //ApplicationSchoolService.CopyAnnApplications(lpGalleryId, annApp);
            //StandardService.CopyStandardsToAnnouncement(fromAnnouncementId, lpGalleryId, (int)AnnouncementTypeEnum.LessonPlan);
            
            Trace.Assert(Context.SchoolYearId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureTeacher(Context);

            //get announcementApplications for copying
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(fromAnnouncementId, true);
            var appIds = annApps.Select(aa => aa.ApplicationRef).ToList();
            //get only simple apps
            var apps = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(appIds).Where(a => !a.IsAdvanced).ToList();
            annApps = annApps.Where(aa => apps.Any(a => a.Id == aa.ApplicationRef)).ToList();

            using (var u = Update())
            {
                AnnouncementAttachmentService.CopyAnnouncementAttachments(fromAnnouncementId, new List<int>(), new List<int> { toAnnouncementId }, u, ServiceLocator, ConnectorLocator);
                AnnouncementAssignedAttributeService.CopyNonStiAttributes(fromAnnouncementId, new List<int> { toAnnouncementId }, u, ServiceLocator, ConnectorLocator);
                ApplicationSchoolService.CopyAnnApplications(annApps, new List<int> { toAnnouncementId }, u);
                ServiceLocator.StandardService.CopyStandardsToAnnouncement(fromAnnouncementId, toAnnouncementId, (int)AnnouncementTypeEnum.LessonPlan);
                u.Commit();
            }
        }

        public override void AdjustDates(IList<int> ids, DateTime startDate, int classId)
        {
            BaseSecurity.EnsureTeacher(Context);
            if (startDate < Context.SchoolYearStartDate || startDate > Context.SchoolYearEndDate)
                throw new ChalkableException("Start date should be between school year start and end date");

            DoUpdate(u => CreateLessonPlanDataAccess(u).AdjustDates(ids, startDate, classId));
        }
    }
}
