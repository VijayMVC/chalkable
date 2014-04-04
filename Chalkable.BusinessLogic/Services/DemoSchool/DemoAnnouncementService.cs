using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementService : DemoSisConnectedService, IAnnouncementService
    {
        public DemoAnnouncementService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        private IDemoAnnouncementStorage CreateAnnouncementStorage()
        {
            if (BaseSecurity.IsAdminViewer(Context))
                return new DemoAnnouncementForAdminStorage(Storage);
            if(Context.Role == CoreRoles.TEACHER_ROLE)
                return new DemoAnnouncementForTeacherStorage(Storage);
            if(Context.Role == CoreRoles.STUDENT_ROLE)
                return new DemoAnnouncementForStudentStorage(Storage);
            throw new ChalkableException("Unsupported role for announcements");
        }


        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            query.RoleId = Context.Role.Id;
            query.PersonId = Context.UserLocalId;
            query.Now = Context.NowSchoolTime.Date;

            var res = new AnnouncementQueryResult();
            res.Announcements = new List<AnnouncementComplex>();
            res.Query = query;
            return res;
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
                    FromDate = fromDate,
                    ToDate = toDate,
                    GradeLevelIds = gradeLevelsIds,
                    ClassId = classId
                };
            return GetAnnouncementsComplex(q);
        }

        public IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null)
        {
            throw new NotImplementedException();
            /*
            if (activities == null)
                activities = GetActivities(query.ClassId, query.FromDate, query.ToDate, query.Start, query.Count);
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
            if (Context.Role == CoreRoles.TEACHER_ROLE || Context.Role == CoreRoles.STUDENT_ROLE)
            {
                query.SisActivitiesIds = activities.Select(x => x.Id).ToList();
                query.OwnedOnly = false;
                query.GradedOnly = false;
                query.StarredOnly = false;
                query.Start = 0;
                query.Count = int.MaxValue;
            }
            var anns = GetAnnouncements(query).Announcements;
            if (anns.Count < activities.Count && (Context.Role == CoreRoles.TEACHER_ROLE || Context.Role == CoreRoles.STUDENT_ROLE))
            {
                var noInDbActivities = activities.Where(x => anns.All(y => y.SisActivityId != x.Id)).ToList();
                AddActivitiesToChalkable(noInDbActivities);
                anns = GetAnnouncements(query).Announcements;
            }
            return MapActivitiesToAnnouncements(anns, activities);
             */
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
            var storage = CreateAnnouncementStorage();
            var nowLocalDate = Context.NowSchoolTime;
            var res = storage.Create(classAnnouncementTypeId, classId, nowLocalDate, Context.UserLocalId ?? 0);
            return res;
        }

        public AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {

            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            var da = CreateAnnouncementStorage();
            var res = da.GetDetails(announcementId, Context.UserLocalId.Value, Context.Role.Id);


            if (res.ClassRef.HasValue && res.SisActivityId.HasValue)
            {
                /*
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
                }*/
            }
            return res;
                
        }

        public void DeleteAnnouncement (int announcementId)
        {
            var storage = CreateAnnouncementStorage();
            var announcement = storage.GetById(announcementId);
            if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                throw new ChalkableSecurityException();
            storage.Delete(announcementId, null, null, null, null);
                
        }

        public void DeleteAnnouncements(int classId, int announcementType, AnnouncementState state)
        {
            var storage = CreateAnnouncementStorage();
            storage.Delete(null, Context.UserLocalId, classId, announcementType, state);
        }

        public void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            if (Context.UserLocalId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var storage = CreateAnnouncementStorage();
            storage.Delete(null, Context.UserLocalId, null, null, state);
        }

        public Announcement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            var storage = CreateAnnouncementStorage();
            var ann = storage.GetById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            Storage.StudentAnnouncementStorage.Update(announcementId, drop);
            ann.Dropped = drop;
            storage.Update(ann);
            return ann;
        }

        public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
        {
            throw new NotImplementedException();
        }
      
        public AnnouncementDetails EditAnnouncement(AnnouncementInfo announcement, int? classId = null, IList<RecipientInfo> recipients = null)
        {
            
            var storage = CreateAnnouncementStorage();
            var ann = storage.GetAnnouncement(announcement.AnnouncementId, Context.RoleId, Context.UserLocalId.Value);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            ann.Content = announcement.Content;
            ann.Subject = announcement.Subject;
            if (Context.Role == CoreRoles.TEACHER_ROLE && announcement.ClassAnnouncementTypeId.HasValue)
            {
                ann.ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeId.Value;
                ann.MaxScore = announcement.MaxScore;
                ann.WeightAddition = announcement.WeightAddition;
                ann.WeightMultiplier = announcement.WeightMultiplier;
                ann.MayBeDropped = announcement.CanDropStudentScore;
                ann.VisibleForStudent = !announcement.HideFromStudents;
            }
            if (BaseSecurity.IsAdminViewer(Context))
                ann.ClassAnnouncementTypeRef = null;

            if (announcement.ExpiresDate.HasValue)
                ann.Expires = announcement.ExpiresDate.Value;

            ann = SetClassToAnnouncement(ann, classId, ann.Expires);
            ann = PrepareReminderData(storage, ann); //todo : remove this later 
            ann = ReCreateRecipients(storage, ann, recipients);
            storage.Update(ann);

            return storage.GetDetails(announcement.AnnouncementId, Context.UserLocalId.Value, Context.RoleId);
        }

        private Announcement Submit(IDemoAnnouncementStorage storage, int announcementId, int? classId)
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

                /*
                if (classId.HasValue)
                {
                    var activity = new Activity();
                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
                    activity = ConnectorLocator.ActivityConnector.CreateActivity(classId.Value, activity);
                    res.SisActivityId = activity.Id;
                }*/
            }
            res = (AnnouncementDetails)PrepareReminderData(storage, res);
            res.GradingStyle = GradingStyleEnum.Numeric100;
            //TODO : add gradingStyle to ClassAnnouncementtype
            //if (res.ClassAnnouncementTypeRef.HasValue)
            //{
            //    var classAnnType = new ClassAnnouncementTypeDataAccess(unitOfWork).GetById(res.ClassAnnouncementTypeRef.Value);
            //    classAnnType. 
            //}
           
            storage.Update(res);
            return res;
        }

        public void SubmitAnnouncement(int announcementId, int recipientId)
        {
            throw new NotImplementedException();
            /*
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var res = Submit(da, uow, announcementId, recipientId);

                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetByDate(res.Expires);
                if(res.ClassAnnouncementTypeRef.HasValue)
                    da.ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, res.PersonRef, recipientId);
                uow.Commit();
            }*/
        }
        
        public void SubmitForAdmin(int announcementId)
        {
            var da = CreateAnnouncementStorage();
            Submit(da, announcementId, null);
        }
      
        private Announcement PrepareReminderData(IDemoAnnouncementStorage storage, Announcement announcement)
        {
            throw new NotImplementedException();
            var dateNow = Context.NowSchoolTime;
            var expires = announcement.Expires;

            if (expires.Date >= Context.NowSchoolTime.Date)
            {
                var annReminders = Storage.AnnouncementReminderStorage.GetList(announcement.Id, Context.UserLocalId ?? 0);
                foreach (var reminder in annReminders)
                {
                    reminder.RemindDate = reminder.Before.HasValue ? expires.AddDays(-reminder.Before.Value) : dateNow.Date;
                }
                Storage.AnnouncementReminderStorage.Update(annReminders);
            }
            else Storage.AnnouncementReminderStorage.DeleteByAnnouncementId(announcement.Id);
            return announcement;
        }
        private Announcement SetClassToAnnouncement(Announcement announcement, int? classId, DateTime expiresDate)
        {
            throw new NotImplementedException();
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
        private Announcement ReCreateRecipients(IDemoAnnouncementStorage storage, Announcement announcement, IList<RecipientInfo> recipientInfos)
        {
            throw new NotImplementedException();
            if (recipientInfos != null && BaseSecurity.IsAdminViewer(Context))
            {

                Storage.AnnouncementRecipientStorage.DeleteByAnnouncementId(announcement.Id);
                var annRecipients = new List<AnnouncementRecipient>();
                foreach (var recipientInfo in recipientInfos)
                {
                    annRecipients.Add(InternalAddAnnouncementRecipient(announcement.Id, recipientInfo));
                }
                Storage.AnnouncementRecipientStorage.Insert(annRecipients);
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
            return Storage.AnnouncementRecipientStorage.GetList(announcementId);
        }

        public void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle)
        {
            throw new NotImplementedException();
        }

        public Announcement GetAnnouncementById(int id)
        {
            var storage = CreateAnnouncementStorage();
            var res = storage.GetAnnouncement(id, Context.Role.Id, Context.UserLocalId ?? 0); // security here 
            if (res == null)
                throw new ChalkableSecurityException();
            return res;
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
            Storage.AnnouncementReminderStorage.Update(id, Context.UserLocalId ?? 0, starred, null, Context.NowSchoolTime.Date);
            return ann;
        }

        public Announcement SetVisibleForStudent(int id, bool visible)
        {
            var ann = GetAnnouncementById(id);
            if(ann.PersonRef != Context.UserLocalId)
                throw new ChalkableSecurityException();
            if (ann.ClassRef.HasValue)
            {
                ann.VisibleForStudent = visible;
                CreateAnnouncementStorage().Update(ann);
            }
            return ann;
        }

        public Announcement GetLastDraft()
        {
            return CreateAnnouncementStorage().GetLastDraft(Context.UserLocalId ?? 0);
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (ann.State == AnnouncementState.Draft)
                throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            return CreateAnnouncementStorage().GetAnnouncementRecipientPersons(announcementId, Context.UserLocalId ?? 0);
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType)
        {
            return CreateAnnouncementStorage().GetLastFieldValues(personId, classId, classAnnouncementType, 10);
        }

        public bool CanAddStandard(int announcementId)
        {
            throw new NotImplementedException();
        }

        public Announcement EditTitle(int announcementId, string title)
        {
            return EditTitle(GetAnnouncementById(announcementId), title, (da, t) => da.Exists(t));
        }

        private Announcement EditTitle(Announcement announcement, string title, Func<IDemoAnnouncementStorage, string, bool> existsTitleAction)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            if (announcement == null || announcement.PersonRef != Context.UserLocalId)
                throw new ChalkableSecurityException();
            if (announcement.Title != title)
            {
                var storage = CreateAnnouncementStorage();
                if (string.IsNullOrEmpty(title))
                    throw new ChalkableException("Title parameter is empty");
                if (existsTitleAction(storage, title))
                    throw new ChalkableException("The item with current title already exists");
                announcement.Title = title;
                storage.Update(announcement);
            }
            return announcement;
        }


        public bool Exists(string title)
        {
            return CreateAnnouncementStorage().Exists(title);
            
        }


        public Standard AddAnnouncementStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann,Context))
                throw new ChalkableSecurityException();

            Storage.AnnouncementStandardStorage.Add(new AnnouncementStandard
            {
                AnnouncementRef = announcementId,
                StandardRef = standardId
            });

            return Storage.StandardStorage.GetById(standardId);
        }
        public Standard RemoveStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();


            Storage.AnnouncementStandardStorage.Delete(announcementId, standardId);
            return Storage.StandardStorage.GetById(standardId);
        }

        public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
        {
            //return Storage.AnnouncementStandardStorage.
            throw new NotImplementedException();
        }
    }
}
