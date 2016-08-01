using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;
using ClassroomOption = Chalkable.Data.School.Model.ClassroomOption;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IClassAnnouncementService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(ClassAnnouncementType classAnnType, int classId, DateTime expiresDate);
        AnnouncementDetails Edit(ClassAnnouncementInfo announcement);
        void DeleteAnnouncements(int classId, int? announcementType, AnnouncementState state);
        bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId);
        IList<string> GetLastFieldValues(int classId, int classAnnouncementType);

        ClassAnnouncement GetClassAnnouncemenById(int classAnnouncementId);
        ClassAnnouncement DropUnDropAnnouncement(int classAnnouncementId, bool drop);
        void CopyAnnouncement(int classAnnouncementId, IList<int> classIds);
        ClassAnnouncement SetVisibleForStudent(int classAnnouncementId, bool visible);

        IList<ClassAnnouncement> GetClassAnnouncements(DateTime? fromDate, DateTime? toDate, int? classId, int? studentId, int? teacherId, bool? graded = null);
        IList<AnnouncementComplex> GetClassAnnouncementsForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, bool? graded = null, int start = 0, int count = int.MaxValue, AnnouncementSortOption? sort = null);
        IList<ClassAnnouncement> GetClassAnnouncementsByFilter(string filter);
        ClassAnnouncement GetLastDraft();
        IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds);
    }

    public class ClassAnnouncementService : BaseAnnouncementService<ClassAnnouncement>, IClassAnnouncementService
    {
        public ClassAnnouncementService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        protected override BaseAnnouncementDataAccess<ClassAnnouncement> CreateDataAccess(UnitOfWork unitOfWork)
        {
            return CreateClassAnnouncementDataAccess(unitOfWork);
        }

        protected ClassAnnouncementDataAccess CreateClassAnnouncementDataAccess(UnitOfWork unitOfWork)
        {
            return CreateClassAnnouncementDataAccess(ServiceLocator, unitOfWork);
        }

        protected static ClassAnnouncementDataAccess CreateClassAnnouncementDataAccess(IServiceLocatorSchool locator, UnitOfWork unitOfWork)
        {
            var context = locator.Context;
            Trace.Assert(context.SchoolYearId.HasValue);
            if (BaseSecurity.IsDistrictOrTeacher(locator.Context))
            {
                if (context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN) || context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN))
                    return new ClassAnnouncementForAdminDataAccess(unitOfWork, context.SchoolYearId.Value);
                if (context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM) || context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM))
                    return new ClassAnnouncementForTeacherDataAccess(unitOfWork, context.SchoolYearId.Value);
            }
            if (context.Role == CoreRoles.STUDENT_ROLE)
                return new ClassAnnouncementForStudentDataAccess(unitOfWork, context.SchoolYearId.Value);

            throw new ChalkableSecurityException("Current user has no permission to view or edit activities");
        }



        public AnnouncementDetails Create(ClassAnnouncementType classAnnType, int classId, DateTime expiresDate)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);

            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var annDa = CreateClassAnnouncementDataAccess(uow);
                var res = annDa.Create(classAnnType.Id, classId, Context.NowSchoolTime, expiresDate, Context.PersonId.Value);
                uow.Commit();
                var sy = new SchoolYearDataAccess(uow).GetByDate(Context.NowSchoolYearTime, Context.SchoolLocalId.Value);
                annDa.ReorderAnnouncements(sy.Id, classAnnType.Id, res.ClassAnnouncementData.ClassRef);
                res = InternalGetDetails(annDa, res.Id);
                var classAnnData = res.ClassAnnouncementData;
                if (classAnnData.ClassAnnouncementTypeRef.HasValue)
                {
                    classAnnData.ClassAnnouncementTypeName = classAnnType.Name;
                    classAnnData.ChalkableAnnouncementType = classAnnType.ChalkableAnnouncementTypeRef;
                }
                return res;
            }
        }

        public AnnouncementDetails Edit(ClassAnnouncementInfo announcement)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                var ann = da.GetAnnouncement(announcement.AnnouncementId, Context.PersonId.Value);
                AnnouncementSecurity.EnsureInModifyAccess(ann, Context);

                ann = UpdateTeacherAnnouncement(ann, announcement, announcement.ClassId, uow, da);
                var res = MergeEditAnnResultWithStiData(da, ann);
                uow.Commit();
                return res;
            }
        }

        public override Announcement EditTitle(int announcementId, string title)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var announcement = GetClassAnnouncemenById(announcementId);
            if (announcement.Title != title)
            {
                using (var uow = Update())
                {
                    if (!announcement.IsOwner)
                        throw new ChalkableSecurityException();
                    var da = CreateClassAnnouncementDataAccess(uow);
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    if (da.Exists(title, announcement.ClassRef, announcement.Expires, announcement.Id))
                        throw new ChalkableException("The item with current title already exists");
                    announcement.Title = title;
                    da.Update(announcement);
                    uow.Commit();
                }
            }
            return announcement;
        }

        public override void Submit(int announcementId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            var res = GetAnnouncementDetails(announcementId);
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                AnnouncementSecurity.EnsureInModifyAccess(res, Context);
                if (res.ClassAnnouncementData.IsDraft)
                {
                    ServiceLocator.AnnouncementAssignedAttributeService.ValidateAttributes(res.AnnouncementAttributes);
                    res.ClassAnnouncementData.State = AnnouncementState.Created;
                    res.ClassAnnouncementData.Created = Context.NowSchoolTime.Date;
                    if (string.IsNullOrEmpty(res.ClassAnnouncementData.Title)
                        || res.ClassAnnouncementData.DefaultTitle == res.ClassAnnouncementData.Title)
                        res.ClassAnnouncementData.Title = res.ClassAnnouncementData.DefaultTitle;

                    var activity = new Activity();
                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
                    activity = ConnectorLocator.ActivityConnector.CreateActivity(res.ClassAnnouncementData.ClassRef, activity);
                    if (da.Exists(activity.Id))
                        throw new ChalkableException("Announcement with such activityId already exists");

                    var annAttDa = new AnnouncementAssignedAttributeDataAccess(uow);
                    annAttDa.Delete(res.AnnouncementAttributes);
                    MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(res, activity);
                    var attributes = res.AnnouncementAttributes.Where(x => x.Attachment != null).ToList();
                    if (attributes.Count > 0)
                    {
                        var atts = new AttachmentDataAccess(uow).GetBySisAttachmentIds(attributes.Select(a => a.Attachment.SisAttachmentId.Value).ToList());
                        foreach (var attribute in res.AnnouncementAttributes)
                        {
                            if (attribute.Attachment == null) continue;
                            var att = atts.FirstOrDefault(x => x.SisAttachmentId == attribute.Attachment.SisAttachmentId);
                            if (att == null) continue;
                            attribute.AttachmentRef = att.Id;
                        }
                    }
                    annAttDa.Insert(res.AnnouncementAttributes);
                }
                da.Update(res.ClassAnnouncementData);

                var sy = new SchoolYearDataAccess(uow).GetByDate(res.ClassAnnouncementData.Expires, Context.SchoolLocalId.Value);
                if (res.ClassAnnouncementData.ClassAnnouncementTypeRef.HasValue)
                    da.ReorderAnnouncements(sy.Id, res.ClassAnnouncementData.ClassAnnouncementTypeRef.Value, res.ClassAnnouncementData.ClassRef);
                uow.Commit();
            }
        }


        private ClassAnnouncement UpdateTeacherAnnouncement(ClassAnnouncement ann, ClassAnnouncementInfo inputAnnData, int classId
            , UnitOfWork uow, ClassAnnouncementDataAccess annDa)
        {
            ann.Content = inputAnnData.Content;
            if (inputAnnData.ExpiresDate.HasValue)
                ann.Expires = inputAnnData.ExpiresDate.Value.Date;
            if (inputAnnData.ClassAnnouncementTypeId.HasValue)
            {
                var classAnnType = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(inputAnnData.ClassAnnouncementTypeId.Value);
                if (classAnnType.ClassRef != inputAnnData.ClassId)
                    throw new ChalkableException("Invalid Class Announcement type id");
                ann.ClassAnnouncementTypeRef = inputAnnData.ClassAnnouncementTypeId.Value;
                ann.MaxScore = inputAnnData.MaxScore;
                ann.IsScored = ann.MaxScore.HasValue && (ann.MaxScore > 0 || inputAnnData.Gradable);

                if (ann.MaxScore == 0 && !inputAnnData.Gradable)
                {
                    var classRoomOption = ServiceLocator.ClassroomOptionService.GetClassOption(classId);
                    if (classRoomOption == null || !classRoomOption.IsAveragingMethodPoints)
                    {
                        ann.IsScored = false;
                    }
                }
                
                ann.WeightAddition = inputAnnData.WeightAddition;
                ann.WeightMultiplier = inputAnnData.WeightMultiplier;
                ann.MayBeDropped = inputAnnData.CanDropStudentScore;
                ann.VisibleForStudent = !inputAnnData.HideFromStudents;
                
                if (inputAnnData.PreviewCommentsEnabled && inputAnnData.DiscussionEnabled && !ann.PreviewCommentsEnabled)
                {
                    if(ann.IsSubmitted)
                        new AnnouncementCommentDataAccess(uow).HideAll(ann.Id);
                }
                ann.DiscussionEnabled = inputAnnData.DiscussionEnabled;
                ann.PreviewCommentsEnabled = inputAnnData.PreviewCommentsEnabled;
                ann.RequireCommentsEnabled = inputAnnData.RequireCommentsEnabled;
                
                if (ann.ClassRef != classId)
                {
                    if (!ann.IsDraft)
                        throw new ChalkableException("Class can't be changed for submmited standard announcement");

                    ann.ClassRef = classId;
                    //clear old data befor swiching 
                    new AnnouncementApplicationDataAccess(uow).DeleteByAnnouncementId(ann.Id);
                    new AnnouncementStandardDataAccess(uow).DeleteNotAssignedToClass(ann.Id, classId);
                }
            }
            annDa.Update(ann);
            if (ann.ClassAnnouncementTypeRef.HasValue && Context.SchoolYearId.HasValue)
                annDa.ReorderAnnouncements(Context.SchoolYearId.Value, ann.ClassAnnouncementTypeRef.Value, ann.ClassRef);
            return ann;
        }

        private AnnouncementDetails MergeEditAnnResultWithStiData(ClassAnnouncementDataAccess annDa, ClassAnnouncement ann)
        {
            var res = InternalGetDetails(annDa, ann.Id);
            if (ann.IsSubmitted)
            {
                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
                foreach (var activityStandard in activity.Standards)
                {
                    if (res.AnnouncementStandards.All(x => x.Standard.Id != activityStandard.Id))
                        res.AnnouncementStandards.Add(new AnnouncementStandardDetails
                        {
                            AnnouncementRef = ann.Id,
                            StandardRef = activityStandard.Id,
                            Standard = new Standard
                            {
                                Id = activityStandard.Id,
                                Name = activityStandard.Name
                            }
                        });
                }
                MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);

                var studentAnnouncements = ServiceLocator.StudentAnnouncementService.GetStudentAnnouncements(ann.Id);
                res.GradingStudentsCount = studentAnnouncements.Count(x => x.IsGraded);
            }
            res.AnnouncementData = PrepareClassAnnouncementTypeData(res.ClassAnnouncementData);
            return res;
        }

        
        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();

                if (announcement.SisActivityId.HasValue)
                    ConnectorLocator.ActivityConnector.DeleteActivity(announcement.SisActivityId.Value);
                da.Delete(announcementId);
                uow.Commit();
            }
        }

        public void DeleteAnnouncements(int classId, int? classAnnouncementType, AnnouncementState state)
        {
            Trace.Assert(Context.PersonId.HasValue);
            DoUpdate(u =>
            {
                var da = CreateClassAnnouncementDataAccess(u);
                var conds = new AndQueryCondition { { ClassAnnouncement.CLASS_REF_FIELD, classId }, { Announcement.STATE_FIELD, state } };
                if (classAnnouncementType.HasValue)
                    conds.Add(ClassAnnouncement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType);
                var classAnns = da.GetAnnouncements(conds, Context.PersonId.Value);
                da.Delete(classAnns.Select(x => x.Id).ToList());
            });
        }


        public bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return DoRead(u => CreateClassAnnouncementDataAccess(u).Exists(title, classId, expiresDate, excludeAnnouncementId));
        }

        public IList<string> GetLastFieldValues(int classId, int classAnnouncementType)
        {
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetLastFieldValues(classId, classAnnouncementType, int.MaxValue));
        }

        public ClassAnnouncement GetClassAnnouncemenById(int classAnnouncementId)
        {
            return InternalGetAnnouncementById(classAnnouncementId);
        }

        public override IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete, bool ownerOnly = false)
        {
            var activities = GetActivities(classId, null, null, startDate, toDate, 0, int.MaxValue, complete);
            var anns = GetByActivitiesIds(activities.Select(x => x.Id).ToList());
            var res = DoRead(u => InternalGetDetailses(CreateClassAnnouncementDataAccess(u), anns.Select(a => a.Id).ToList(), ownerOnly));

            foreach (var activity in activities)
            {
                var ann = res.FirstOrDefault(x => x.ClassAnnouncementData.SisActivityId == activity.Id);
                bool annExists = ann != null;
                if (!annExists)
                    ann = new AnnouncementDetails
                    {
                        AnnouncementData = new ClassAnnouncement(),
                        AnnouncementApplications = new List<AnnouncementApplication>(),
                        AnnouncementAttachments = new List<AnnouncementAttachment>(),
                        AnnouncementQnAs = new List<AnnouncementQnAComplex>(),
                        AnnouncementAttributes = new List<AnnouncementAssignedAttribute>(),
                        AnnouncementStandards = new List<AnnouncementStandardDetails>(),
                        StudentAnnouncements = new List<StudentAnnouncementDetails>(),
                    };
                MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(ann, activity);
                if (!annExists)
                    res.Add(ann);
            }
            return res;
        }

        public override IList<AnnouncementComplex> GetAnnouncementsByIds(IList<int> announcementIds)
        {
            //TODO impl stored procedure GetClassAnnouncementsByIds
            IList<AnnouncementComplex> anns = DoRead(u => InternalGetDetailses(CreateClassAnnouncementDataAccess(u), announcementIds))
                                              .Cast<AnnouncementComplex>().ToList();

            var activitiesIds = anns.Select(x => x.ClassAnnouncementData.SisActivityId.Value).ToList();
            var activities = ConnectorLocator.ActivityConnector.GetActivitiesByIds(activitiesIds).ToList();
            DoUpdate(u => anns = MapActivitiesToAnnouncements(ServiceLocator, u, anns, activities));
            return anns;
        }

        public override AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                var res = InternalGetDetails(da, announcementId);
                if (res.ClassAnnouncementData.SisActivityId.HasValue)
                {
                    var activity = ConnectorLocator.ActivityConnector.GetActivity(res.ClassAnnouncementData.SisActivityId.Value);
                    if (activity == null)
                        throw new NoAnnouncementException();

                    MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(res, activity);

                    //insert missing announcementAssignedAttribute
                    if (HasMissingAttributes(res))
                    {
                        AnnouncementAssignedAttributeService.AddMissingSisAttributes(res.ClassAnnouncementData, res.AnnouncementAttributes, uow, ConnectorLocator, ServiceLocator);
                        AnnouncementAssignedAttributeService.AttachMissingAttachments(res.ClassAnnouncementData, res.AnnouncementAttributes, uow, ConnectorLocator, ServiceLocator);
                        res.AnnouncementAttributes = new AnnouncementAssignedAttributeDataAccess(uow).GetListByAnntId(announcementId);
                    }
                }
                res.AnnouncementData = PrepareClassAnnouncementTypeData(res.ClassAnnouncementData);
                uow.Commit();
                return res;
            }
        }

        protected override AnnouncementDetails InternalGetDetails(BaseAnnouncementDataAccess<ClassAnnouncement> dataAccess, int announcementId)
        {
            var onlyOwner = !Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);
            return InternalGetDetails(dataAccess, announcementId, onlyOwner);
        }

        private bool HasMissingAttributes(AnnouncementDetails ann)
        {
            return ann.AnnouncementAttributes.Any(x => x.Id <= 0 || IsMissingAttachment(x));
        }

        private bool IsMissingAttachment(AnnouncementAssignedAttribute attribute)
        {
            return attribute.Attachment != null && string.IsNullOrEmpty(attribute.Attachment.Uuid) && ServiceLocator.CrocodocService.IsDocument(attribute.Attachment.Name);
        }

        private ClassAnnouncement PrepareClassAnnouncementTypeData(ClassAnnouncement classAnnouncement)
        {
            if (classAnnouncement.ClassAnnouncementTypeRef.HasValue)
            {
                if (string.IsNullOrEmpty(classAnnouncement.ClassAnnouncementTypeName))
                {
                    var classAnnType = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(classAnnouncement.ClassAnnouncementTypeRef.Value);
                    classAnnouncement.ClassAnnouncementTypeName = classAnnType.Name;
                    classAnnouncement.ChalkableAnnouncementType = classAnnType.ChalkableAnnouncementTypeRef;
                }
                else
                {
                    var chlkAnnType = ServiceLocator.ClassAnnouncementTypeService.GetChalkableAnnouncementTypeByAnnTypeName(classAnnouncement.ClassAnnouncementTypeName);
                    classAnnouncement.ChalkableAnnouncementType = chlkAnnType?.Id;
                }
            }
            return classAnnouncement;
        }

        public ClassAnnouncement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                var ann = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!ann.IsSubmitted || ann.SisActivityId == null)
                    throw new ChalkableException("Announcement is not submitted yet!");
                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
                if (activity == null)
                    throw new NoAnnouncementException();

                AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
                ann.Dropped = drop;
                da.Update(ann);
                activity.IsDropped = drop;
                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);
                uow.Commit();
                return ann;
            }
        }

        /// <summary>
        /// Copies class announcements. First sends sis activities ids to iNow
        /// then copies data in our DB. Data in DB is default, because on feed we
        /// merge data from iNow.
        /// </summary>
        /// <returns>Copied ids. NOT NEW!</returns>
        public override IList<int> Copy(IList<int> classAnnouncementIds, int fromClassId, int toClassId, DateTime? startDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureAdminOrTeacher(Context);
            if (!ServiceLocator.ClassService.IsTeacherClasses(Context.PersonId.Value, fromClassId, toClassId))
                throw new ChalkableSecurityException("You can copy announcements only between your classes");

            if (classAnnouncementIds == null || classAnnouncementIds.Count == 0)
                return new List<int>();

            startDate = startDate ?? CalculateStartDateForCopying(toClassId);
            var annt = DoRead(u => CreateClassAnnouncementDataAccess(u).GetByIds(classAnnouncementIds));
            var sisActivitiesIdsToCopy = annt
                .Where(x => !x.IsDraft && x.SisActivityId.HasValue).Select(x => x.SisActivityId.Value).ToList();

            var sisCopyResult = ConnectorLocator.ActivityConnector.CopyActivities(new ActivityCopyOptions
            {
                ActivityIds = sisActivitiesIdsToCopy,
                StartDate = startDate,
                CopyToSectionIds = new List<int> { toClassId }
            });

            if (sisCopyResult == null || !sisCopyResult.Any(x => x.NewActivityId.HasValue))
                return new List<int>();

            
            var announcementApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(classAnnouncementIds, true);
            var applicationIds = announcementApps.Select(x => x.ApplicationRef).ToList();
            //Only simple apps
            var applications = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(applicationIds)
                .Where(x => !x.IsAdvanced).ToList();
            //Announcement apps. Only simple apps can be copied
            announcementApps = announcementApps.Where(x => applications.Any(y => y.Id == x.ApplicationRef)).ToList();

            IDictionary<int, int> fromToAnnouncementsIds;
            //IList<Pair<AnnouncementAttachment, AnnouncementAttachment>> annAttachmentsCopyResult;

            using (var u = Update())
            {
                var attachmentsOwners = new ClassTeacherDataAccess(u).GetClassTeachers(fromClassId, null).Select(x => x.PersonRef).ToList();

                var sisActivityCopyRes = sisCopyResult
                    .Where(x => x.NewActivityId.HasValue)
                    .Select(x=> new SisActivityCopyResult
                    {
                        FromActivityId = x.SourceActivityId,
                        ToActivityId = x.NewActivityId.Value,
                        ToClassId = x.CopyToSectionId
                    }).ToList();

                fromToAnnouncementsIds = CreateClassAnnouncementDataAccess(u).CopyClassAnnouncementsToClass(sisActivityCopyRes,  Context.NowSchoolTime);
                AnnouncementAttachmentService.CopyAnnouncementAttachments(fromToAnnouncementsIds, attachmentsOwners, u, ServiceLocator, ConnectorLocator);
                ApplicationSchoolService.CopyAnnApplications(announcementApps, fromToAnnouncementsIds.Select(x => x.Value).ToList(), u);

                u.Commit();
            }

            //Here we copy content
            //var attachmentsToCopy = annAttachmentsCopyResult.Transform(x => x.Attachment).ToList();
            //ServiceLocator.AttachementService.CopyContent(attachmentsToCopy);

            return fromToAnnouncementsIds.Select(x => x.Value).ToList();
        }

        public void CopyAnnouncement(int classAnnouncementId, IList<int> classIds)
        {
            Trace.Assert(Context.PersonId.HasValue);

            var ann = GetClassAnnouncemenById(classAnnouncementId);
            if (ann.IsDraft)
                throw new ChalkableException("Current announcement is not submited yet");

            if (!ann.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement doesn't have activityId");

            var inowRes = ConnectorLocator.ActivityConnector.CopyActivity(ann.SisActivityId.Value, classIds);
            if (inowRes != null && inowRes.Any(x => x.NewActivityId.HasValue))
            {
                //var activities = inowRes.Where(x => x.NewActivityId.HasValue)
                //    .Select(x => new Activity { Id = x.NewActivityId.Value, SectionId = x.CopyToSectionId })
                //    .ToList();

                //get announcementApplications for copying
                var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(classAnnouncementId, true);
                var appIds = annApps.Select(aa => aa.ApplicationRef).ToList();
                //get only simple apps
                var apps = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(appIds).Where(a => !a.IsAdvanced).ToList();
                annApps = annApps.Where(aa => apps.Any(a => a.Id == aa.ApplicationRef)).ToList();

                using (var u = Update())
                {
                    var da = CreateClassAnnouncementDataAccess(u);
                    var sisCopyResult = inowRes.Where(x => x.NewActivityId.HasValue)
                        .Select(x => new SisActivityCopyResult
                        {
                            FromActivityId = x.SourceActivityId,
                            ToActivityId = x.NewActivityId.Value,
                            ToClassId = x.CopyToSectionId
                        }).ToList();

                    var resAnnIds = da.CopyClassAnnouncementsToClass(sisCopyResult, Context.NowSchoolYearTime).Select(x=>x.Value).ToList();
                    var attOwners = new ClassTeacherDataAccess(u).GetClassTeachers(ann.ClassRef, null).Select(x => x.PersonRef).ToList();

                    AnnouncementAttachmentService.CopyAnnouncementAttachments(classAnnouncementId, attOwners, resAnnIds, u, ServiceLocator, ConnectorLocator);
                    ApplicationSchoolService.CopyAnnApplications(annApps, resAnnIds, u);

                    u.Commit();
                }
            }
        }

        protected override void SetComplete(Announcement announcement, bool complete)
        {
            var classAnn = announcement as ClassAnnouncement;
            if (classAnn != null && !classAnn.SisActivityId.HasValue)
                throw new ChalkableException("There are no such item in Inow");
            ConnectorLocator.ActivityConnector.CompleteActivity(classAnn.SisActivityId.Value, complete);
        }

        public override void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            if (CoreRoles.TEACHER_ROLE == Context.Role)
                ConnectorLocator.ActivityConnector.CompleteTeacherActivities(syId, Context.PersonId.Value, null, complete, toDate, DateTime.MinValue);
            if (CoreRoles.STUDENT_ROLE == Context.Role)
                ConnectorLocator.ActivityConnector.CompleteStudentActivities(syId, Context.PersonId.Value, null, complete, toDate, DateTime.MinValue);
        }

        protected override void AfterSubmitStandardsToAnnouncement(ClassAnnouncement classAnnouncement, IList<int> standardsIds)
        {
            //submit standards to inow 
            if (!classAnnouncement.IsSubmitted) return;
            Trace.Assert(classAnnouncement.SisActivityId.HasValue);
            var activity = ConnectorLocator.ActivityConnector.GetActivity(classAnnouncement.SisActivityId.Value);
            activity.Standards = standardsIds.Select(sId => new ActivityStandard { Id = sId }).ToList();
            ConnectorLocator.ActivityConnector.UpdateActivity(classAnnouncement.SisActivityId.Value, activity);
        }

        protected override void AfterAddingStandard(ClassAnnouncement announcement, AnnouncementStandard announcementStandard)
        {
            //insert standard to inow 
            if (!announcement.IsSubmitted) return;
            Trace.Assert(announcement.SisActivityId.HasValue);
            var activity = ConnectorLocator.ActivityConnector.GetActivity(announcement.SisActivityId.Value);
            activity.Standards = activity.Standards.Concat(new[] { new ActivityStandard { Id = announcementStandard.StandardRef } });
            ConnectorLocator.ActivityConnector.UpdateActivity(announcement.SisActivityId.Value, activity);
        }

        protected override void AfterRemovingStandard(ClassAnnouncement announcement, int standardId)
        {
            // removing standard from inow
            if (!announcement.IsSubmitted) return;
            Trace.Assert(announcement.SisActivityId.HasValue);
            var activity = ConnectorLocator.ActivityConnector.GetActivity(announcement.SisActivityId.Value);
            activity.Standards = activity.Standards.Where(x => x.Id != standardId).ToList();
            ConnectorLocator.ActivityConnector.UpdateActivity(announcement.SisActivityId.Value, activity);
        }

        public override bool CanAddStandard(int announcementId)
        {
            return DoRead(u => BaseSecurity.IsTeacher(Context) && CreateClassAnnouncementDataAccess(u).CanAddStandard(announcementId));
        }


        public ClassAnnouncement SetVisibleForStudent(int classAnnouncementId, bool visible)
        {
            var ann = GetClassAnnouncemenById(classAnnouncementId);
            if (ann.IsSubmitted)
            {
                Trace.Assert(ann.SisActivityId.HasValue);
                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
                activity.DisplayInHomePortal = visible;
                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);

                ann.VisibleForStudent = visible;
                DoUpdate(u => CreateClassAnnouncementDataAccess(u).Update(ann));
            }
            return ann;
        }


        public IList<ClassAnnouncement> GetClassAnnouncements(DateTime? fromDate, DateTime? toDate, int? classId, int? studentId, int? teacherId, bool? graded = null)
        {
            return GetAnnouncementsComplex(classId, studentId, teacherId, fromDate, toDate, null, graded)
                    .Select(x => x.ClassAnnouncementData)
                    .ToList();

        }

        public IList<ClassAnnouncement> GetClassAnnouncementsByFilter(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            //gets classAnnouncemnts by filter only from chalkabledb 
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetClassAnnouncementByFilter(filter, Context.PersonId.Value));
        }

        public IList<AnnouncementComplex> GetClassAnnouncementsForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete,
            bool? graded = null, int start = 0, int count = int.MaxValue, AnnouncementSortOption? sort = null)
        {
            return GetAnnouncementsComplex(classId, null, null, fromDate, toDate, complete, graded, start, count, sort);
        }

        private IList<AnnouncementComplex> GetAnnouncementsComplex(int? classId, int? studentId, int? teacherId, DateTime? fromDate,
            DateTime? toDate, bool? complete, bool? graded, int start = 0, int count = Int32.MaxValue, AnnouncementSortOption? sortOption = null)
        {
            var activities = GetActivities(classId, studentId, teacherId, fromDate, toDate, start, count, complete, graded, sortOption);
            var activitiesIds = activities.Select(x => x.Id).ToList();
            var anns = GetByActivitiesIds(activitiesIds);
            IList<AnnouncementComplex> res = null;
            DoUpdate(u => res = MergeAnnouncementsWithActivities(ServiceLocator, u, anns, activities));
            return res ?? new List<AnnouncementComplex>();
        }

        public static IList<AnnouncementComplex> MergeAnnouncementsWithActivities(IServiceLocatorSchool locator, UnitOfWork unitOfWork, IList<AnnouncementComplex> announcements, IList<Activity> activities)
        {
            var activitiesIds = activities.Select(x => x.Id).ToList();
            if (announcements.Count < activities.Count)
            {
                var noInDbActivities = activities.Where(x => announcements.All(y => y.ClassAnnouncementData.SisActivityId != x.Id)).ToList();
                AddActivitiesToChalkable(locator, unitOfWork, noInDbActivities);
                announcements = CreateClassAnnouncementDataAccess(locator, unitOfWork).GetByActivitiesIds(activitiesIds, locator.Context.PersonId.Value);
            }
            var res = MapActivitiesToAnnouncements(locator, unitOfWork, announcements, activities);
            return res;
        }

        public IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetByActivitiesIds(activitiesIds, Context.PersonId.Value));
        }

        private static void AddActivitiesToChalkable(IServiceLocatorSchool locator, UnitOfWork u, IList<Activity> activities)
        {
            AddActivitiesToChalkable(locator, activities, CreateClassAnnouncementDataAccess(locator, u));
        }

        private static void AddActivitiesToChalkable(IServiceLocatorSchool locator, IList<Activity> activities, ClassAnnouncementDataAccess dataAccess)
        {
            if (activities == null) return;
            EnsureInAnnouncementsExisting(activities, dataAccess);
            IList<ClassAnnouncement> addToChlkAnns = new List<ClassAnnouncement>();
            foreach (var activity in activities)
            {
                var ann = new ClassAnnouncement
                {
                    Created = locator.Context.NowSchoolTime,
                    State = AnnouncementState.Created,
                    SchoolYearRef = locator.Context.SchoolYearId.Value,
                    SisActivityId = activity.Id,
                };
                MapperFactory.GetMapper<ClassAnnouncement, Activity>().Map(ann, activity);
                addToChlkAnns.Add(ann);
            }
            if (addToChlkAnns.Count > 0)
                dataAccess.Insert(addToChlkAnns);
        }

        private static void EnsureInAnnouncementsExisting(IList<Activity> activities, ClassAnnouncementDataAccess dataAccess)
        {
            var ids = activities.Select(x => x.Id).ToList();
            if (dataAccess.Exists(ids))
                throw new ChalkableException($"Announcement with such activity Ids {ids.Select(x => x.ToString()).JoinString()} already exists");
        }

        private IList<Activity> GetActivities(int? classId, int? studentId, int? teacherId, DateTime? fromDate, DateTime? toDate, int start, int count, bool? complete = false, bool? graded = null, AnnouncementSortOption? sort = AnnouncementSortOption.DueDateAscending)
        {
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            Trace.Assert(Context.PersonId.HasValue);

            var hasViewAdminPermissions = Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);
            var end = count + start;
            start = start + 1;
            var intSort = (int?)sort;

            //For teachers who has view classroom(admin permissions)
            if (studentId.HasValue && hasViewAdminPermissions)
                return ConnectorLocator.ActivityConnector.GetStudentAcivities(Context.SchoolYearId.Value, studentId.Value,
                    intSort, start, end, toDate, fromDate, complete, graded, classId);

            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return ConnectorLocator.ActivityConnector.GetStudentAcivities(Context.SchoolYearId.Value, Context.PersonId.Value,
                    intSort, start, end, toDate, fromDate, complete, graded, classId);

            if (classId.HasValue)
                return ConnectorLocator.ActivityConnector.GetActivities(classId.Value, start, end, intSort, toDate, fromDate, complete);

            if (teacherId.HasValue && hasViewAdminPermissions)
                return ConnectorLocator.ActivityConnector.GetTeacherActivities(Context.SchoolYearId.Value, teacherId.Value,
                    intSort, start, end, toDate, fromDate, complete);

            if (Context.Role == CoreRoles.TEACHER_ROLE)
                return ConnectorLocator.ActivityConnector.GetTeacherActivities(Context.SchoolYearId.Value, Context.PersonId.Value,
                    intSort, start, end, toDate, fromDate, complete);

            return new List<Activity>();
        }

        private static IList<AnnouncementComplex> MapActivitiesToAnnouncements(IServiceLocatorSchool locator, UnitOfWork u, IList<AnnouncementComplex> anns, IEnumerable<Activity> activities)
        {
            var res = new List<AnnouncementComplex>();
            var needToUpdate = new List<ClassAnnouncement>();
            var clAnns = anns.Where(x => x.ClassAnnouncementData != null).ToList();
            foreach (var activity in activities)
            {
                var ann = clAnns.FirstOrDefault(x => x.ClassAnnouncementData.SisActivityId == activity.Id);
                if (ann != null)
                {
                    if (ann.ClassAnnouncementData.Expires != activity.Date || ann.ClassAnnouncementData.Title != activity.Name)
                        needToUpdate.Add(ann.ClassAnnouncementData);
                    MapperFactory.GetMapper<AnnouncementComplex, Activity>().Map(ann, activity);
                    var chlkAnnType = locator.ClassAnnouncementTypeService.GetChalkableAnnouncementTypeByAnnTypeName(ann.ClassAnnouncementData.ClassAnnouncementTypeName);
                    ann.ClassAnnouncementData.ChalkableAnnouncementType = chlkAnnType?.Id;
                    res.Add(ann);
                }
            }

            if (needToUpdate.Count > 0)
            {
                CreateClassAnnouncementDataAccess(locator, u).Update(needToUpdate);
            }
            return res;
        }

        public ClassAnnouncement GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        protected override void SetComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId, bool filterByExpiryDate, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            if (CoreRoles.TEACHER_ROLE == Context.Role)
                ConnectorLocator.ActivityConnector.CompleteTeacherActivities(syId, Context.PersonId.Value, classId, complete, startDate, endDate);
            if (CoreRoles.STUDENT_ROLE == Context.Role)
                ConnectorLocator.ActivityConnector.CompleteStudentActivities(syId, Context.PersonId.Value, classId, complete, startDate, endDate);
        }
    }
}
