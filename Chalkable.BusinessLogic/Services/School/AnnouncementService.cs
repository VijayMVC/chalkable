using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chalkable.BusinessLogic.Mapping;
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
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementService
    {
        AnnouncementDetails CreateAnnouncement(int? classAnnouncementTypeId, int? classId = null);
        AnnouncementDetails GetAnnouncementDetails(int announcementId);
        void DeleteAnnouncement(int announcementId);
        void DeleteAnnouncements(int classId, int announcementType, AnnouncementState state);
        void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft);

        Announcement EditTitle(int announcementId, string title);
        bool Exists(string title);

        AnnouncementDetails EditAnnouncement(AnnouncementInfo announcement, int? classId = null, IList<RecipientInfo> recipients = null);
        void SubmitAnnouncement(int announcementId, int recipientId);
        void SubmitForAdmin(int announcementId);

        Announcement GetAnnouncementById(int id);
        IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly);
        IList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false);
        IList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false);
        IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, IList<int> gradeLevelsIds = null, int? classId = null);
        IList<AnnouncementComplex> GetAnnouncements(string filter);

        IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null);

        Announcement GetLastDraft();

        void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle);
        Announcement DropUnDropAnnouncement(int announcementId, bool drop);
        IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId);


        IList<AnnouncementRecipient> GetAnnouncementRecipients(int announcementId);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId); 
        int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);

        Announcement Star(int id, bool starred);
        Announcement SetVisibleForStudent(int id, bool visible);

        IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType);

        bool CanAddStandard(int announcementId);
        Standard AddAnnouncementStandard(int announcementId, int standardId);
        Standard RemoveStandard(int announcementId, int standardId);
        IList<AnnouncementStandard> GetAnnouncementStandards(int classId);

        void CopyAnnouncement(int id, IList<int> classIds);
    }

    public class AnnouncementService : SisConnectedService, IAnnouncementService
    {
        public AnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        private AnnouncementDataAccess CreateAnnoucnementDataAccess(UnitOfWork unitOfWork)
        {
            if(BaseSecurity.IsAdminViewer(Context))
                return new AnnouncementForAdminDataAccess(unitOfWork, Context.SchoolLocalId);
            if(Context.Role == CoreRoles.TEACHER_ROLE)
                return new AnnouncementForTeacherDataAccess(unitOfWork, Context.SchoolLocalId);
            if(Context.Role == CoreRoles.STUDENT_ROLE)
                return new AnnouncementForStudentDataAccess(unitOfWork, Context.SchoolLocalId);
            throw new ChalkableException("Unsupported role for announcements");
        }


        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            using (var uow = Read())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                query.RoleId = Context.Role.Id;
                query.PersonId = Context.UserLocalId;
                query.Now = Context.NowSchoolTime.Date;
                var res = da.GetAnnouncements(query);
                return res;
            }
        }

        public IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly)
        {
            return GetAnnouncements(new AnnouncementsQuery {Count = count, GradedOnly = gradedOnly}).Announcements;
        }

        public IList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false)
        {
            return GetAnnouncements(false, start, count, null, null, onlyOwners);
        }
        public IList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false)
        {
            var q = new AnnouncementsQuery
            {
                StarredOnly = starredOnly,
                Start = start,
                Count = count,
                ClassId = classId,
                MarkingPeriodId = markingPeriodId,
                OwnedOnly = ownerOnly,
            };
            //return new PaginatedList<AnnouncementComplex>(res.Announcements, start / count, count, res.SourceCount);
            return GetAnnouncementsComplex(q);
        }

        public IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, IList<int> gradeLevelsIds = null, int? classId = null)
        {
            var q = new AnnouncementsQuery
                {
                    FromDate = fromDate.Date,
                    ToDate = toDate.Date,
                    GradeLevelIds = gradeLevelsIds,
                    ClassId = classId
                };
            return GetAnnouncementsComplex(q);
        }

        public IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null)
        {
            if (Context.Role != CoreRoles.TEACHER_ROLE && Context.Role != CoreRoles.STUDENT_ROLE)
                throw new NotImplementedException();
            //TODO: Looks shity....think about this method and whole approach
            if (activities == null)
                activities = GetActivities(query.ClassId, query.FromDate, query.ToDate, query.Start, query.Count, query.StarredOnly);
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
            var q = new AnnouncementsQuery { SisActivitiesIds = activities.Select(x => x.Id).ToList() };
            var anns = GetAnnouncements(q).Announcements;
            if (anns.Count < activities.Count)
            {
                var noInDbActivities = activities.Where(x => anns.All(y => y.SisActivityId != x.Id)).ToList();
                AddActivitiesToChalkable(noInDbActivities);
                anns = GetAnnouncements(q).Announcements;
            }
            return MapActivitiesToAnnouncements(anns, activities);
        }


        private void AddActivitiesToChalkable(IEnumerable<Activity> activities)
        {
            IList<ClassDetails> classes = new List<ClassDetails>();
            using (var uow = Read())
            {
                var ids = activities.Select(x => x.Id).ToList();
                if(CreateAnnoucnementDataAccess(uow).Exists(ids))
                    throw new ChalkableException(string.Format("Announcement with such activity Ids {0} already exists", ids.Select(x => x.ToString()).JoinString() ));
                    
            }
            classes = ServiceLocator.ClassService.GetClasses(null, null, Context.UserLocalId);
            
            IList<Announcement> addToChlkAnns = new List<Announcement>();
            foreach (var activity in activities)
            {
                var ann = new Announcement
                {
                    Created = Context.NowSchoolTime,
                    State = AnnouncementState.Created,
                    SchoolRef = Context.SchoolLocalId.Value,
                    SisActivityId = activity.Id,
                    PersonRef = Context.UserLocalId.Value
                };
                if (Context.Role == CoreRoles.STUDENT_ROLE)
                    ann.PersonRef = classes.First(x => x.Id == activity.SectionId).TeacherRef.Value; 
                MapperFactory.GetMapper<Announcement, Activity>().Map(ann, activity);
                addToChlkAnns.Add(ann);
                
            }
            if (addToChlkAnns.Count > 0)
            {
                using (var uow = Update())
                {
                    if (addToChlkAnns.All(x => x.ClassRef.HasValue))
                    {
                        if (addToChlkAnns.Any(x => classes.All(y => y.Id != x.ClassRef)))
                            throw new SecurityException();
                    }
                    var da = CreateAnnoucnementDataAccess(uow); 
                    da.Insert(addToChlkAnns);
                    
                    uow.Commit();
                }
            }
        }

        private IList<Activity> GetActivities(int? classId, DateTime? fromDate, DateTime? toDate, int start, int count, bool starredOnly = false)
        {
            var schoolYear = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var end = count + start;
            start = start + 1;
            if (classId.HasValue)
                return ConnectorLocator.ActivityConnector.GetActivities(classId.Value, start, end, toDate, fromDate, starredOnly);
            if (Context.Role == CoreRoles.TEACHER_ROLE)
                return ConnectorLocator.ActivityConnector.GetTeacherActivities(schoolYear.Id, Context.UserLocalId.Value, start, end, toDate, fromDate);
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return ConnectorLocator.ActivityConnector.GetStudentAcivities(schoolYear.Id, Context.UserLocalId.Value, start, end, toDate, fromDate);
            return new List<Activity>();
        }
 
        private IList<AnnouncementComplex> MapActivitiesToAnnouncements(IList<AnnouncementComplex> anns, IEnumerable<Activity> activities)
        {
            var res = new List<AnnouncementComplex>();
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.SisActivityId == activity.Id);
                if (ann != null)
                {
                    MapperFactory.GetMapper<AnnouncementComplex, Activity>().Map(ann, activity);
                    res.Add(ann);       
                }
            }
            return res;
        }
        
        public IList<AnnouncementComplex> GetAnnouncements(string filter)
        {
            //TODO : rewrite impl for better performance
            var anns = GetAnnouncements(new AnnouncementsQuery()).Announcements;
            IList<AnnouncementComplex> res = new List<AnnouncementComplex>();
            if (!string.IsNullOrEmpty(filter))
            {
                string[] words = filter.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Count(); i++)
                {
                    string word = words[i];
                    int annOrder;
                    IList<AnnouncementComplex> curretnAnns = new List<AnnouncementComplex>();
                    if (int.TryParse(words[i], out annOrder))
                    {
                        curretnAnns = anns.Where(x => x.Order == annOrder).ToList();
                    }
                    else
                    {
                        curretnAnns = anns.Where(x =>
                                         (!x.ClassAnnouncementTypeRef.HasValue && x.Subject != null && x.Subject.ToLower().Contains(word))
                                         || (x.ClassRef.HasValue && x.ClassName.ToLower().Contains(word))
                                         || (!x.ClassRef.HasValue && "all".Contains(word))
                                         || x.ClassAnnouncementTypeName.ToLower().Contains(word)
                                         || x.Title != null && x.Title.ToLower().Contains(word)
                                         || x.Content != null && x.Content.ToLower().Contains(word)
                                         ).ToList();
                    }
                    res = res.Union(curretnAnns).ToList();
                }
            }
            return res;
        }

        public AnnouncementDetails CreateAnnouncement(int? classAnnouncementTypeId, int? classId = null)
        {
            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var annDa = CreateAnnoucnementDataAccess(uow);
                var nowLocalDate = Context.NowSchoolTime;
                var res = annDa.Create(classAnnouncementTypeId, classId, nowLocalDate, Context.UserLocalId ?? 0);
                uow.Commit();
                return res;
            }
        }

        public AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var res = da.GetDetails(announcementId, Context.UserLocalId.Value, Context.Role.Id);
                if (res.ClassRef.HasValue && res.SisActivityId.HasValue)
                {
                    var activity = ConnectorLocator.ActivityConnector.GetActivity(res.SisActivityId.Value);
                    MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(res, activity);
                    if (Context.Role == CoreRoles.TEACHER_ROLE)
                    {
                        //TODO: rewrite this later 
                        var atts = res.AnnouncementAttachments.Where(x => x.SisAttachmentId.HasValue && x.Id <= 0).ToList();
                        foreach (var annAtt in atts)
                        {
                            annAtt.PersonRef = Context.UserLocalId.Value;
                            if (string.IsNullOrEmpty(annAtt.Uuid) && ServiceLocator.CrocodocService.IsDocument(annAtt.Name))
                            {
                                var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(annAtt.SisAttachmentId.Value);
                                annAtt.Uuid = ServiceLocator.CrocodocService.UploadDocument(annAtt.Name, content).uuid;
                            }
                            new AnnouncementAttachmentDataAccess(uow).Insert(annAtt);
                        }
                    }
                }
                uow.Commit();
                return res;
            }
        }

        public void DeleteAnnouncement (int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var announcement = da.GetById(announcementId);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();

                if(announcement.ClassRef.HasValue && announcement.SisActivityId.HasValue)
                    ConnectorLocator.ActivityConnector.DeleteActivity(announcement.SisActivityId.Value);
                da.Delete(announcementId, null, null, null, null);
                uow.Commit();
            }
        }

        public void DeleteAnnouncements(int classId, int announcementType, AnnouncementState state)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                da.Delete(null, Context.UserLocalId, classId, announcementType, state);
                uow.Commit();
            }
        }

        public void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            if(Context.UserLocalId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                da.Delete(null, Context.UserLocalId, null, null, state);
                uow.Commit();
            }
        }

        public Announcement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var ann =  da.GetById(announcementId);
                if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                    throw new ChalkableSecurityException();
                
                //var stAnnDa = new StudentAnnouncementDataAccess(uow);
                //stAnnDa.Update(announcementId, drop);
                ann.Dropped = drop;
                da.Update(ann);
                uow.Commit();
                return ann;
            }
        }

        public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
        {
            throw new NotImplementedException();
        }
      
        public AnnouncementDetails EditAnnouncement(AnnouncementInfo announcement, int? classId = null, IList<RecipientInfo> recipients = null)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var ann = da.GetAnnouncement(announcement.AnnouncementId, Context.RoleId, Context.UserLocalId.Value);
                if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                    throw new ChalkableSecurityException();

                ann.Content = announcement.Content;
                ann.Subject = announcement.Subject;
                if (Context.Role == CoreRoles.TEACHER_ROLE && announcement.ClassAnnouncementTypeId.HasValue)
                {
                    ann.ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeId.Value;
                    ann.MaxScore = announcement.MaxScore;
                    ann.IsScored = announcement.MaxScore > 0;
                    ann.WeightAddition = announcement.WeightAddition;
                    ann.WeightMultiplier = announcement.WeightMultiplier;
                    ann.MayBeDropped = announcement.CanDropStudentScore;
                    ann.VisibleForStudent = !announcement.HideFromStudents;
                }
                if (BaseSecurity.IsAdminViewer(Context))
                    ann.ClassAnnouncementTypeRef = null;

                if(announcement.ExpiresDate.HasValue)
                    ann.Expires = announcement.ExpiresDate.Value;

                ann = SetClassToAnnouncement(ann, classId, ann.Expires);
                ann = PreperingReminderData(uow, ann); //todo : remove this later 
                ann = ReCreateRecipients(uow, ann, recipients);
                da.Update(ann);
                if (ann.ClassAnnouncementTypeRef.HasValue && ann.ClassRef.HasValue)
                {
                    var date = ann.Expires > DateTime.MinValue ? ann.Expires : ann.Created;
                    var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetByDate(date);
                    da.ReorderAnnouncements(sy.Id, ann.ClassAnnouncementTypeRef.Value, ann.PersonRef, ann.ClassRef.Value);

                    var stDa = new AnnouncementStandardDataAccess(uow);
                    stDa.Delete(new AndQueryCondition{{AnnouncementStandard.ANNOUNCEMENT_REF_FIELD, ann.Id}}
                               , new AndQueryCondition{{Class.ID_FIELD, ann.ClassRef}}, true);

                }
                uow.Commit();
                var res = da.GetDetails(announcement.AnnouncementId, Context.UserLocalId.Value, Context.RoleId);
                if (res.State == AnnouncementState.Created && res.ClassRef.HasValue && res.SisActivityId.HasValue)
                {
                    var activity = ConnectorLocator.ActivityConnector.GetActivity(res.SisActivityId.Value);
                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res); 
                    ConnectorLocator.ActivityConnector.UpdateActivity(res.SisActivityId.Value, activity);
                }
                return res;
            }                
            
            //TODO: rewrite this for better performens
        }

        private Announcement Submit(AnnouncementDataAccess dataAccess, UnitOfWork unitOfWork, int announcementId, int? classId)
        {

            var res = GetAnnouncementDetails(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                throw new ChalkableSecurityException();
            var dateNow = Context.NowSchoolTime.Date;
            SetClassToAnnouncement(res, classId, res.Expires);
            if (res.State == AnnouncementState.Draft)
            {
                res.State = AnnouncementState.Created;
                res.Created = dateNow;
                if (string.IsNullOrEmpty(res.Title) || res.DefaultTitle == res.Title)
                    res.Title = res.DefaultTitle;
                if (classId.HasValue)
                {
                    var activity = new Activity();
                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
                    activity = ConnectorLocator.ActivityConnector.CreateActivity(classId.Value, activity);
                    if(CreateAnnoucnementDataAccess(unitOfWork).Exists(activity.Id))
                        throw new ChalkableException("Announcement with such activityId already exists");
                    res.SisActivityId = activity.Id;
                }
            }
            res = (AnnouncementDetails)PreperingReminderData(unitOfWork, res);
            res.GradingStyle = GradingStyleEnum.Numeric100;
            //TODO : add gradingStyle to ClassAnnouncementtype
            //if (res.ClassAnnouncementTypeRef.HasValue)
            //{
            //    var classAnnType = new ClassAnnouncementTypeDataAccess(unitOfWork).GetById(res.ClassAnnouncementTypeRef.Value);
            //    classAnnType. 
            //}
           
            dataAccess.Update(res);
            return res;
        }

        public void SubmitAnnouncement(int announcementId, int recipientId)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var res = Submit(da, uow, announcementId, recipientId);

                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetByDate(res.Expires);
                if(res.ClassAnnouncementTypeRef.HasValue)
                    da.ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, res.PersonRef, recipientId);
                uow.Commit();
            }
        }
        
        public void SubmitForAdmin(int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                Submit(da, uow, announcementId, null);
                uow.Commit();
            }
        }
      
        private Announcement PreperingReminderData(UnitOfWork unitOfWork, Announcement announcement)
        {
            var dateNow = Context.NowSchoolTime;
            var expires = announcement.Expires;
            var da = new AnnouncementReminderDataAccess(unitOfWork);
            if (expires.Date >= Context.NowSchoolTime.Date)
            {
                var annReminders = da.GetList(announcement.Id, Context.UserLocalId ?? 0);
                foreach (var reminder in annReminders)
                {
                    reminder.RemindDate = reminder.Before.HasValue ? expires.AddDays(-reminder.Before.Value) : dateNow.Date;
                }
                da.Update(annReminders);
            }
            else da.DeleteByAnnouncementId(announcement.Id);
            return announcement;
        }
        private Announcement SetClassToAnnouncement(Announcement announcement, int? classId, DateTime expiresDate)
        {
            if (classId.HasValue)
            {
                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(expiresDate);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var mpc = ServiceLocator.MarkingPeriodService.GetMarkingPeriodClass(classId.Value, mp.Id);
                if (mpc == null)
                    throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_MARKING_PERIOD);
                if (announcement.State == AnnouncementState.Created && announcement.ClassRef != classId)
                      throw new ChalkableException("Class can't be changed for submmited announcement");
                announcement.ClassRef = classId;
            }
            return announcement;
        }
        private Announcement ReCreateRecipients(UnitOfWork unitOfWork, Announcement announcement, IList<RecipientInfo> recipientInfos)
        {
            if (recipientInfos != null && BaseSecurity.IsAdminViewer(Context))
            {
                var da = new AnnouncementRecipientDataAccess(unitOfWork);
                da.DeleteByAnnouncementId(announcement.Id);
                var annRecipients = new List<AnnouncementRecipient>();
                foreach (var recipientInfo in recipientInfos)
                {
                    annRecipients.Add(InternalAddAnnouncementRecipient(announcement.Id, recipientInfo));
                }
                da.Insert(annRecipients);
            }
            return announcement;
        }
        private AnnouncementRecipient InternalAddAnnouncementRecipient(int announcementId, RecipientInfo recipientInfo)
        {
            var announcementRecipient = new AnnouncementRecipient
            {
                AnnouncementRef = announcementId,
                ToAll = recipientInfo.ToAll,
                RoleRef = recipientInfo.ToAll ? null : recipientInfo.RoleId,
                GradeLevelRef = recipientInfo.ToAll ? null : recipientInfo.GradeLevelId,
                PersonRef = recipientInfo.ToAll ? null : recipientInfo.PersonId
            };
            return announcementRecipient;
        }

        //TODO: security check 
        public IList<AnnouncementRecipient> GetAnnouncementRecipients(int announcementId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementRecipientDataAccess(uow);
                return da.GetList(announcementId);
            }
        }

        public void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle)
        {
            throw new NotImplementedException();
        }

        public Announcement GetAnnouncementById(int id)
        {
            using (var uow = Read())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var res = da.GetAnnouncement(id, Context.Role.Id, Context.UserLocalId ?? 0); // security here 
                if(res == null)
                    throw new ChalkableSecurityException();
                return res;
            }
        }

        public int GetNewAnnouncementItemOrder(AnnouncementDetails announcement)
        {
            var attOrder = announcement.AnnouncementAttachments.Max(x => (int?)x.Order);
            var appOrder = announcement.AnnouncementApplications.Max(x => (int?)x.Order);
            int order = 0;
            if (attOrder.HasValue)
            {
                if (appOrder.HasValue)
                {
                    order = Math.Max(attOrder.Value, appOrder.Value) + 1;
                }
                else
                {
                    order = attOrder.Value + 1;
                }
            }
            else
            {
                if (appOrder.HasValue)
                {
                    order = appOrder.Value + 1;
                }
            }
            return order;
        }

        public Announcement Star(int id, bool starred)
        {
            var ann = GetAnnouncementById(id);
            using (var uow = Update())
            {
                new AnnouncementRecipientDataDataAccess(uow)
                    .Update(id, Context.UserLocalId ?? 0, starred, null, Context.NowSchoolTime.Date);
                uow.Commit();
                return ann;
            }
            //if(!Context.UserLocalId.HasValue)
            //    throw new UnassignedUserException();
            //var ann = GetAnnouncementById(id);
            //if(ann.State != AnnouncementState.Created)
            //    throw new ChalkableException("Not created item can't be starred");
            //if(!ann.SisActivityId.HasValue)
            //    throw new ChalkableException("there are not such item in Inow");
            //var activityStarring = new ActivityStarring
            //    {
            //        ActivityId = ann.SisActivityId.Value,
            //        PersonId = Context.UserLocalId.Value,
            //        Starred = starred
            //    };
            //var activity = ConnectorLocator.ActivityConnector.StarrActivity(activityStarring.ActivityId, activityStarring);
            //MapperFactory.GetMapper<Announcement, Activity>().Map(ann, activity);
            //return ann;
        }

        public Announcement SetVisibleForStudent(int id, bool visible)
        {
            var ann = GetAnnouncementById(id);
            if(ann.PersonRef != Context.UserLocalId)
                throw new ChalkableSecurityException();
            if (ann.ClassRef.HasValue)
            {
                //using (var uow = Update())
                //{
                //    //ann.VisibleForStudent = visible;
                //    //CreateAnnoucnementDataAccess(uow).Update(ann);
                //    //uow.Commit();
                //}
                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
                activity.DisplayInHomePortal = visible;
                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);
                ann.VisibleForStudent = visible;
            }
            return ann;
        }

        public Announcement GetLastDraft()
        {
            using (var uow = Read())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                return da.GetLastDraft(Context.UserLocalId ?? 0);
            }
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (ann.State == AnnouncementState.Draft)
                throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            using (var uow = Read())
            {
                return CreateAnnoucnementDataAccess(uow).GetAnnouncementRecipientPersons(announcementId, Context.UserLocalId ?? 0);
            }
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType)
        {
            using (var uow = Read())
            {
                return CreateAnnoucnementDataAccess(uow).GetLastFieldValues(personId, classId, classAnnouncementType, 10);
            }
        }

        public Announcement EditTitle(int announcementId, string title)
        {
            return EditTitle(GetAnnouncementById(announcementId), title, (da, t) => da.Exists(t));
        }

        private Announcement EditTitle(Announcement announcement, string title, Func<AnnouncementDataAccess, string, bool> existsTitleAction)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            if (announcement == null || announcement.PersonRef != Context.UserLocalId)
                throw new ChalkableSecurityException();
            if (announcement.Title != title)
            {
                using (var uow = Update())
                {
                    var da = CreateAnnoucnementDataAccess(uow);
                    if(string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    if (existsTitleAction(da, title))
                        throw new ChalkableException("The item with current title already exists");
                    announcement.Title = title;
                    da.Update(announcement);               
                    uow.Commit();
                }
            } 
            return announcement;
        }


        public bool Exists(string title)
        {
            using (var uow = Read())
            {
                return CreateAnnoucnementDataAccess(uow).Exists(title);
            }
        }


        public Standard AddAnnouncementStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann,Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AnnouncementStandardDataAccess(uow)
                    .Insert(new AnnouncementStandard
                        {
                            AnnouncementRef = announcementId,
                            StandardRef = standardId
                        });
                uow.Commit();
                return new StandardDataAccess(uow).GetById(standardId);
            }
        }
        public Standard RemoveStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AnnouncementStandardDataAccess(uow).Delete(announcementId, standardId);
                if (ann.State == AnnouncementState.Created && ann.SisActivityId.HasValue)
                {
                    var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
                    activity.Standards = activity.Standards.Where(x => x.Id != standardId).ToList();
                    ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);
                }
                uow.Commit();
                return new StandardDataAccess(uow).GetById(standardId);
            }
        }

        public bool CanAddStandard(int announcementId)
        {
            using (var uow = Read())
            {
               return CreateAnnoucnementDataAccess(uow).CanAddStandard(announcementId);
            }
        }

        public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
        {
            using (var uow = Read())
            {
                return new AnnouncementStandardDataAccess(uow).GetAnnouncementStandards(classId);
            }
        }
        
        public void CopyAnnouncement(int id, IList<int> classIds)
        {
            var ann = GetAnnouncementById(id);
            if(ann.State != AnnouncementState.Created)
                throw new ChalkableException("Current announcement is not submited yet");
            if(!ann.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement doesn't have activityId");
            ConnectorLocator.ActivityConnector.CopyActivity(ann.SisActivityId.Value, classIds);
        }
    }
}
