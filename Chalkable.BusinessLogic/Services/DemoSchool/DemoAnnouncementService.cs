using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementService : DemoSchoolServiceBase, IAnnouncementService
    {
        public DemoAnnouncementService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            query.RoleId = Context.Role.Id;
            query.PersonId = Context.UserLocalId;
            query.Now = Context.NowSchoolTime.Date;

            return CreateAnnouncementStorage(Context, Storage).GetAnnouncements(query);
        }

        public IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly)
        {
            return GetAnnouncements(new AnnouncementsQuery {Count = count, GradedOnly = gradedOnly}).Announcements;
        }

        public IList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false)
        {
            return GetAnnouncements(false, start, count, null, null, onlyOwners);
        }
        public IList<AnnouncementComplex> GetAnnouncements(bool? complete, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false)
        {
            var q = new AnnouncementsQuery
            {
                Complete = complete,
                Start = start,
                Count = count,
                ClassId = classId,
                MarkingPeriodId = markingPeriodId,
                OwnedOnly = ownerOnly,
            };
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
            var anns = GetAnnouncements(query).Announcements;
            return anns;
        }

        private IDemoAnnouncementStorage CreateAnnouncementStorage(UserContext context, DemoStorage storage)
        {
            return DemoStorage.CreateAnnouncementStorage(context, storage);
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
                                         (x.Subject != null && x.Subject.ToLower().Contains(word))
                                         || (x.ClassName.ToLower().Contains(word))
                                         || ("all".Contains(word))
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

        public AnnouncementDetails CreateAnnouncement(int? classAnnouncementTypeId, int classId)
        {
            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
                throw new ChalkableSecurityException();
            var nowLocalDate = Context.NowSchoolTime;
            var res = CreateAnnouncementStorage(Context, Storage).Create(classAnnouncementTypeId, classId, nowLocalDate, Context.UserLocalId ?? 0);
            return res;
        }

        public AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            return CreateAnnouncementStorage(Context, Storage).GetDetails(announcementId, Context.UserLocalId.Value, Context.Role.Id); ;
                
        }

        public void DeleteAnnouncement (int announcementId)
        {
            var announcement = CreateAnnouncementStorage(Context, Storage).GetById(announcementId);
            if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                throw new ChalkableSecurityException();
            CreateAnnouncementStorage(Context, Storage).Delete(announcementId, null, null, null, null);
                
        }

        public void DeleteAnnouncements(int classId, int? announcementType, AnnouncementState state)
        {
            CreateAnnouncementStorage(Context, Storage).Delete(null, Context.UserLocalId, classId, announcementType, state);
        }

        public void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            if (Context.UserLocalId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            CreateAnnouncementStorage(Context, Storage).Delete(null, Context.UserLocalId, null, null, state);
        }

        public Announcement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            var ann = CreateAnnouncementStorage(Context, Storage).GetById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            Storage.StudentAnnouncementStorage.Update(announcementId, drop);
            ann.Dropped = drop;
            CreateAnnouncementStorage(Context, Storage).Update(ann);
            return ann;
        }

        public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
        {
            throw new NotImplementedException();
        }
      
        public AnnouncementDetails EditAnnouncement(AnnouncementInfo announcement, int? classId = null)
        {

            var ann = CreateAnnouncementStorage(Context, Storage).GetAnnouncement(announcement.AnnouncementId, Context.RoleId, Context.UserLocalId.Value);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            ann.Content = announcement.Content;
            ann.Subject = announcement.Subject;
            if (Context.Role == CoreRoles.TEACHER_ROLE && announcement.ClassAnnouncementTypeId.HasValue)
            {
                ann.ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeId.Value;
                ann.IsScored = announcement.MaxScore > 0;
                ann.MaxScore = announcement.MaxScore;
                ann.WeightAddition = announcement.WeightAddition;
                ann.WeightMultiplier = announcement.WeightMultiplier;
                ann.MayBeDropped = announcement.CanDropStudentScore;
                ann.VisibleForStudent = !announcement.HideFromStudents;
            }
            if (BaseSecurity.IsAdminViewer(Context))
                throw new NotImplementedException();

            if (announcement.ExpiresDate.HasValue)
                ann.Expires = announcement.ExpiresDate.Value;

            ann = SetClassToAnnouncement(ann, classId, ann.Expires);
            //ann = ReCreateRecipients(CreateAnnouncementStorage(Context, Storage), ann);
            CreateAnnouncementStorage(Context, Storage).Update(ann);

            return CreateAnnouncementStorage(Context, Storage).GetDetails(announcement.AnnouncementId, Context.UserLocalId.Value, Context.RoleId);
        }

        private Announcement Submit(IDemoAnnouncementStorage storage, int announcementId, int? classId)
        {
            var res = GetAnnouncementDetails(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                throw new ChalkableSecurityException();
            SetClassToAnnouncement(res, classId, res.Expires);
            return storage.SubmitAnnouncement(classId, res);
        }

        public void SubmitAnnouncement(int announcementId, int recipientId)
        {
            var res = Submit(CreateAnnouncementStorage(Context, Storage), announcementId, recipientId);

            var sy = Storage.SchoolYearStorage.GetByDate(res.Expires);
            if(res.ClassAnnouncementTypeRef.HasValue)
                CreateAnnouncementStorage(Context, Storage).ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, recipientId);
        }
        
        public void SubmitForAdmin(int announcementId)
        {
            Submit(CreateAnnouncementStorage(Context, Storage), announcementId, null);
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
                announcement.ClassRef = classId.Value;
            }
            return announcement;
        }
        private Announcement ReCreateRecipients(IDemoAnnouncementStorage storage, Announcement announcement, IList<RecipientInfo> recipientInfos)
        {
            if (recipientInfos != null && BaseSecurity.IsAdminViewer(Context))
            {

                Storage.AnnouncementRecipientStorage.DeleteByAnnouncementId(announcement.Id);
                var annRecipients = new List<AnnouncementRecipient>();
                foreach (var recipientInfo in recipientInfos)
                {
                    annRecipients.Add(InternalAddAnnouncementRecipient(announcement.Id, recipientInfo));
                }
                Storage.AnnouncementRecipientStorage.Add(annRecipients);
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
            var res = CreateAnnouncementStorage(Context, Storage).GetAnnouncement(id, Context.Role.Id, Context.UserLocalId ?? 0); // security here 
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

        public void SetComplete(int announcementId, bool complete)
        {
            if (!Context.UserLocalId.HasValue)
                throw new Exception("User local id doesn't have a valid value");
            CreateAnnouncementStorage(Context, Storage).SetComplete(announcementId, Context.UserLocalId.Value, complete);
        }

        public Announcement SetVisibleForStudent(int id, bool visible)
        {
            var ann = GetAnnouncementById(id);
            //if(ann.PersonRef != Context.UserLocalId)
            //    throw new ChalkableSecurityException();
            ann.VisibleForStudent = visible;
            CreateAnnouncementStorage(Context, Storage).Update(ann);
            return ann;
        }

        public Announcement GetLastDraft()
        {
            return CreateAnnouncementStorage(Context, Storage).GetLastDraft(Context.UserLocalId ?? 0);
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (ann.State == AnnouncementState.Draft)
                throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            return CreateAnnouncementStorage(Context, Storage).GetAnnouncementRecipientPersons(announcementId, Context.UserLocalId ?? 0);
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType)
        {
            return CreateAnnouncementStorage(Context, Storage).GetLastFieldValues(personId, classId, classAnnouncementType, 10);
        }

        public bool CanAddStandard(int announcementId)
        {
            return CreateAnnouncementStorage(Context, Storage).CanAddStandard(announcementId);
        }

        public Announcement EditTitle(int announcementId, string title)
        {
            return EditTitle(GetAnnouncementById(announcementId), title, (da, t) => da.Exists(t));
        }

        private Announcement EditTitle(Announcement announcement, string title, Func<IDemoAnnouncementStorage, string, bool> existsTitleAction)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            if (announcement != null)
            {
                if (announcement.Title != title)
                {
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    var c = Storage.ClassStorage.GetById(announcement.ClassRef);
                    if (c.PrimaryTeacherRef != Context.UserLocalId)
                        throw new ChalkableSecurityException();
                    if (existsTitleAction(CreateAnnouncementStorage(Context, Storage), title))
                        throw new ChalkableException("The item with current title already exists");
                    announcement.Title = title;
                    CreateAnnouncementStorage(Context, Storage).Update(announcement);
                }    
            }
            return announcement;
        }


        public bool Exists(string title)
        {
            return CreateAnnouncementStorage(Context, Storage).Exists(title);
            
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
            var announcementIds = (Storage.AnnouncementStandardStorage).GetAll().Select(x => x.AnnouncementRef);


            var annStandarts = new List<AnnouncementStandard>();
            foreach (var id in announcementIds)
            {
                var announcement = CreateAnnouncementStorage(Context, Storage).GetById(id);

                if (announcement.ClassRef == classId)
                {
                    var standards = Storage.AnnouncementStandardStorage.GetAll(id);
                    annStandarts.AddRange(standards);
                }
                
            }

            return annStandarts;
        }

        public void CopyAnnouncement(int id, IList<int> classIds)
        {
            var ann = GetAnnouncementById(id);
            if (ann.State != AnnouncementState.Created)
                throw new ChalkableException("Current announcement is not submited yet");
            if (!ann.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement doesn't have activityId");
            Storage.StiActivityStorage.CopyActivity(ann.SisActivityId.Value, classIds);
        }
    }
}
