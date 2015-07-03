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
    }

    public class ClassAnnouncementService : BaseAnnouncementService, IClassAnnouncementService
    {
        public ClassAnnouncementService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        protected ClassAnnouncementDataAccess CreateDataAccess(UnitOfWork unitOfWork)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if (Context.Role == CoreRoles.TEACHER_ROLE)
                return new AnnouncementForTeacherDataAccess(unitOfWork, Context.SchoolLocalId.Value);
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return new AnnouncementForStudentDataAccess(unitOfWork, Context.SchoolLocalId.Value);
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
                var annDa = CreateDataAccess(uow);
                var res = annDa.Create(classAnnType.Id, classId, Context.NowSchoolTime, expiresDate, Context.PersonId.Value);
                uow.Commit();
                var sy = new SchoolYearDataAccess(uow).GetByDate(Context.NowSchoolYearTime, Context.SchoolLocalId.Value);
                annDa.ReorderAnnouncements(sy.Id, classAnnType.Id, res.ClassAnnouncementData.ClassRef);
                res = GetDetails(annDa, res.Id);// annDa.GetDetails(res.Id, Context.PersonId.Value, Context.RoleId);
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
                var da = CreateDataAccess(uow);
                var ann = da.GetAnnouncement(announcement.AnnouncementId, Context.PersonId.Value);
                AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
                
                ann = UpdateTeacherAnnouncement(ann, announcement, announcement.ClassId, uow, da);
                var res = MargeEditAnnResultWithStiData(da, ann);
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
                    var da = CreateDataAccess(uow);
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
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
                var res = GetAnnouncementDetails(announcementId);
                var classAnn = res.ClassAnnouncementData;
                AnnouncementSecurity.EnsureInModifyAccess(res, Context);
                if (res.IsDraft)
                {
                    res.State = AnnouncementState.Created;
                    res.Created = Context.NowSchoolTime.Date;
                    if (string.IsNullOrEmpty(res.Title) || classAnn.DefaultTitle == res.Title)
                        res.Title = classAnn.DefaultTitle;

                    var activity = new Activity();
                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
                    activity = ConnectorLocator.ActivityConnector.CreateActivity(classAnn.ClassRef, activity);
                    if (da.Exists(activity.Id))
                        throw new ChalkableException("Announcement with such activityId already exists");
                    classAnn.SisActivityId = activity.Id;
                }
                da.Update(classAnn);

                var sy = new SchoolYearDataAccess(uow).GetByDate(classAnn.Expires, Context.SchoolLocalId.Value);
                if (classAnn.ClassAnnouncementTypeRef.HasValue)
                    da.ReorderAnnouncements(sy.Id, classAnn.ClassAnnouncementTypeRef.Value, classAnn.ClassRef);
                uow.Commit();
            }
        }


        private ClassAnnouncement UpdateTeacherAnnouncement(ClassAnnouncement ann, ClassAnnouncementInfo inputAnnData, int classId
            , UnitOfWork uow, ClassAnnouncementDataAccess annDa)
        {
            ann.Content = inputAnnData.Content;
            if (inputAnnData.ExpiresDate.HasValue)
                ann.Expires = inputAnnData.ExpiresDate.Value;
            if (inputAnnData.ClassAnnouncementTypeId.HasValue)
            {
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

        private AnnouncementDetails MargeEditAnnResultWithStiData(ClassAnnouncementDataAccess annDa, ClassAnnouncement ann)
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
            else if (ann.ClassAnnouncementTypeRef.HasValue)
            {
                var classAnnType = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(ann.ClassAnnouncementTypeRef.Value);
                res.ClassAnnouncementData.ClassAnnouncementTypeName = classAnnType.Name;
                res.ClassAnnouncementData.ChalkableAnnouncementType = classAnnType.ChalkableAnnouncementTypeRef;
            }
            return res;
        }


        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
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
                    var da = CreateDataAccess(u);
                    var conds = new AndQueryCondition {{ClassAnnouncement.CLASS_REF_FIELD, classId}, {Announcement.STATE_FIELD, state}};
                    if(classAnnouncementType.HasValue)
                        conds.Add(ClassAnnouncement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType);
                    var classAnns = da.GetClassAnnouncements(conds, Context.PersonId.Value);
                    da.Delete(classAnns.Select(x => x.Id).ToList());
                });
        }

        public override void DeleteAnnouncements(int personId, AnnouncementState state = AnnouncementState.Draft)
        {
            Trace.Assert(Context.PersonId.HasValue);
            DoUpdate(u =>
                {
                    var da = CreateDataAccess(u);
                    var drafts = da.GetClassAnnouncements(new AndQueryCondition {{Announcement.STATE_FIELD, state}}, Context.PersonId.Value);
                    da.Delete(drafts.Select(x => x.Id).ToList());
                });
        }

        public bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return DoRead(u => CreateDataAccess(u).Exists(title, classId, expiresDate, excludeAnnouncementId));
        }

        public IList<string> GetLastFieldValues(int classId, int classAnnouncementType)
        {
            return DoRead(u => CreateDataAccess(u).GetLastFieldValues(classId, classAnnouncementType, int.MaxValue));
        }

        public ClassAnnouncement GetClassAnnouncemenById(int classAnnouncementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncement(classAnnouncementId, Context.PersonId.Value));
        }

        public override Announcement GetAnnouncementById(int id)
        {
            return GetClassAnnouncemenById(id);
        }

        public override AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
                var res = GetDetails(da, announcementId);
                if (res.ClassAnnouncementData.SisActivityId.HasValue)
                {
                    var activity = ConnectorLocator.ActivityConnector.GetActivity(res.ClassAnnouncementData.SisActivityId.Value);
                    if (activity == null)
                    {
                        throw new NoAnnouncementException();
                    }
                    MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(res, activity);
                    var chlkAnnType = ServiceLocator.ClassAnnouncementTypeService.GetChalkableAnnouncementTypeByAnnTypeName(res.ClassAnnouncementData.ClassAnnouncementTypeName);
                    res.ClassAnnouncementData.ChalkableAnnouncementType = chlkAnnType != null ? chlkAnnType.Id : (int?)null;
                }
                else if (res.ClassAnnouncementData.ClassAnnouncementTypeRef.HasValue)
                {
                    var classAnnType = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(res.ClassAnnouncementData.ClassAnnouncementTypeRef.Value);
                    res.ClassAnnouncementData.ClassAnnouncementTypeName = classAnnType.Name;
                    res.ClassAnnouncementData.ChalkableAnnouncementType = classAnnType.ChalkableAnnouncementTypeRef;
                }
                uow.Commit();
                return res;
            }
        }

        private AnnouncementDetails GetDetails(ClassAnnouncementDataAccess dataAccess, int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var ann = dataAccess.GetDetails(announcementId, Context.PersonId.Value, Context.Role.Id);
            if (ann == null)
                throw new NoAnnouncementException();
            var annStandards = ServiceLocator.StandardService.GetAnnouncementStandards(announcementId);
            ann.AnnouncementStandards = annStandards.Where(x => ann.AnnouncementStandards.Any(y => y.StandardRef == x.StandardRef
                                                    && y.AnnouncementRef == x.AnnouncementRef)).ToList();
            return ann;
        }

        
        public ClassAnnouncement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
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
            return DoRead(u => CreateDataAccess(u).CanAddStandard(announcementId));
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
                DoUpdate(u => CreateDataAccess(u).Update(ann));
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
            return DoRead(u => CreateDataAccess(u).GetClassAnnouncementByFilter(filter, Context.PersonId.Value));
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

        private IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetByActivitiesIds(activitiesIds, Context.PersonId.Value));
        }

        private void AddActivitiesToChalkable(IList<Activity> activities)
        {
            if (activities == null) return;
            using (var uow = Read())
            {
                var ids = activities.Select(x => x.Id).ToList();
                if (CreateDataAccess(uow).Exists(ids))
                    throw new ChalkableException(string.Format("Announcement with such activity Ids {0} already exists", ids.Select(x => x.ToString()).JoinString()));

            }
            IList<ClassAnnouncement> addToChlkAnns = new List<ClassAnnouncement>();
            foreach (var activity in activities)
            {
                var ann = new ClassAnnouncement
                {
                    Created = Context.NowSchoolTime,
                    State = AnnouncementState.Created,
                    SchoolRef = Context.SchoolLocalId.Value,
                    SisActivityId = activity.Id,
                };
                MapperFactory.GetMapper<ClassAnnouncement, Activity>().Map(ann, activity);
                addToChlkAnns.Add(ann);

            }
            if (addToChlkAnns.Count > 0)
            {
                using (var uow = Update())
                {
                    var da = CreateDataAccess(uow);
                    da.Insert(addToChlkAnns);
                    uow.Commit();
                }
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
            {
                using (var uow = Update())
                {
                    CreateDataAccess(uow).Update(needToUpdate);
                    uow.Commit();
                }
            }
            return res;
        }



        public new ClassAnnouncement GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }


        public override IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncementRecipientPersons(announcementId, Context.PersonId.Value));
        }
    }
}
