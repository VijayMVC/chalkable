using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Models;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.announcement;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementCompleteStorage : BaseDemoIntStorage<AnnouncementComplete>
    {
        public DemoAnnouncementCompleteStorage()
            : base(null, true)
        {
        }

        public void SetComplete(AnnouncementComplete complete)
        {
            if (data.Count(x => x.Value.AnnouncementId == complete.AnnouncementId && x.Value.UserId == complete.UserId) == 0)
            {
                data.Add(GetNextFreeId(), complete);
            }
            var item = data.First(x => x.Value.AnnouncementId == complete.AnnouncementId && x.Value.UserId == complete.UserId).Key;
            data[item] = complete;
        }

        public bool? GetComplete(int announcementId, int userId)
        {
            if (data.Count(x => x.Value.AnnouncementId == announcementId && x.Value.UserId == userId) == 0)
            {
                return false;
            }
            return data.First(x => x.Value.AnnouncementId == announcementId && x.Value.UserId == userId).Value.Complete;
        }
    }

    public class DemoAnnouncementRecipientStorage : BaseDemoIntStorage<AnnouncementRecipient>
    {
        public DemoAnnouncementRecipientStorage()
            : base(x => x.Id)
        {
        }


        public void DeleteByAnnouncementId(int announcementId)
        {
            var annRep = data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Key).ToList();
            Delete(annRep);
        }

        public IList<AnnouncementRecipient> GetList(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
        }
    }

    public class DemoAnnouncementService : DemoSchoolServiceBase, IAnnouncementService
    {
        private DemoAnnouncementCompleteStorage AnnouncementCompleteStorage { get; set; }
        private DemoAnnouncementStorage AnnouncementStorage { get; set; }
        private DemoAnnouncementRecipientStorage AnnouncementRecipientStorage { get; set; }
        private DemoAnnouncementStandardStorage AnnouncementStandardStorage { get; set; }
        private DemoStiActivityStorage ActivityStorage { get; set; }
        private IAnnouncementProcessor AnnouncementProcessor { get; set; }
        


        public DemoAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AnnouncementCompleteStorage = new DemoAnnouncementCompleteStorage();
            AnnouncementRecipientStorage = new DemoAnnouncementRecipientStorage();
            AnnouncementStandardStorage = new DemoAnnouncementStandardStorage();
            ActivityStorage = new DemoStiActivityStorage();
        }

        public void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state)
        {
            var announcementsToDelete = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = userId,
                Id = announcementId,
                ClassId = classId
            }).Announcements;
            if (state.HasValue)
                announcementsToDelete = announcementsToDelete.Where(x => x.State == state).ToList();
            if (announcementType.HasValue)
                announcementsToDelete = announcementsToDelete.Where(x => x.ClassAnnouncementTypeRef == announcementType).ToList();

            foreach (var announcementComplex in announcementsToDelete)
            {
                if (announcementComplex.SisActivityId.HasValue)
                {
                    var scores = ActivityScoreStorage.GetSores(announcementComplex.SisActivityId.Value);
                    StorageLocator.StiActivityScoreStorage.Delete(scores);
                    var studentAnnouncements =
                        StorageLocator.StudentAnnouncementStorage.GetAll()
                            .Where(x => x.AnnouncementId == announcementComplex.Id)
                            .ToList();
                    StorageLocator.StudentAnnouncementStorage.Delete(studentAnnouncements);

                    var qnas = StorageLocator.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
                    {
                        AnnouncementId = announcementComplex.Id
                    }).AnnouncementQnAs;

                    ServiceLocator.AnnouncementQnAService.Delete(qnas);
                    StorageLocator.StiActivityStorage.Delete(announcementComplex.SisActivityId.Value);
                }
                var announcementApps = StorageLocator.AnnouncementApplicationStorage.GetAll(announcementComplex.Id, true);
                StorageLocator.AnnouncementApplicationStorage.Delete(announcementApps);

                var attachments = StorageLocator.AnnouncementAttachmentStorage.GetAll(announcementComplex.Id);
                StorageLocator.AnnouncementAttachmentStorage.Delete(attachments);
                var standarts = StorageLocator.AnnouncementStandardStorage.GetAll(announcementComplex.Id);
                StorageLocator.AnnouncementStandardStorage.Delete(standarts);
            }

            AnnouncementStorage.Delete(announcementsToDelete.Select(x => x.Id).ToList());
        }

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            query.RoleId = Context.Role.Id;
            query.PersonId = Context.PersonId;
            query.Now = Context.NowSchoolTime.Date;
            if (Context.PersonId == null)
                throw new ChalkableException("User local id is null");
            var announcements = AnnouncementStorage.GetData().Select(x => x.Value);
            if (query.Id.HasValue)
                announcements = announcements.Where(x => x.Id == query.Id);

            if (query.ClassId.HasValue)
                announcements = announcements.Where(x => x.ClassRef == query.ClassId);

            if (query.MarkingPeriodId.HasValue)
            {
                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(query.MarkingPeriodId.Value);
                announcements = announcements.Where(x => x.Expires >= mp.StartDate && x.Expires <= mp.EndDate);
            }

            if (query.FromDate.HasValue)
                announcements = announcements.Where(x => x.Expires >= query.FromDate);

            if (query.ToDate.HasValue)
                announcements = announcements.Where(x => x.Expires <= query.ToDate);
            if (query.Complete.HasValue)
                announcements = announcements.Where(x => AnnouncementCompleteStorage.GetComplete(x.Id, Context.PersonId.Value) == query.Complete);


            foreach (var announcementComplex in announcements)
            {
                var complete = AnnouncementCompleteStorage.GetComplete(announcementComplex.Id, Context.PersonId.Value);
                var cls = ServiceLocator.ClassService.GetById(announcementComplex.ClassRef);
                announcementComplex.FullClassName = cls.Name + " " + cls.ClassNumber;
                announcementComplex.Complete = complete.HasValue && complete.Value;
            }
            if (query.OwnedOnly)
                announcements = announcements.Where(x => x.PrimaryTeacherRef == query.PersonId);

            if (query.SisActivitiesIds != null)
                announcements = announcements.Where(x => query.SisActivitiesIds.Contains(x.Id));


            announcements = AnnouncementProcessor.GetAnnouncements(announcements, query);

            if (query.Start > 0)
                announcements = announcements.Skip(query.Start);
            if (query.Count > 0)
                announcements = announcements.Take(query.Count);

            return new AnnouncementQueryResult
            {
                Announcements = announcements.ToList(),
                Query = query,
                SourceCount = AnnouncementStorage.GetData().Count
            };
        }

        public IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly)
        {
            return GetAnnouncements(new AnnouncementsQuery {Count = count, GradedOnly = gradedOnly}).Announcements;
        }

        public IList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false)
        {
            return GetAnnouncements(false, start, count, null, null, onlyOwners);
        }
        public IList<AnnouncementComplex> GetAnnouncements(bool? complete, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false, bool? graded = null)
        {
            var q = new AnnouncementsQuery
            {
                Complete = complete,
                Start = start,
                Count = count,
                ClassId = classId,
                MarkingPeriodId = markingPeriodId,
                OwnedOnly = ownerOnly,
                Graded = graded
            };
            return GetAnnouncementsComplex(q);
        }

        public IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, int? classId = null)
        {
            var q = new AnnouncementsQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    ClassId = classId
                };
            return GetAnnouncementsComplex(q);
        }

        public IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null)
        {
            var anns = GetAnnouncements(query).Announcements;
            return anns;
        }

        public IList<AnnouncementComplex> GetAnnouncements(string filter)
        {
            //TODO : rewrite impl for better performance
            var anns = GetAnnouncements(new AnnouncementsQuery()).Announcements;
            IList<AnnouncementComplex> res = new List<AnnouncementComplex>();
            if (string.IsNullOrEmpty(filter)) 
                return res;
            var words = filter.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < words.Count(); i++)
            {
                var word = words[i];
                int annOrder;
                IList<AnnouncementComplex> currentAnns = new List<AnnouncementComplex>();
                if (int.TryParse(words[i], out annOrder))
                {
                    currentAnns = anns.Where(x => x.Order == annOrder).ToList();
                }
                else
                {
                    currentAnns = anns.Where(x =>
                        (x.Subject != null && x.Subject.ToLower().Contains(word))
                        || (x.ClassName.ToLower().Contains(word))
                        || ("all".Contains(word))
                        || x.ClassAnnouncementTypeName.ToLower().Contains(word)
                        || x.Title != null && x.Title.ToLower().Contains(word)
                        || x.Content != null && x.Content.ToLower().Contains(word)
                        ).ToList();
                }
                res = res.Union(currentAnns).ToList();
            }
            return res;
        }

        public AnnouncementDetails CreateAnnouncement(ClassAnnouncementType classAnnType, int classId, DateTime expiresDate)
        {
            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
                throw new ChalkableSecurityException();
            var draft = GetLastDraft();
            var res = AnnouncementStorage.Create(classAnnType.Id, classId, expiresDate, Context.PersonId ?? 0);

            if (draft != null)
            {
                res.Content = draft.Content;
            }
            return res;
        }

        public void DeleteAnnouncement (int announcementId)
        {
            var announcement = AnnouncementStorage.GetById(announcementId);
            if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                throw new ChalkableSecurityException();
            AnnouncementStorage.Delete(announcementId, null, null, null, null);
                
        }

        public void DeleteAnnouncements(int classId, int? announcementType, AnnouncementState state)
        {
            AnnouncementStorage.Delete(null, Context.PersonId, classId, announcementType, state);
        }

        public void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            if (Context.PersonId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            AnnouncementStorage.Delete(null, Context.PersonId, null, null, state);
        }

        public Announcement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            var ann = AnnouncementStorage.GetById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            StudentAnnouncementStorage.Update(announcementId, drop);
            ann.Dropped = drop;
            AnnouncementStorage.Update(ann);
            return ann;
        }

        public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
        {
            throw new NotImplementedException();
        }
      
        public AnnouncementDetails EditAnnouncement(AnnouncementInfo announcement, int? classId = null)
        {

            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            
            var ann = AnnouncementStorage.GetAnnouncement(announcement.AnnouncementId, Context.RoleId, Context.PersonId.Value);
          
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

                if (!ann.IsScored && ann.SisActivityId.HasValue)
                {
                    var studentIds = ServiceLocator.StudentAnnouncementService.GetStudentAnnouncements(ann.Id).Select(x => x.StudentId);

                    StorageLocator.StiActivityScoreStorage.ResetScores(ann.SisActivityId.Value, studentIds);
                }

                if (classId.HasValue && ann.ClassRef != classId.Value && ann.State == AnnouncementState.Draft)
                {
                    // clearing some data before switching between classes
                    ann.Title = null;
                    StorageLocator.AnnouncementApplicationStorage.DeleteByAnnouncementId(ann.Id);
                }
            }
            if (BaseSecurity.IsAdminViewer(Context))
                throw new NotImplementedException();

            ann.Expires = announcement.ExpiresDate.HasValue ? announcement.ExpiresDate.Value : DateTime.Today.AddDays(1);
            ann = SetClassToAnnouncement(ann, classId, ann.Expires);
            AnnouncementStorage.Update(ann);

            return GetDetails(announcement.AnnouncementId, Context.PersonId.Value, Context.RoleId);
        }


        private AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, DateTime? expiresDateTime = null)
        {
            if (Context.SchoolLocalId == null) throw new Exception("Context school local id is null");

            var annId = AnnouncementStorage.GetNextFreeId();
            var person = ServiceLocator.PersonService.GetPerson(userId);

            if (!classAnnouncementTypeId.HasValue)
                classAnnouncementTypeId = StorageLocator.ClassAnnouncementTypeStorage.GetAll(classId).First().Id;

            //todo: create admin announcements if it's admin


            var cls = StorageLocator.ClassStorage.GetById(classId);
            var announcement = new AnnouncementComplex
            {
                ClassAnnouncementTypeName = StorageLocator.ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId.Value).Name,
                ChalkableAnnouncementType = classAnnouncementTypeId,
                PrimaryTeacherName = person.FullName(),
                ClassName = cls.Name,
                PrimaryTeacherGender = person.Gender,
                FullClassName = cls.Name + " " + cls.ClassNumber,
                IsScored = false,
                Id = annId,
                PrimaryTeacherRef = userId,
                IsOwner = Context.PersonId == userId,
                ClassRef = classId,
                ClassAnnouncementTypeRef = classAnnouncementTypeId,
                Created = nowLocalDate,
                Expires = expiresDateTime.HasValue ? expiresDateTime.Value : DateTime.MinValue,
                State = AnnouncementState.Draft,
                GradingStyle = GradingStyleEnum.Numeric100,
                SchoolRef = Context.SchoolLocalId.Value,
                QnACount = 0,
                Order = annId,
                AttachmentsCount = 0,
                ApplicationCount = 0,
                OwnerAttachmentsCount = 0,
                StudentsCountWithAttachments = 0
            };


            data[announcement.Id] = announcement;
            return ConvertToDetails(announcement);
        }

        private AnnouncementDetails ConvertToDetails(AnnouncementComplex announcement)
        {
            var announcementAttachments = ServiceLocator.AnnouncementAttachmentService.GetAttachments(announcement.Id);

            var announcementApplications = new List<AnnouncementApplication>();
            var announcementsQnA = ServiceLocator.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id);
            var announcementStandards = AnnouncementStandardStorage.GetAnnouncementStandards(announcement.Id);

            if (Context.PersonId == null) throw new Exception("Context user local id is null");

            var isComplete = AnnouncementCompleteStorage.GetComplete(announcement.Id,
                Context.PersonId.Value);

            var cls = ServiceLocator.ClassService.GetClassDetailsById(announcement.ClassRef);
            var owner = announcement.PrimaryTeacherRef.HasValue
                ? ServiceLocator.PersonService.GetPerson(announcement.PrimaryTeacherRef.Value)
                : null;

            var studentAnnouncements =
                ServiceLocator.StudentAnnouncementService.GetStudentAnnouncements(announcement.Id);

            return new AnnouncementDetails
            {
                Id = announcement.Id,
                SisActivityId = announcement.SisActivityId,
                Complete = isComplete.HasValue && isComplete.Value,
                Subject = announcement.Subject,
                StudentsCount = announcement.StudentsCount,
                WeightAddition = announcement.WeightAddition,
                WeightMultiplier = announcement.WeightMultiplier,
                QnACount = announcement.QnACount,
                OwnerAttachmentsCount = announcement.OwnerAttachmentsCount,
                PrimaryTeacherRef = announcement.PrimaryTeacherRef,
                IsOwner = announcement.PrimaryTeacherRef == Context.PersonId,
                PrimaryTeacherName = announcement.PrimaryTeacherName,
                SchoolRef = announcement.SchoolRef,
                ClassRef = announcement.ClassRef,
                FullClassName = cls.Name + " " + cls.ClassNumber,
                PrimaryTeacherGender = announcement.PrimaryTeacherGender,
                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
                Created = announcement.Created,
                Expires = announcement.Expires,
                GradingStyle = announcement.GradingStyle,
                State = announcement.State,
                IsScored = announcement.IsScored,
                Avg = announcement.Avg,
                Title = announcement.Title,
                Content = announcement.Content,
                AnnouncementAttachments = announcementAttachments,
                AnnouncementApplications = announcementApplications,
                AnnouncementQnAs = announcementsQnA,
                AnnouncementStandards = announcementStandards,
                Owner = owner,
                ApplicationCount = announcementApplications.Count,
                AttachmentsCount = announcementAttachments.Count,
                StudentAnnouncements = studentAnnouncements,
                VisibleForStudent = announcement.VisibleForStudent,
                MayBeDropped = announcement.MayBeDropped,
                Order = announcement.Order,
                ClassAnnouncementTypeName = announcement.ClassAnnouncementTypeName,
                ChalkableAnnouncementType = announcement.ChalkableAnnouncementType,
                ClassName = announcement.ClassName,
                MaxScore = announcement.MaxScore
            };
        }

        public AnnouncementDetails GetDetails(int announcementId, int userId, int roleId)
        {
            var announcement = GetAnnouncements(new AnnouncementsQuery
            {
                Id = announcementId,
                PersonId = userId,
                RoleId = roleId
            }).Announcements.First();

            return ConvertToDetails(announcement);
        }

        private Announcement Submit(int announcementId, int? classId)
        {
            var res = GetAnnouncementDetails(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                throw new ChalkableSecurityException();
            SetClassToAnnouncement(res, classId, res.Expires);
            return SubmitAnnouncement(classId, res);
        }

        public void SubmitAnnouncement(int announcementId, int recipientId)
        {
            var res = Submit(announcementId, recipientId);

            var sy = StorageLocator.SchoolYearStorage.GetByDate(res.Expires);
            if(res.ClassAnnouncementTypeRef.HasValue)
                ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, recipientId);
        }

        public IList<AnnouncementComplex> GetByActivitiesIds(IList<int> importantActivitiesIds)
        {
            var announcements = GetAnnouncements(new AnnouncementsQuery()).Announcements;
            return announcements.Where(x => importantActivitiesIds.Contains(x.Id)).ToList();
        }


        public void AddDemoAnnouncementsForClass(int classId)
        {
            var types = StorageLocator.ClassAnnouncementTypeStorage.GetAll(classId);

            foreach (var classAnnouncementType in types)
            {
                var classRef = classAnnouncementType.ClassRef;
                var announcementType = classAnnouncementType.Id;
                var announcementDetail = Create(announcementType, classRef, DateTime.Today, DemoSchoolConstants.TeacherId,
                    DateTime.Now.AddDays(1));
                var cls = StorageLocator.ClassStorage.GetById(classRef);
                announcementDetail.ClassName = cls.Name;
                announcementDetail.FullClassName = cls.Name + " " + cls.ClassNumber;
                announcementDetail.IsScored = true;
                announcementDetail.MaxScore = 100;
                announcementDetail.Title = classAnnouncementType.Name + " " + classId;
                announcementDetail.VisibleForStudent = true;

                if (announcementDetail.AnnouncementStandards == null)
                    announcementDetail.AnnouncementStandards = new List<AnnouncementStandardDetails>();

                SubmitAnnouncement(classId, announcementDetail);
            }
        }

        public void DuplicateAnnouncement(int id, IList<int> classIds)
        {
            
        }
        
        public void SubmitForAdmin(int announcementId)
        {
            Submit(announcementId, null);
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
        
        public void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle)
        {
            throw new NotImplementedException();
        }

        public Announcement GetAnnouncementById(int id)
        {
            var res = AnnouncementStorage.GetAnnouncement(id, Context.Role.Id, Context.PersonId ?? 0); // security here 
            if (res == null)
                throw new ChalkableSecurityException();
            return res;
        }

        public AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var res = GetDetails(announcementId, Context.PersonId.Value, Context.Role.Id); ;
            res.AnnouncementStandards = ServiceLocator.StandardService.GetAnnouncementStandards(announcementId);
            return res;
        }

        public int GetNewAnnouncementItemOrder(AnnouncementDetails announcement)
        {
            var attOrder = announcement.AnnouncementAttachments.Max(x => (int?)x.Order);
            var appOrder = announcement.AnnouncementApplications.Max(x => (int?)x.Order);
            var order = 0;
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
            if (!Context.PersonId.HasValue)
                throw new Exception("User local id doesn't have a valid value");

            AnnouncementCompleteStorage.SetComplete(new AnnouncementComplete
            {
                AnnouncementId = announcementId,
                Complete = complete,
                UserId = Context.PersonId.Value
            });
        }

        public Announcement SetVisibleForStudent(int id, bool visible)
        {
            var ann = GetAnnouncementById(id);
            ann.VisibleForStudent = visible;
            AnnouncementStorage.Update(ann);
            return ann;
        }

        public Announcement GetLastDraft()
        {
            return GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = Context.PersonId ?? 0,
            }).Announcements.Where(x => x.State == AnnouncementState.Draft).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (ann.State == AnnouncementState.Draft)
                throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            
            var annRecipients = AnnouncementRecipientStorage.GetList(announcementId);
            var result = new List<Person>();
            foreach (var announcementRecipient in annRecipients)
            {
                if (announcementRecipient.PersonRef.HasValue)
                    result.Add(ServiceLocator.PersonService.GetPerson(announcementRecipient.PersonRef));
            }
            return result;
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType)
        {
            var announcements = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = personId,
                ClassId = classId,
            }).Announcements.ToList();

            return announcements.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType).Take(count).Select(x => x.Content).ToList();
        }

        public AnnouncementDetails SubmitAnnouncement(int? classId, AnnouncementDetails res)
        {
            var dateNow = Context.NowSchoolTime.Date;

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
                    activity = StorageLocator.StiActivityStorage.CreateActivity(classId.Value, activity);
                    if (Exists(activity.Id))
                        throw new ChalkableException("Announcement with such activityId already exists");
                    res.SisActivityId = activity.Id;

                    var persons = StorageLocator.PersonStorage.GetPersons(new PersonQuery
                    {
                        ClassId = classId,
                        RoleId = CoreRoles.STUDENT_ROLE.Id
                    }).Persons.Select(x => x.Id).ToList();


                    foreach (var personId in persons)
                    {
                        StorageLocator.StudentAnnouncementStorage.Add(new StudentAnnouncement
                        {
                            AnnouncementId = res.Id,
                            ActivityId = activity.Id,
                            StudentId = personId
                        });

                        StorageLocator.StiActivityScoreStorage.Add(new Score
                        {
                            ActivityId = activity.Id,
                            StudentId = personId
                        });
                    }


                    res.StudentsCount = persons.Count;


                }
            }
            res.GradingStyle = GradingStyleEnum.Numeric100;
            Update(res);
            return res;
        }

        public bool CanAddStandard(int announcementId)
        {
            var announcement = GetAnnouncementById(announcementId);
            var cls = ServiceLocator.ClassService.GetClassDetailsById(announcement.ClassRef);
            return StorageLocator.ClassStandardStorage.GetAll().Count(x => x.ClassRef == cls.Id || x.ClassRef == cls.CourseRef) > 0;
        }

        public Announcement EditTitle(int announcementId, string title)
        {
            var ann = GetAnnouncementById(announcementId);
            return EditTitle(ann, title, (da, t) => da.Exists(t, ann.ClassRef, ann.Expires, announcementId));
        }

        private Announcement EditTitle(Announcement announcement, string title, Func<IDemoAnnouncementStorage, string, bool> existsTitleAction)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if (announcement != null)
            {
                if (announcement.Title != title)
                {
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    var c = StorageLocator.ClassStorage.GetById(announcement.ClassRef);
                    if (c.PrimaryTeacherRef != Context.PersonId)
                        throw new ChalkableSecurityException();
                    if (existsTitleAction(StorageLocator.AnnouncementStorage, title))
                        throw new ChalkableException("The item with current title already exists");
                    announcement.Title = title;
                    StorageLocator.AnnouncementStorage.Update(announcement);
                }    
            }
            return announcement;
        }


        public bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return AnnouncementStorage.Exists(title, classId, expiresDate, excludeAnnouncementId);
        }


        public Standard AddAnnouncementStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann,Context))
                throw new ChalkableSecurityException();

            AnnouncementStandardStorage.Add(new AnnouncementStandard
            {
                AnnouncementRef = announcementId,
                StandardRef = standardId
            });

            return ServiceLocator.StandardService.GetStandardById(standardId);
        }
        public Standard RemoveStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();


            AnnouncementStandardStorage.Delete(announcementId, standardId);
            return ServiceLocator.StandardService.GetStandardById(standardId);
        }

        public void RemoveAllAnnouncementStandards(int standardId)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
        {
            var announcementIds = AnnouncementStandardStorage.GetAll().Select(x => x.AnnouncementRef);


            var annStandarts = new List<AnnouncementStandard>();
            foreach (var id in announcementIds)
            {
                var announcement = GetAnnouncementById(id);

                if (announcement.ClassRef == classId)
                {
                    var standards = AnnouncementStandardStorage.GetAll(id);
                    annStandarts.AddRange(standards);
                }
            }

            return annStandarts;
        }

        public void CopyAnnouncement(int id, IList<int> classIds)
        {
            var sourceAnnouncement = GetDetails(id, Context.PersonId.Value, Context.Role.Id);
            if (sourceAnnouncement.State != AnnouncementState.Created)
                throw new ChalkableException("Current announcement is not submited yet");
            if (!sourceAnnouncement.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement doesn't have activityId");

            foreach (var classId in classIds)
            {
                var announcement = Create(sourceAnnouncement.ClassAnnouncementTypeRef, classId, DateTime.Today,
                    Context.PersonId ?? 0);

                StorageLocator.AnnouncementCompleteStorage.Add(new AnnouncementComplete
                {
                    AnnouncementId = announcement.Id,
                    Complete = sourceAnnouncement.Complete,
                    UserId = Context.PersonId.Value
                });
                announcement.Complete = sourceAnnouncement.Complete;
                announcement.Subject = sourceAnnouncement.Subject;
                announcement.WeightAddition = sourceAnnouncement.WeightAddition;
                announcement.GradingStyle = sourceAnnouncement.GradingStyle;
                announcement.WeightMultiplier = sourceAnnouncement.WeightMultiplier;
                announcement.QnACount = sourceAnnouncement.QnACount;
                announcement.OwnerAttachmentsCount = sourceAnnouncement.OwnerAttachmentsCount;
                announcement.PrimaryTeacherRef = announcement.PrimaryTeacherRef;
                announcement.IsOwner = announcement.PrimaryTeacherRef == Context.PersonId;
                announcement.PrimaryTeacherName = sourceAnnouncement.PrimaryTeacherName;
                announcement.SchoolRef = sourceAnnouncement.SchoolRef;
                announcement.PrimaryTeacherGender = sourceAnnouncement.PrimaryTeacherGender;
                announcement.Expires = sourceAnnouncement.Expires;
                announcement.GradingStyle = sourceAnnouncement.GradingStyle;
                announcement.State = sourceAnnouncement.State;
                announcement.IsScored = sourceAnnouncement.IsScored;
                announcement.Avg = sourceAnnouncement.Avg;
                announcement.Title = sourceAnnouncement.Title;
                announcement.Content = sourceAnnouncement.Content;
                announcement.Owner = announcement.PrimaryTeacherRef.HasValue
                    ? StorageLocator.PersonStorage.GetById(announcement.PrimaryTeacherRef.Value)
                    : null;
                announcement.ApplicationCount = sourceAnnouncement.ApplicationCount;
                announcement.AttachmentsCount = sourceAnnouncement.AttachmentsCount;
                announcement.VisibleForStudent = sourceAnnouncement.VisibleForStudent;
                announcement.MayBeDropped = sourceAnnouncement.MayBeDropped;
                announcement.Order = sourceAnnouncement.Order;
                announcement.ClassAnnouncementTypeName = sourceAnnouncement.ClassAnnouncementTypeName;
                announcement.ChalkableAnnouncementType = sourceAnnouncement.ChalkableAnnouncementType;
                announcement.ClassName = sourceAnnouncement.ClassName;
                announcement.MaxScore = sourceAnnouncement.MaxScore;
                // Added this copying 'cause of CHLK-3240
                announcement.SisActivityId = sourceAnnouncement.Id;

                var resAnnouncement = SubmitAnnouncement(classId, announcement);
                StorageLocator.StiActivityScoreStorage.DuplicateFrom(sourceAnnouncement.SisActivityId.Value, resAnnouncement.SisActivityId.Value);
                var sourceStudentAnnouncements =
                        StorageLocator.StudentAnnouncementStorage.GetAll()
                            .Where(x => x.AnnouncementId == sourceAnnouncement.Id)
                            .ToList();

                foreach (var sourceStudentAnnouncement in sourceStudentAnnouncements)
                {
                    StorageLocator.StudentAnnouncementStorage.Add(new StudentAnnouncement
                    {
                        ActivityId = resAnnouncement.SisActivityId.Value,
                        StudentId = sourceStudentAnnouncement.StudentId,
                        Absent = sourceStudentAnnouncement.Absent,
                        AbsenceCategory = sourceStudentAnnouncement.AbsenceCategory,
                        AlphaGradeId = sourceStudentAnnouncement.AlphaGradeId,
                        AlternateScoreId = sourceStudentAnnouncement.AlternateScoreId,
                        Comment = sourceStudentAnnouncement.Comment,
                        ScoreDropped = sourceStudentAnnouncement.ScoreDropped,
                        AlternateScore = sourceStudentAnnouncement.AlternateScore,
                        Exempt = sourceStudentAnnouncement.Exempt,
                        Incomplete = sourceStudentAnnouncement.Incomplete,
                        Late = sourceStudentAnnouncement.Late,
                        NumericScore = sourceStudentAnnouncement.NumericScore,
                        OverMaxScore = sourceStudentAnnouncement.OverMaxScore,
                        ScoreValue = sourceStudentAnnouncement.ScoreValue,
                        Withdrawn = sourceStudentAnnouncement.Withdrawn,
                        AnnouncementId = resAnnouncement.Id,
                        AnnouncementTitle = sourceAnnouncement.Title,
                        ExtraCredit = sourceStudentAnnouncement.ExtraCredit,
                        IsWithdrawn = sourceStudentAnnouncement.IsWithdrawn
                    });
                }




                var announcementAttachments = StorageLocator.AnnouncementAttachmentStorage.DuplicateFrom(sourceAnnouncement.Id, resAnnouncement.Id);
                resAnnouncement.AnnouncementAttachments = announcementAttachments;
                var announcementApplications = new List<AnnouncementApplication>();

                resAnnouncement.StudentAnnouncements = StorageLocator.StudentAnnouncementStorage.GetAll(resAnnouncement.Id);
                resAnnouncement.AnnouncementApplications = announcementApplications;

                foreach (var announcementQnAComplex in sourceAnnouncement.AnnouncementQnAs)
                {
                    StorageLocator.AnnouncementQnAStorage.Add(new AnnouncementQnAComplex
                    {
                        AnnouncementRef = resAnnouncement.Id,
                        Answer = announcementQnAComplex.Answer,
                        AnswererRef = announcementQnAComplex.AnswererRef,
                        IsOwner = announcementQnAComplex.IsOwner,
                        Question = announcementQnAComplex.Question,
                        QuestionTime = announcementQnAComplex.QuestionTime,
                        State = announcementQnAComplex.State,
                        AskerRef = announcementQnAComplex.AskerRef,
                        Asker = announcementQnAComplex.Asker,
                        ClassRef = announcementQnAComplex.ClassRef,
                        Answerer = announcementQnAComplex.Answerer,
                        AnsweredTime = announcementQnAComplex.AnsweredTime
                    });
                }

                var announcementsQnA = StorageLocator.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
                {
                    AnnouncementId = resAnnouncement.Id
                }).AnnouncementQnAs;
                resAnnouncement.AnnouncementQnAs = announcementsQnA;

                foreach (var announcementStandardDetails in sourceAnnouncement.AnnouncementStandards)
                {
                    StorageLocator.AnnouncementStandardStorage.Add(new AnnouncementStandardDetails
                    {
                        AnnouncementRef = resAnnouncement.Id,
                        Standard = announcementStandardDetails.Standard,
                        StandardRef = announcementStandardDetails.StandardRef
                    });
                }

                var announcementStandards = AnnouncementStandardStorage.GetAll(resAnnouncement.Id).Select(x => new AnnouncementStandardDetails
                    {
                        AnnouncementRef = x.AnnouncementRef,
                        StandardRef = x.StandardRef,
                        Standard = StorageLocator.StandardStorage.GetById(x.StandardRef)
                    }).ToList();
                resAnnouncement.AnnouncementStandards = announcementStandards;

                Update(resAnnouncement);
            }
        }

        public IAnnouncementService GetTeacherAnnouncementService()
        {
            throw new NotImplementedException();
        }

        public void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
        {
            if (!Context.PersonId.HasValue)
                throw new Exception("User local id doesn't have a valid value");
            var annsResult = GetAnnouncements(new AnnouncementsQuery
                {
                    ToDate = toDate,
                    PersonId = Context.PersonId,
                    Complete = !complete,
                    RoleId = Context.RoleId,
                });
            foreach (var ann in annsResult.Announcements)
                SetComplete(ann.Id, Context.PersonId.Value, complete);
        }
    }
}
