using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using Microsoft.Data.OData.Query.SemanticAst;

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

        IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null);

        IList<ClassAnnouncement> GetClassAnnouncements(DateTime? fromDate, DateTime? toDate, int? classId, bool onlyOwners = false, bool? graded = null);
        IList<AnnouncementComplex> GetClassAnnouncementsForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, bool onlyOwners = false, bool? graded = null, int start = 0, int count = int.MaxValue);
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
            Trace.Assert(Context.SchoolYearId.HasValue);
            if (Context.Role == CoreRoles.TEACHER_ROLE)
                return new ClassAnnouncementForTeacherDataAccess(unitOfWork, Context.SchoolYearId.Value);
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return new ClassAnnouncementForStudentDataAccess(unitOfWork, Context.SchoolYearId.Value);
            throw new NotImplementedException();
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
                res = GetDetails(annDa, res.Id);
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
                    throw  new ChalkableException("Invalid Class Announcement type id");
                ann.ClassAnnouncementTypeRef = inputAnnData.ClassAnnouncementTypeId.Value;
                ann.MaxScore = inputAnnData.MaxScore;
                ann.IsScored = inputAnnData.MaxScore > 0;
                ann.WeightAddition = inputAnnData.WeightAddition;
                ann.WeightMultiplier = inputAnnData.WeightMultiplier;
                ann.MayBeDropped = inputAnnData.CanDropStudentScore;
                ann.VisibleForStudent = !inputAnnData.HideFromStudents;
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
            var res = GetDetails(annDa, ann.Id);
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
                    var conds = new AndQueryCondition {{ClassAnnouncement.CLASS_REF_FIELD, classId}, {Announcement.STATE_FIELD, state}};
                    if(classAnnouncementType.HasValue)
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
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u =>
                {
                    var res = CreateClassAnnouncementDataAccess(u).GetAnnouncement(classAnnouncementId, Context.PersonId.Value);
                    if(res == null)
                        throw new NoAnnouncementException();
                    return res;
                });
        }

        public override Announcement GetAnnouncementById(int id)
        {
            return GetClassAnnouncemenById(id);
        }

        public override IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool ownerOnly = false)
        {
            var activities = GetActivities(classId, startDate, toDate, 0, int.MaxValue, null);
            var anns = GetByActivitiesIds(activities.Select(x => x.Id).ToList());
            var res = new List<AnnouncementDetails>();
            
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.ClassAnnouncementData.SisActivityId == activity.Id);
                using (var uow = Read())
                {
                    var da = CreateClassAnnouncementDataAccess(uow);
                    var details = ann != null ? GetDetails(da, ann.Id) : new AnnouncementDetails();
                    MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(details, activity);
                    res.Add(details);
                }
            }
            return res;
        }

        public override AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                var res = GetDetails(da, announcementId);
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

        private bool HasMissingAttributes(AnnouncementDetails ann)
        {
            return ann.AnnouncementAttributes.Any(x => x.Id <= 0 || IsMissingAttachment(x));
        }

        private bool IsMissingAttachment(AnnouncementAssignedAttribute attribute)
        {
            return attribute.Attachment != null && string.IsNullOrEmpty(attribute.Attachment.Uuid) && ServiceLocator.CrocodocService.IsDocument(attribute.Attachment.Name);
        }

        private AnnouncementDetails GetDetails(ClassAnnouncementDataAccess dataAccess, int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var ann = dataAccess.GetDetails(announcementId, Context.PersonId.Value, Context.Role.Id);
            if (ann == null)
                throw new NoAnnouncementException();
            var annStandards = ServiceLocator.StandardService.GetAnnouncementStandards(announcementId);
            ann.AnnouncementStandards = annStandards.Where(x => ann.AnnouncementStandards.Any(y => y.StandardRef == x.StandardRef && y.AnnouncementRef == x.AnnouncementRef)).ToList();
            return ann;
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
                    classAnnouncement.ChalkableAnnouncementType = chlkAnnType != null ? chlkAnnType.Id : (int?)null;
                }
            }
            return classAnnouncement;
        }

        public ClassAnnouncement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            using (var uow = Update())
            {
                var da = CreateClassAnnouncementDataAccess(uow);
                var ann = da.GetById(announcementId);
                AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
                ann.Dropped = drop;
                da.Update(ann);
                uow.Commit();
                return ann;
            }
        }

        public void CopyAnnouncement(int classAnnouncementId, IList<int> classIds)
        {
            var ann = GetClassAnnouncemenById(classAnnouncementId);
            if (ann.IsDraft)
                throw new ChalkableException("Current announcement is not submited yet");
            if (!ann.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement doesn't have activityId");
            ConnectorLocator.ActivityConnector.CopyActivity(ann.SisActivityId.Value, classIds);
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
                ConnectorLocator.ActivityConnector.CompleteTeacherActivities(syId, Context.PersonId.Value, complete, toDate);
            if (CoreRoles.STUDENT_ROLE == Context.Role)
                ConnectorLocator.ActivityConnector.CompleteStudentActivities(syId, Context.PersonId.Value, complete, toDate);
        }


        protected override void AfterAddingStandard(Announcement announcement, AnnouncementStandard announcementStandard)
        {
            //insert standard to inow 
            var classAnn = GetClassAnnouncemenById(announcement.Id);
            if (!classAnn.IsSubmitted) return;
            var activity = ConnectorLocator.ActivityConnector.GetActivity(classAnn.SisActivityId.Value);
            activity.Standards = activity.Standards.Concat(new[] { new ActivityStandard { Id = announcementStandard.StandardRef } });
            ConnectorLocator.ActivityConnector.UpdateActivity(classAnn.SisActivityId.Value, activity);
        }

        protected override void AfterRemovingStandard(Announcement announcement, int standardId)
        {
            // removing standard from inow
            var classAnn = GetClassAnnouncemenById(announcement.Id);
            if (!classAnn.IsSubmitted) return;
            var activity = ConnectorLocator.ActivityConnector.GetActivity(classAnn.SisActivityId.Value);
            activity.Standards = activity.Standards.Where(x => x.Id != standardId).ToList();
            ConnectorLocator.ActivityConnector.UpdateActivity(classAnn.SisActivityId.Value, activity);
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
                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
                activity.DisplayInHomePortal = visible;
                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);

                ann.VisibleForStudent = visible;
                DoUpdate(u => CreateClassAnnouncementDataAccess(u).Update(ann));
            }
            return ann;
        }
        

        public IList<ClassAnnouncement> GetClassAnnouncements(DateTime? fromDate, DateTime? toDate, int? classId, bool onlyOwners = false, bool? graded = null)
        {
            return GetAnnouncementsComplex(new AnnouncementsQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    ClassId = classId,
                    OwnedOnly = onlyOwners,
                    Graded = graded,
                }).Select(x => x.ClassAnnouncementData).ToList();
        }

        public IList<ClassAnnouncement> GetClassAnnouncementsByFilter(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            //gets classAnnouncemnts by filter only from chalkabledb 
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetClassAnnouncementByFilter(filter, Context.PersonId.Value));
        }

        public IList<AnnouncementComplex> GetClassAnnouncementsForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, bool onlyOwners = false, bool? graded = null, int start = 0, int count = int.MaxValue)
        {
            return GetAnnouncementsComplex(new AnnouncementsQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    ClassId = classId,
                    Complete = complete,
                    OwnedOnly = onlyOwners,
                    Graded = graded,
                    Start = start,
                    Count = count
                });
        }

        public IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null)
        {
            if (Context.Role != CoreRoles.TEACHER_ROLE && Context.Role != CoreRoles.STUDENT_ROLE)
                throw new NotImplementedException();
            //TODO: Looks shity....think about this method and whole approach
            if (activities == null)
                activities = GetActivities(query.ClassId, query.FromDate, query.ToDate, query.Start, query.Count, query.Complete, query.Graded);
            else
            {
                if (query.ClassId.HasValue)
                    activities = activities.Where(x => x.SectionId == query.ClassId).ToList();
                if (query.FromDate.HasValue)
                    activities = activities.Where(x => x.Date >= query.FromDate).ToList();
                if (query.ToDate.HasValue)
                    activities = activities.Where(x => x.Date <= query.ToDate).ToList();
                activities = activities.Skip(query.Start).Take(query.Count).ToList();
            }
            var activitiesIds = activities.Select(x => x.Id).ToList();
            var anns = GetByActivitiesIds(activitiesIds);
            if (anns.Count < activities.Count)
            {
                var noInDbActivities = activities.Where(x => anns.All(y => y.ClassAnnouncementData.SisActivityId != x.Id)).ToList();
                AddActivitiesToChalkable(noInDbActivities);
                anns = GetByActivitiesIds(activitiesIds);
            }
            var res = MapActivitiesToAnnouncements(anns, activities);
            return res;
        }

        public IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetByActivitiesIds(activitiesIds, Context.PersonId.Value));
        }

        private void AddActivitiesToChalkable(IList<Activity> activities)
        {
            if (activities == null) return;
            EnsureInAnnouncementsExisting(activities);
            IList<ClassAnnouncement> addToChlkAnns = new List<ClassAnnouncement>();
            foreach (var activity in activities)
            {
                var ann = new ClassAnnouncement
                    {
                        Created = Context.NowSchoolTime,
                        State = AnnouncementState.Created,
                        SchoolYearRef = Context.SchoolYearId.Value,
                        SisActivityId = activity.Id,
                    };
                MapperFactory.GetMapper<ClassAnnouncement, Activity>().Map(ann, activity);
                addToChlkAnns.Add(ann);
            }
            if (addToChlkAnns.Count > 0)
                DoUpdate(u => CreateClassAnnouncementDataAccess(u).Insert(addToChlkAnns));
            
        }
        
        private void EnsureInAnnouncementsExisting(IList<Activity> activities)
        {
            using (var uow = Read())
            {
                var ids = activities.Select(x => x.Id).ToList();
                if (CreateClassAnnouncementDataAccess(uow).Exists(ids))
                    throw new ChalkableException(string.Format("Announcement with such activity Ids {0} already exists", ids.Select(x => x.ToString()).JoinString()));
            }
        }

        private IList<Activity> GetActivities(int? classId, DateTime? fromDate, DateTime? toDate, int start, int count, bool? complete = false, bool? graded = null)
        {
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            Trace.Assert(Context.PersonId.HasValue);
            var end = count + start;
            start = start + 1;
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return ConnectorLocator.ActivityConnector.GetStudentAcivities(Context.SchoolYearId.Value, Context.PersonId.Value, start, end, toDate, fromDate, complete, graded, classId);
            if (classId.HasValue)
                return ConnectorLocator.ActivityConnector.GetActivities(classId.Value, start, end, toDate, fromDate, complete);
            if (Context.Role == CoreRoles.TEACHER_ROLE)
                return ConnectorLocator.ActivityConnector.GetTeacherActivities(Context.SchoolYearId.Value, Context.PersonId.Value, start, end, toDate, fromDate, complete);
            return new List<Activity>();
        }

        private IList<AnnouncementComplex> MapActivitiesToAnnouncements(IList<AnnouncementComplex> anns, IEnumerable<Activity> activities)
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
                    var chlkAnnType = ServiceLocator.ClassAnnouncementTypeService.GetChalkableAnnouncementTypeByAnnTypeName(ann.ClassAnnouncementData.ClassAnnouncementTypeName);
                    ann.ClassAnnouncementData.ChalkableAnnouncementType = chlkAnnType != null ? chlkAnnType.Id : (int?)null;
                    res.Add(ann);
                }
            }
            if (needToUpdate.Count > 0)
                DoUpdate(u => CreateDataAccess(u).Update(needToUpdate));
            return res;
        }

        public ClassAnnouncement GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            return DoRead(u => CreateClassAnnouncementDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        protected override void SetComplete(int schoolYearId, int personId, int roleId, DateTime? tillDateToUpdate, int? classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            
            SetAnnouncementsAsComplete(tillDateToUpdate, true);
        }
    }
}
