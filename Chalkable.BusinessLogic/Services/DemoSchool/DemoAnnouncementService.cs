//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Chalkable.BusinessLogic.Common;
//using Chalkable.BusinessLogic.Mapping.ModelMappers;
//using Chalkable.BusinessLogic.Model;
//using Chalkable.BusinessLogic.Security;
//using Chalkable.BusinessLogic.Services.DemoSchool.Common;
//using Chalkable.BusinessLogic.Services.School;
//using Chalkable.Common;
//using Chalkable.Common.Exceptions;
//using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
//using Chalkable.Data.School.Model;
//using Chalkable.Data.School.Model.Announcements;
//using Chalkable.StiConnector.Connectors.Model;

//namespace Chalkable.BusinessLogic.Services.DemoSchool
//{

//    public class AnnouncementComplete
//    {
//        public int AnnouncementId { get; set; }
//        public int UserId { get; set; }
//        public bool Complete { get; set; }
//    }

//    public class DemoStiActivityStorage:BaseDemoIntStorage<Activity>
//    {
//        public DemoStiActivityStorage()
//            : base(x => x.Id, true)
//        {
//        }

//        public Activity CreateActivity(int classId, Activity activity)
//        {
//            activity.SectionId = classId;
//            activity.Id = GetNextFreeId();
//            data.Add(activity.Id, activity);
//            return activity;
//        }


//        public bool Exists(int id)
//        {
//            return data.Count(x => x.Value.Id == id) > 0;
//        }

//        public void CopyActivity(int sisActivityId, IList<int> classIds)
//        {
//            var activity = GetById(sisActivityId);

//            var classIdsFiltered = classIds.Where(x => x != activity.SectionId).ToList();

//            foreach (var classId in classIdsFiltered)
//            {
//                activity.Id = GetNextFreeId();
//                activity.SectionId = classId;

//                if (activity.Attributes == null)
//                    activity.Attributes = new List<ActivityAssignedAttribute>();

//                foreach (var attachment in activity.Attributes)
//                {
//                    attachment.ActivityId = activity.Id;
//                }
//                data.Add(activity.Id, activity);
//            }
//        }

//        public IEnumerable<Activity> GetAll(int sectionId)
//        {
//            return data.Where(x => x.Value.SectionId == sectionId).Select(x => x.Value).ToList();
//        }
//    }

//    public class DemoAnnouncementStorage : BaseDemoIntStorage<AnnouncementComplex>
//    {
//        public DemoAnnouncementStorage()
//            : base(x => x.Id)
//        {

//        }

//        public DemoAnnouncementStorage(Dictionary<int, AnnouncementComplex> anns, int lastIndex)
//            : base(x => x.Id)
//        {
//            data = anns;
//            Index = lastIndex;
//        }


//        public new Announcement GetById(int announcementId)
//        {
//            return data.ContainsKey(announcementId) ? data[announcementId] : null;
//        }

//        public bool Exists(string s, int classId, DateTime expiresDate, int? excludeAnnouncementId)
//        {
//            return data.Count(x => x.Value.Title == s && x.Value.ClassRef == classId 
//                && x.Value.Expires.Date == expiresDate.Date && excludeAnnouncementId != x.Value.Id) > 0;
//        }
//    }

//    public class DemoAnnouncementCompleteStorage : BaseDemoIntStorage<AnnouncementComplete>
//    {
//        public DemoAnnouncementCompleteStorage()
//            : base(null, true)
//        {
//        }

//        public void SetComplete(AnnouncementComplete complete)
//        {
//            if (data.Count(x => x.Value.AnnouncementId == complete.AnnouncementId && x.Value.UserId == complete.UserId) == 0)
//            {
//                data.Add(GetNextFreeId(), complete);
//            }
//            var item = data.First(x => x.Value.AnnouncementId == complete.AnnouncementId && x.Value.UserId == complete.UserId).Key;
//            data[item] = complete;
//        }

//        public bool? GetComplete(int announcementId, int userId)
//        {
//            if (data.Count(x => x.Value.AnnouncementId == announcementId && x.Value.UserId == userId) == 0)
//            {
//                return false;
//            }
//            return data.First(x => x.Value.AnnouncementId == announcementId && x.Value.UserId == userId).Value.Complete;
//        }
//    }

//    public class DemoAnnouncementRecipientStorage : BaseDemoIntStorage<AdminAnnouncementRecipient>
//    {
//        public DemoAnnouncementRecipientStorage()
//            : base(x => x.Id)
//        {
//        }


//        public void DeleteByAnnouncementId(int announcementId)
//        {
//            var annRep = data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Key).ToList();
//            Delete(annRep);
//        }

//        public IList<AdminAnnouncementRecipient> GetList(int announcementId)
//        {
//            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
//        }

//        public IList<AdminAnnouncementRecipient> GetList(int announcementId, int? roleId, int? personId, bool toAll = false)
//        {
//            //TODO rewrites impl
//            var recipients = data.Select(x => x.Value);
//            return recipients.ToList();
//        }
//    }

//    public class DemoAnnouncementService : DemoSchoolServiceBase, IAnnouncementService
//    {
//        private DemoAnnouncementCompleteStorage AnnouncementCompleteStorage { get; set; }
//        private DemoAnnouncementStorage AnnouncementStorage { get; set; }
//        private DemoAnnouncementRecipientStorage AnnouncementRecipientStorage { get; set; }
        
//        private DemoStiActivityStorage ActivityStorage { get; set; }
//        private IAnnouncementProcessor AnnouncementProcessor { get; set; }
        
//        public DemoAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
//        {
//            AnnouncementCompleteStorage = new DemoAnnouncementCompleteStorage();
//            AnnouncementRecipientStorage = new DemoAnnouncementRecipientStorage();
//            AnnouncementStorage = new DemoAnnouncementStorage();
//            ActivityStorage = new DemoStiActivityStorage();
//            SetupAnnouncementProcessor(Context, serviceLocator);
//        }

//        public DemoAnnouncementService(IServiceLocatorSchool serviceLocator, DemoAnnouncementCompleteStorage announcementCompleteStorage,
//            DemoAnnouncementStorage announcementStorage, DemoAnnouncementRecipientStorage announcementRecipientStorage) : base(serviceLocator)
//        {
//            AnnouncementStorage = announcementStorage;
//            AnnouncementCompleteStorage = announcementCompleteStorage;
//            AnnouncementRecipientStorage = announcementRecipientStorage;
//        }

//        public void SetAnnouncementProcessor(IAnnouncementProcessor processor)
//        {
//            AnnouncementProcessor = processor;
//        }

//        public void SetupAnnouncementProcessor(UserContext context, IServiceLocatorSchool locator)
//        {
//            if (BaseSecurity.IsDistrictAdmin(context))
//                SetAnnouncementProcessor(new AdminAnnouncementProcessor(locator));
//            if (Context.Role == CoreRoles.TEACHER_ROLE)
//                SetAnnouncementProcessor(new TeacherAnnouncementProcessor(locator));
//            if (Context.Role == CoreRoles.STUDENT_ROLE)
//                SetAnnouncementProcessor(new StudentAnnouncementProcessor(locator));
//        }

//        public Announcement GetAnnouncement(int announcementId, int roleId, int userId)
//        {
//            return AnnouncementProcessor.GetAnnouncement(AnnouncementStorage.GetData().Select(x => x.Value), announcementId, roleId, userId);
//        }

//        public IList<AdminAnnouncementRecipient> GetAnnouncementRecipients(int announcementId, int? roleId, int? personId, bool toAll = false)
//        {
//            return AnnouncementRecipientStorage.GetList(announcementId, roleId, personId, toAll);
//        }

//        public IList<AdminAnnouncementRecipient> GetAnnouncementRecipients(int? announcementId)
//        {
//            return announcementId.HasValue
//                ? AnnouncementRecipientStorage.GetList(announcementId.Value)
//                : AnnouncementRecipientStorage.GetAll();
//        }

//        public void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state)
//        {
//            var announcementsToDelete = GetAnnouncements(new AnnouncementsQuery
//            {
//                PersonId = userId,
//                Id = announcementId,
//                ClassId = classId
//            }).Announcements;
//            if (state.HasValue)
//                announcementsToDelete = announcementsToDelete.Where(x => x.State == state).ToList();
//            if (announcementType.HasValue)
//                announcementsToDelete = announcementsToDelete.Where(x => x.ClassAnnouncementTypeRef == announcementType).ToList();

//            foreach (var announcementComplex in announcementsToDelete)
//            {
//                if (announcementComplex.SisActivityId.HasValue)
//                {
//                    ((DemoStudentAnnouncementService) ServiceLocator.StudentAnnouncementService)
//                        .DeleteStudentAnnouncements(announcementComplex.Id, announcementComplex.SisActivityId.Value);

//                    var qnas = ServiceLocator.AnnouncementQnAService.GetAnnouncementQnAs(announcementComplex.Id);
//                    foreach (var announcementQnAComplex in qnas)
//                    {
//                        ServiceLocator.AnnouncementQnAService.Delete(announcementQnAComplex.Id);    
//                    }
//                    ActivityStorage.Delete(announcementComplex.SisActivityId.Value);
//                }
//                ((DemoApplicationSchoolService)ServiceLocator.ApplicationSchoolService).DeleteAnnouncementApplications(announcementComplex.Id, true);
//                ((DemoAnnouncementAttachmentService)ServiceLocator.AnnouncementAttachmentService).DeleteAttachments(announcementComplex.Id);

//                var standards = GetAnnouncementStandards(announcementComplex.Id);
//                ((DemoStandardService)ServiceLocator.StandardService).DeleteAnnouncementStandards(standards);
                
//            }

//            AnnouncementStorage.Delete(announcementsToDelete.Select(x => x.Id).ToList());
//        }

//        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
//        {
//            query.RoleId = Context.Role.Id;
//            query.PersonId = Context.PersonId;
//            query.Now = Context.NowSchoolTime.Date;
//            if (!Context.PersonId.HasValue)
//                throw new UnassignedUserException();
//            var announcements = AnnouncementStorage.GetData().Select(x => x.Value);
//            if (query.Id.HasValue) 
//                announcements = announcements.Where(x => x.Id == query.Id);

//            if (query.ClassId.HasValue)
//                announcements = announcements.Where(x => x.ClassRef == query.ClassId);

//            if (query.MarkingPeriodId.HasValue)
//            {
//                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(query.MarkingPeriodId.Value);
//                announcements = announcements.Where(x => x.Expires >= mp.StartDate && x.Expires <= mp.EndDate);
//            }

//            if (query.FromDate.HasValue)
//                announcements = announcements.Where(x => x.Expires >= query.FromDate);

//            if (query.ToDate.HasValue)
//                announcements = announcements.Where(x => x.Expires <= query.ToDate);
//            if (query.Complete.HasValue)
//                announcements = announcements.Where(x => AnnouncementCompleteStorage.GetComplete(x.Id, Context.PersonId.Value) == query.Complete);


//            foreach (var announcementComplex in announcements)
//            {
//                var complete = AnnouncementCompleteStorage.GetComplete(announcementComplex.Id, Context.PersonId.Value);
//                if (announcementComplex.ClassRef.HasValue)
//                {
//                    var cls = ServiceLocator.ClassService.GetById(announcementComplex.ClassRef.Value);
//                    announcementComplex.FullClassName = cls.Name + " " + cls.ClassNumber;    
//                }
//                announcementComplex.Complete = complete.HasValue && complete.Value;
//            }
//            if (query.OwnedOnly)
//                announcements = announcements.Where(x => x.PrimaryTeacherRef == query.PersonId);

//            if (query.SisActivitiesIds != null)
//                announcements = announcements.Where(x => query.SisActivitiesIds.Contains(x.Id));


//            announcements = AnnouncementProcessor.GetAnnouncements(announcements, query);

//            if (query.Start > 0)
//                announcements = announcements.Skip(query.Start);
//            if (query.Count > 0)
//                announcements = announcements.Take(query.Count);

//            return new AnnouncementQueryResult
//            {
//                Announcements = announcements.ToList(),
//                Query = query,
//                SourceCount = AnnouncementStorage.GetData().Count
//            };
//        }

//        public IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly)
//        {
//            return GetAnnouncements(new AnnouncementsQuery {Count = count, GradedOnly = gradedOnly}).Announcements;
//        }

//        public IList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false)
//        {
//            return GetAnnouncementsForFeed(false, start, count, null, null, onlyOwners);
//        }
//        public IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false, bool? graded = null)
//        {
//            var q = new AnnouncementsQuery
//            {
//                Complete = complete,
//                Start = start,
//                Count = count,
//                ClassId = classId,
//                MarkingPeriodId = markingPeriodId,
//                OwnedOnly = ownerOnly,
//                Graded = graded
//            };
//            return GetAnnouncementsComplex(q);
//        }

//        public IList<AnnouncementComplex> GetAnnouncements(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, bool includeAdminAnnouncement = false)
//        {
//            var q = new AnnouncementsQuery
//                {
//                    FromDate = fromDate,
//                    ToDate = toDate,
//                    ClassId = classId
//                };
//            return GetAnnouncementsComplex(q);
//        }

//        public IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null)
//        {
//            var anns = GetAnnouncements(query).Announcements;
//            return anns;
//        }

//        public IList<AnnouncementComplex> GetAnnouncements(string filter)
//        {
//            var anns = GetAnnouncements(new AnnouncementsQuery()).Announcements;
//            IList<AnnouncementComplex> res = new List<AnnouncementComplex>();
//            if (string.IsNullOrEmpty(filter)) 
//                return res;
//            var words = filter.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
//            for (var i = 0; i < words.Count(); i++)
//            {
//                var word = words[i];
//                int annOrder;
//                IList<AnnouncementComplex> currentAnns;
//                if (int.TryParse(words[i], out annOrder))
//                {
//                    currentAnns = anns.Where(x => x.Order == annOrder).ToList();
//                }
//                else
//                {
//                    currentAnns = anns.Where(x =>
//                        (x.Subject != null && x.Subject.ToLower().Contains(word))
//                        || (x.ClassName.ToLower().Contains(word))
//                        || ("all".Contains(word))
//                        || x.ClassAnnouncementTypeName.ToLower().Contains(word)
//                        || x.Title != null && x.Title.ToLower().Contains(word)
//                        || x.Content != null && x.Content.ToLower().Contains(word)
//                        ).ToList();
//                }
//                res = res.Union(currentAnns).ToList();
//            }
//            return res;
//        }

//        public AnnouncementDetails CreateAnnouncement(ClassAnnouncementType classAnnType, int classId, DateTime expiresDate)
//        {
//            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
//                throw new ChalkableSecurityException();
//            var draft = GetLastDraft();
//            var res = Create(classAnnType.Id, classId, expiresDate, Context.PersonId ?? 0);

//            if (draft != null)
//            {
//                res.Content = draft.Content;
//            }
//            return ConvertToDetails(res);
//        }

//        public void DeleteAnnouncement (int announcementId)
//        {
//            var announcement = AnnouncementStorage.GetById(announcementId);
//            if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
//                throw new ChalkableSecurityException();
//            Delete(announcementId, null, null, null, null);
                
//        }

//        public void DeleteAnnouncements(int classId, int? announcementType, AnnouncementState state)
//        {
//            Delete(null, Context.PersonId, classId, announcementType, state);
//        }

//        public void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
//        {
//            if (Context.PersonId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
//                throw new ChalkableSecurityException();

//            Delete(null, Context.PersonId, null, null, state);
//        }

//        public Announcement DropUnDropAnnouncement(int announcementId, bool drop)
//        {
//            var ann = AnnouncementStorage.GetById(announcementId);
//            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
//                throw new ChalkableSecurityException();

//            ((DemoStudentAnnouncementService)ServiceLocator.StudentAnnouncementService).DropUndropAnnouncement(announcementId, drop);
//            ann.Dropped = drop;
//            AnnouncementStorage.Update((AnnouncementComplex)ann);
//            return ann;
//        }

//        public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
//        {
//            throw new NotImplementedException();
//        }

//        public AnnouncementDetails EditAnnouncement(AnnouncementInfo announcement, int? classId = null, IList<int> groupsIds = null)
//        {

//            if (!Context.PersonId.HasValue)
//                throw new UnassignedUserException();
            
//            var ann = GetAnnouncement(announcement.AnnouncementId, Context.RoleId, Context.PersonId.Value);
//            AnnouncementSecurity.EnsureInModifyAccess(ann, Context);

//            ann.Content = announcement.Content;
//            ann.Subject = announcement.Subject;
//            if (Context.Role == CoreRoles.TEACHER_ROLE && announcement.ClassAnnouncementTypeId.HasValue)
//            {
//                ann.ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeId.Value;
//                ann.IsScored = announcement.MaxScore > 0;
//                ann.MaxScore = announcement.MaxScore;
//                ann.WeightAddition = announcement.WeightAddition;
//                ann.WeightMultiplier = announcement.WeightMultiplier;
//                ann.MayBeDropped = announcement.CanDropStudentScore;
//                ann.VisibleForStudent = !announcement.HideFromStudents;

//                if (!ann.IsScored && ann.SisActivityId.HasValue)
//                {
//                    ((DemoStudentAnnouncementService) ServiceLocator.StudentAnnouncementService).ResetScores(ann.Id, ann.SisActivityId.Value);
                    
//                }

//                if (classId.HasValue && ann.ClassRef != classId.Value && ann.State == AnnouncementState.Draft)
//                {
//                    // clearing some data before switching between classes
//                    ann.Title = null;
//                    ((DemoApplicationSchoolService)ServiceLocator.ApplicationSchoolService).DeleteAnnouncementApplications(ann.Id);
//                }
//            }
//            if (BaseSecurity.IsDistrictAdmin(Context))
//                throw new NotImplementedException();

//            ann.Expires = announcement.ExpiresDate.HasValue ? announcement.ExpiresDate.Value : DateTime.Today.AddDays(1);
//            ann = SetClassToAnnouncement(ann, classId, ann.Expires);
//            AnnouncementStorage.Update((AnnouncementComplex)ann);

//            return GetDetails(announcement.AnnouncementId, Context.PersonId.Value, Context.RoleId);
//        }


//        private AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, 
//            DateTime? expiresDateTime = null)
//        {
//            if (Context.SchoolLocalId == null) throw new Exception("Context school local id is null");

//            var annId = AnnouncementStorage.GetNextFreeId();
//            var person = ServiceLocator.PersonService.GetPerson(userId);

//            if (!classAnnouncementTypeId.HasValue)
//                classAnnouncementTypeId = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId).First().Id;

//            //todo: create admin announcements if it's admin


//            var cls = ServiceLocator.ClassService.GetById(classId);
//            var announcement = new AnnouncementComplex
//            {
//                ClassAnnouncementTypeName = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(classAnnouncementTypeId.Value).Name,
//                ChalkableAnnouncementType = classAnnouncementTypeId,
//                PrimaryTeacherName = person.FullName(),
//                ClassName = cls.Name,
//                PrimaryTeacherGender = person.Gender,
//                FullClassName = cls.Name + " " + cls.ClassNumber,
//                IsScored = false,
//                Id = annId,
//                SisActivityId = annId,
//                PrimaryTeacherRef = userId,
//                IsOwner = Context.PersonId == userId,
//                ClassRef = classId,
//                ClassAnnouncementTypeRef = classAnnouncementTypeId,
//                Created = nowLocalDate,
//                Expires = expiresDateTime.HasValue ? expiresDateTime.Value : DateTime.Today,
//                State = AnnouncementState.Draft,
//                GradingStyle = GradingStyleEnum.Numeric100,
//                SchoolRef = Context.SchoolLocalId.Value,
//                QnACount = 0,
//                Order = annId,
//                AttachmentsCount = 0,
//                ApplicationCount = 0,
//                OwnerAttachmentsCount = 0,
//                StudentsCountWithAttachments = 0
//            };

//            AnnouncementStorage.Add(announcement);
//            return ConvertToBasicDetails(announcement);
//        }

//        private AnnouncementDetails ConvertToBasicDetails(AnnouncementComplex announcement)
//        {
//            return new AnnouncementDetails
//            {
//                Id = announcement.Id,
//                SisActivityId = announcement.SisActivityId,
//                Subject = announcement.Subject,
//                StudentsCount = announcement.StudentsCount,
//                WeightAddition = announcement.WeightAddition,
//                WeightMultiplier = announcement.WeightMultiplier,
//                QnACount = announcement.QnACount,
//                OwnerAttachmentsCount = announcement.OwnerAttachmentsCount,
//                PrimaryTeacherRef = announcement.PrimaryTeacherRef,
//                IsOwner = announcement.PrimaryTeacherRef == Context.PersonId,
//                PrimaryTeacherName = announcement.PrimaryTeacherName,
//                SchoolRef = announcement.SchoolRef,
//                ClassRef = announcement.ClassRef,
//                PrimaryTeacherGender = announcement.PrimaryTeacherGender,
//                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
//                Created = announcement.Created,
//                Expires = announcement.Expires,
//                GradingStyle = announcement.GradingStyle,
//                State = announcement.State,
//                IsScored = announcement.IsScored,
//                Avg = announcement.Avg,
//                Title = announcement.Title,
//                Content = announcement.Content,
//                VisibleForStudent = announcement.VisibleForStudent,
//                MayBeDropped = announcement.MayBeDropped,
//                Order = announcement.Order,
//                ClassAnnouncementTypeName = announcement.ClassAnnouncementTypeName,
//                ChalkableAnnouncementType = announcement.ChalkableAnnouncementType,
//                ClassName = announcement.ClassName,
//                MaxScore = announcement.MaxScore
//            };
//        }

//        private AnnouncementDetails ConvertToDetails(AnnouncementComplex announcement)
//        {
//            var announcementAttachments = ServiceLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(announcement.Id);

//            var announcementApplications = new List<AnnouncementApplication>();
//            var announcementsQnA = ServiceLocator.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id);
//            var announcementStandards = ((DemoStandardService)ServiceLocator.StandardService).GetAnnouncementStandards(announcement.Id);

//            if (Context.PersonId == null) throw new Exception("Context user local id is null");

//            var isComplete = AnnouncementCompleteStorage.GetComplete(announcement.Id,
//                Context.PersonId.Value);

//            var cls = announcement.ClassRef.HasValue 
//                ? ServiceLocator.ClassService.GetClassDetailsById(announcement.ClassRef.Value)
//                : null;
//            var owner = announcement.PrimaryTeacherRef.HasValue
//                ? ServiceLocator.PersonService.GetPerson(announcement.PrimaryTeacherRef.Value)
//                : null;

            
//            return new AnnouncementDetails
//            {
//                Id = announcement.Id,
//                SisActivityId = announcement.SisActivityId,
//                Complete = isComplete.HasValue && isComplete.Value,
//                Subject = announcement.Subject,
//                StudentsCount = announcement.StudentsCount,
//                WeightAddition = announcement.WeightAddition,
//                WeightMultiplier = announcement.WeightMultiplier,
//                QnACount = announcement.QnACount,
//                OwnerAttachmentsCount = announcement.OwnerAttachmentsCount,
//                PrimaryTeacherRef = announcement.PrimaryTeacherRef,
//                IsOwner = announcement.PrimaryTeacherRef == Context.PersonId,
//                PrimaryTeacherName = announcement.PrimaryTeacherName,
//                SchoolRef = announcement.SchoolRef,
//                ClassRef = announcement.ClassRef,
//                FullClassName = cls != null ?  cls.Name + " " + cls.ClassNumber : null,
//                PrimaryTeacherGender = announcement.PrimaryTeacherGender,
//                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
//                Created = announcement.Created,
//                Expires = announcement.Expires,
//                GradingStyle = announcement.GradingStyle,
//                State = announcement.State,
//                IsScored = announcement.IsScored,
//                Avg = announcement.Avg,
//                Title = announcement.Title,
//                Content = announcement.Content,
//                AnnouncementAttachments = announcementAttachments,
//                AnnouncementApplications = announcementApplications,
//                AnnouncementQnAs = announcementsQnA,
//                AnnouncementStandards = announcementStandards,
//                Owner = owner,
//                ApplicationCount = announcementApplications.Count,
//                AttachmentsCount = announcementAttachments.Count,
//                VisibleForStudent = announcement.VisibleForStudent,
//                MayBeDropped = announcement.MayBeDropped,
//                Order = announcement.Order,
//                ClassAnnouncementTypeName = announcement.ClassAnnouncementTypeName,
//                ChalkableAnnouncementType = announcement.ChalkableAnnouncementType,
//                ClassName = announcement.ClassName,
//                MaxScore = announcement.MaxScore,
//                StudentAnnouncements =
//                    ServiceLocator.StudentAnnouncementService.GetStudentAnnouncements(announcement.Id)
//            };
//        }

//        public AnnouncementDetails GetDetails(int announcementId, int userId, int roleId)
//        {
//            var announcement = GetAnnouncements(new AnnouncementsQuery
//            {
//                Id = announcementId,
//                PersonId = userId,
//                RoleId = roleId
//            }).Announcements.First();

//            return ConvertToDetails(announcement);
//        }

//        private Announcement Submit(int announcementId, int? classId)
//        {
//            var res = GetAnnouncementDetails(announcementId);
//            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
//                throw new ChalkableSecurityException();
//            SetClassToAnnouncement(res, classId, res.Expires);
//            return SubmitAnnouncement(classId, res);
//        }

//        public void SubmitAnnouncement(int announcementId, int recipientId)
//        {
//            var res = Submit(announcementId, recipientId);

//            var sy = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetByDate(res.Expires);
//            if(res.ClassAnnouncementTypeRef.HasValue)
//                ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, recipientId);
//        }

//        private void ReorderAnnouncements(int id, int value, int recipientId)
//        {
//        }

//        public IList<AnnouncementComplex> GetByActivitiesIds(IList<int> importantActivitiesIds)
//        {
//            var announcements = GetAnnouncements(new AnnouncementsQuery()).Announcements;
//            return announcements.Where(x => importantActivitiesIds.Contains(x.Id)).ToList();
//        }


//        public void AddDemoAnnouncementsForClass(int classId)
//        {
//            var types = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId);

//            foreach (var classAnnouncementType in types)
//            {
//                var announcementType = classAnnouncementType.Id;
//                var announcementDetail = Create(announcementType, classId, DateTime.Today, DemoSchoolConstants.TeacherId,
//                    DateTime.Now.AddDays(1));
//                var cls = ServiceLocator.ClassService.GetById(classId);
//                announcementDetail.ClassName = cls.Name;
//                announcementDetail.FullClassName = cls.Name + " " + cls.ClassNumber;
//                announcementDetail.IsScored = true;
//                announcementDetail.MaxScore = 100;
//                announcementDetail.Title = classAnnouncementType.Name + " " + classId;
//                announcementDetail.VisibleForStudent = true;
//                SubmitAnnouncement(classId, (AnnouncementDetails)announcementDetail);
//            }
//        }

//        public void DuplicateAnnouncement(int id, IList<int> classIds)
//        {
            
//        }
        
//        public void SubmitForAdmin(int announcementId)
//        {
//            Submit(announcementId, null);
//        }
      
//        private Announcement SetClassToAnnouncement(Announcement announcement, int? classId, DateTime expiresDate)
//        {
//            if (classId.HasValue)
//            {
//                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(expiresDate);
//                if (mp == null)
//                    throw new NoMarkingPeriodException();
//                var mpc = ServiceLocator.MarkingPeriodService.GetMarkingPeriodClass(classId.Value, mp.Id);
//                if (mpc == null)
//                    throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_MARKING_PERIOD);
//                if (announcement.State == AnnouncementState.Created && announcement.ClassRef != classId)
//                      throw new ChalkableException("Class can't be changed for submmited announcement");
//                announcement.ClassRef = classId.Value;
//            }
//            return announcement;
//        }
        
//        public void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle)
//        {
//            throw new NotImplementedException();
//        }

//        public Announcement GetAnnouncementById(int id)
//        {
//            var res = GetAnnouncement(id, Context.Role.Id, Context.PersonId ?? 0); // security here 
//            if (res == null)
//                throw new ChalkableSecurityException();
//            return res;
//        }

//        public AnnouncementDetails GetAnnouncementDetails(int announcementId)
//        {
//            if (!Context.PersonId.HasValue)
//                throw new UnassignedUserException();
//            var res = GetDetails(announcementId, Context.PersonId.Value, Context.Role.Id); ;
//            res.AnnouncementStandards = ServiceLocator.StandardService.GetAnnouncementStandards(announcementId);
//            return res;
//        }

//        public int GetNewAnnouncementItemOrder(AnnouncementDetails announcement)
//        {
//            var attOrder = announcement.AnnouncementAttachments.Max(x => (int?)x.Order);
//            var appOrder = announcement.AnnouncementApplications.Max(x => (int?)x.Order);
//            var order = 0;
//            if (attOrder.HasValue)
//            {
//                if (appOrder.HasValue)
//                {
//                    order = Math.Max(attOrder.Value, appOrder.Value) + 1;
//                }
//                else
//                {
//                    order = attOrder.Value + 1;
//                }
//            }
//            else
//            {
//                if (appOrder.HasValue)
//                {
//                    order = appOrder.Value + 1;
//                }
//            }
//            return order;
//        }

//        public void SetComplete(int announcementId, bool complete)
//        {
//            if (!Context.PersonId.HasValue)
//                throw new UnassignedUserException("User local id doesn't have a valid value");

//            AnnouncementCompleteStorage.SetComplete(new AnnouncementComplete
//            {
//                AnnouncementId = announcementId,
//                Complete = complete,
//                UserId = Context.PersonId.Value
//            });
//        }

//        public Announcement SetVisibleForStudent(int id, bool visible)
//        {
//            var ann = GetAnnouncementById(id);
//            ann.VisibleForStudent = visible;
//            AnnouncementStorage.Update((AnnouncementComplex)ann);
//            return ann;
//        }

//        public Announcement GetLastDraft()
//        {
//            return GetAnnouncements(new AnnouncementsQuery
//            {
//                PersonId = Context.PersonId ?? 0,
//            }).Announcements.Where(x => x.State == AnnouncementState.Draft).OrderByDescending(x => x.Id).FirstOrDefault();
//        }

//        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
//        {
//            //TODO : implement this later
//            throw new NotImplementedException();
//            //var ann = GetAnnouncementById(announcementId);
//            //if (ann.State == AnnouncementState.Draft)
//            //    throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            
//            //var annRecipients = AnnouncementRecipientStorage.GetList(announcementId);
//            //var result = new List<Person>();
//            //foreach (var announcementRecipient in annRecipients)
//            //{
//            //    if (announcementRecipient.PersonRef.HasValue)
//            //        result.Add(ServiceLocator.PersonService.GetPerson(announcementRecipient.PersonRef ?? 0));
//            //}
//            //return result;
//        }

//        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType)
//        {
//            var announcements = GetAnnouncements(new AnnouncementsQuery
//            {
//                PersonId = personId,
//                ClassId = classId,
//            }).Announcements.ToList();

//            return announcements.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType).Take(10).Select(x => x.Content).ToList();
//        }

//        public AnnouncementDetails SubmitAnnouncement(int? classId, AnnouncementDetails res)
//        {
//            var dateNow = Context.NowSchoolTime.Date;

//            if (res.State == AnnouncementState.Draft)
//            {
//                res.State = AnnouncementState.Created;
//                res.Created = dateNow;
//                if (string.IsNullOrEmpty(res.Title) || res.DefaultTitle == res.Title)
//                    res.Title = res.DefaultTitle;
//                if (classId.HasValue)
//                {
//                    var activity = new Activity();
//                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
//                    activity = ActivityStorage.CreateActivity(classId.Value, activity);
//                    res.SisActivityId = activity.Id;

//                    var persons = ((DemoPersonService)ServiceLocator.PersonService).GetPersons(new PersonQuery
//                    {
//                        ClassId = classId,
//                        RoleId = CoreRoles.STUDENT_ROLE.Id
//                    }).Persons.Select(x => x.Id).ToList();


//                    foreach (var personId in persons)
//                    {
//                        ((DemoStudentAnnouncementService)ServiceLocator.StudentAnnouncementService)
//                            .AddStudentAnnouncement(new StudentAnnouncement
//                            {
//                                AnnouncementId = res.Id,
//                                ActivityId = activity.Id,
//                                StudentId = personId
//                            });
//                    }
//                    res.StudentsCount = persons.Count;
//                }
//            }
//            res.GradingStyle = GradingStyleEnum.Numeric100;
//            res = ConvertToDetails(res);
//            AnnouncementStorage.Update(res);
//            return res;
//        }

//        public bool CanAddStandard(int announcementId)
//        {
//            var announcement = GetAnnouncementById(announcementId);
//            if (!announcement.ClassRef.HasValue) 
//                return false;
//            var cls = ServiceLocator.ClassService.GetClassDetailsById(announcement.ClassRef.Value);
//            return ((DemoStandardService)ServiceLocator.StandardService).ClassStandardsExist(cls);
//        }

//        public Announcement EditTitle(int announcementId, string title)
//        {
//            var ann = GetAnnouncementById(announcementId);
//            if(!ann.ClassRef.HasValue)
//                throw new NotImplementedException(); //TODO: implement if is admin announcement
//            return EditTitle(ann, title, (da, t) => da.Exists(t, ann.ClassRef.Value, ann.Expires, announcementId));
//        }

//        private Announcement EditTitle(Announcement announcement, string title, Func<DemoAnnouncementStorage, string, bool> existsTitleAction)
//        {
//            if (!Context.PersonId.HasValue)
//                throw new UnassignedUserException();
//            if (announcement != null)
//            {
//                if (announcement.Title == title) return announcement;

//                if (string.IsNullOrEmpty(title))
//                    throw new ChalkableException("Title parameter is empty");
//                if (!announcement.ClassRef.HasValue)
//                    throw new NotImplementedException(); //TODO: implement if is admin announcement

//                var c = ServiceLocator.ClassService.GetById(announcement.ClassRef.Value);
//                if (c.PrimaryTeacherRef != Context.PersonId)
//                    throw new ChalkableSecurityException();
//                if (existsTitleAction(AnnouncementStorage, title))
//                    throw new ChalkableException("The item with current title already exists");
//                announcement.Title = title;
//                AnnouncementStorage.Update((AnnouncementComplex)announcement);
//            }
//            return announcement;
//        }


//        public bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
//        {
//            return AnnouncementStorage.Exists(title, classId, expiresDate, excludeAnnouncementId);
//        }


//        public Standard AddAnnouncementStandard(int announcementId, int standardId)
//        {
//            var ann = GetAnnouncementById(announcementId);
//            if(!AnnouncementSecurity.CanModifyAnnouncement(ann,Context))
//                throw new ChalkableSecurityException();

//            ((DemoStandardService)ServiceLocator.StandardService).AddAnnouncementStandard(new AnnouncementStandard
//            {
//                AnnouncementRef = announcementId,
//                StandardRef = standardId
//            });
//            return ServiceLocator.StandardService.GetStandardById(standardId);
//        }
//        public Standard RemoveStandard(int announcementId, int standardId)
//        {
//            var ann = GetAnnouncementById(announcementId);
//            if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
//                throw new ChalkableSecurityException();

//            ((DemoStandardService) ServiceLocator.StandardService).DeleteAnnouncementStandards(announcementId, standardId);
            
//            return ServiceLocator.StandardService.GetStandardById(standardId);
//        }

//        public void RemoveAllAnnouncementStandards(int standardId)
//        {
//            ((DemoStandardService)ServiceLocator.StandardService).DeleteAnnouncementStandards(standardId);
//        }

//        public IList<AnnouncementStandard> GetAnnouncementStandards(int announcementId)
//        {
//            var annStandards = ((DemoStandardService) ServiceLocator.StandardService).GetAnnouncementStandards(announcementId);
//            var res = annStandards.Select(anDetail => new AnnouncementStandard
//            {
//                AnnouncementRef = anDetail.AnnouncementRef, 
//                StandardRef = anDetail.StandardRef
//            }).ToList();

//            return res;
//        }

//        public void CopyAnnouncement(int id, IList<int> classIds)
//        {
//            var sourceAnnouncement = GetDetails(id, Context.PersonId.Value, Context.Role.Id);
//            if (sourceAnnouncement.State != AnnouncementState.Created)
//                throw new ChalkableException("Current announcement is not submited yet");
//            if (!sourceAnnouncement.SisActivityId.HasValue)
//                throw new ChalkableException("Current announcement doesn't have activityId");

//            foreach (var classId in classIds)
//            {
//                var announcement = Create(sourceAnnouncement.ClassAnnouncementTypeRef, classId, DateTime.Today,
//                    Context.PersonId ?? 0);

//                AnnouncementCompleteStorage.Add(new AnnouncementComplete
//                {
//                    AnnouncementId = announcement.Id,
//                    Complete = sourceAnnouncement.Complete,
//                    UserId = Context.PersonId.Value
//                });
//                announcement.Complete = sourceAnnouncement.Complete;
//                announcement.Subject = sourceAnnouncement.Subject;
//                announcement.WeightAddition = sourceAnnouncement.WeightAddition;
//                announcement.GradingStyle = sourceAnnouncement.GradingStyle;
//                announcement.WeightMultiplier = sourceAnnouncement.WeightMultiplier;
//                announcement.QnACount = sourceAnnouncement.QnACount;
//                announcement.OwnerAttachmentsCount = sourceAnnouncement.OwnerAttachmentsCount;
//                announcement.PrimaryTeacherRef = announcement.PrimaryTeacherRef;
//                announcement.IsOwner = announcement.PrimaryTeacherRef == Context.PersonId;
//                announcement.PrimaryTeacherName = sourceAnnouncement.PrimaryTeacherName;
//                announcement.SchoolRef = sourceAnnouncement.SchoolRef;
//                announcement.PrimaryTeacherGender = sourceAnnouncement.PrimaryTeacherGender;
//                announcement.Expires = sourceAnnouncement.Expires;
//                announcement.GradingStyle = sourceAnnouncement.GradingStyle;
//                announcement.State = sourceAnnouncement.State;
//                announcement.IsScored = sourceAnnouncement.IsScored;
//                announcement.Avg = sourceAnnouncement.Avg;
//                announcement.Title = sourceAnnouncement.Title;
//                announcement.Content = sourceAnnouncement.Content;
//                announcement.Owner = announcement.PrimaryTeacherRef.HasValue
//                    ? ServiceLocator.PersonService.GetPerson(announcement.PrimaryTeacherRef.Value)
//                    : null;
//                announcement.ApplicationCount = sourceAnnouncement.ApplicationCount;
//                announcement.AttachmentsCount = sourceAnnouncement.AttachmentsCount;
//                announcement.VisibleForStudent = sourceAnnouncement.VisibleForStudent;
//                announcement.MayBeDropped = sourceAnnouncement.MayBeDropped;
//                announcement.Order = sourceAnnouncement.Order;
//                announcement.ClassAnnouncementTypeName = sourceAnnouncement.ClassAnnouncementTypeName;
//                announcement.ChalkableAnnouncementType = sourceAnnouncement.ChalkableAnnouncementType;
//                announcement.ClassName = sourceAnnouncement.ClassName;
//                announcement.MaxScore = sourceAnnouncement.MaxScore;
//                // Added this copying 'cause of CHLK-3240
//                announcement.SisActivityId = sourceAnnouncement.Id;

//                var resAnnouncement = SubmitAnnouncement(classId, announcement);
//                var sourceStudentAnnouncements = ServiceLocator.StudentAnnouncementService.GetStudentAnnouncements(sourceAnnouncement.Id);
//                foreach (var sourceStudentAnnouncement in sourceStudentAnnouncements)
//                {
//                    ((DemoStudentAnnouncementService)ServiceLocator.StudentAnnouncementService).AddStudentAnnouncement(new StudentAnnouncement
//                    {
//                        ActivityId = resAnnouncement.SisActivityId.Value,
//                        StudentId = sourceStudentAnnouncement.StudentId,
//                        Absent = sourceStudentAnnouncement.Absent,
//                        AbsenceCategory = sourceStudentAnnouncement.AbsenceCategory,
//                        AlphaGradeId = sourceStudentAnnouncement.AlphaGradeId,
//                        AlternateScoreId = sourceStudentAnnouncement.AlternateScoreId,
//                        Comment = sourceStudentAnnouncement.Comment,
//                        ScoreDropped = sourceStudentAnnouncement.ScoreDropped,
//                        AlternateScore = sourceStudentAnnouncement.AlternateScore,
//                        Exempt = sourceStudentAnnouncement.Exempt,
//                        Incomplete = sourceStudentAnnouncement.Incomplete,
//                        Late = sourceStudentAnnouncement.Late,
//                        NumericScore = sourceStudentAnnouncement.NumericScore,
//                        OverMaxScore = sourceStudentAnnouncement.OverMaxScore,
//                        ScoreValue = sourceStudentAnnouncement.ScoreValue,
//                        Withdrawn = sourceStudentAnnouncement.Withdrawn,
//                        AnnouncementId = resAnnouncement.Id,
//                        AnnouncementTitle = sourceAnnouncement.Title,
//                        ExtraCredit = sourceStudentAnnouncement.ExtraCredit,
//                        IsWithdrawn = sourceStudentAnnouncement.IsWithdrawn
//                    });
//                }

//                resAnnouncement.AnnouncementAttachments = ((DemoAnnouncementAttachmentService)ServiceLocator.AnnouncementAttachmentService).DuplicateFrom(sourceAnnouncement.Id, resAnnouncement.Id); ;
//                var announcementApplications = new List<AnnouncementApplication>();

//                resAnnouncement.StudentAnnouncements = ServiceLocator.StudentAnnouncementService.GetStudentAnnouncements(resAnnouncement.Id);
//                resAnnouncement.AnnouncementApplications = announcementApplications;

//                foreach (var announcementQnAComplex in sourceAnnouncement.AnnouncementQnAs)
//                {
//                    ((DemoAnnouncementQnAService) ServiceLocator.AnnouncementQnAService).AddQnA(new AnnouncementQnAComplex
//                    {
//                        AnnouncementRef = resAnnouncement.Id,
//                        Answer = announcementQnAComplex.Answer,
//                        AnswererRef = announcementQnAComplex.AnswererRef,
//                        IsOwner = announcementQnAComplex.IsOwner,
//                        Question = announcementQnAComplex.Question,
//                        QuestionTime = announcementQnAComplex.QuestionTime,
//                        State = announcementQnAComplex.State,
//                        AskerRef = announcementQnAComplex.AskerRef,
//                        Asker = announcementQnAComplex.Asker,
//                        ClassRef = announcementQnAComplex.ClassRef,
//                        Answerer = announcementQnAComplex.Answerer,
//                        AnsweredTime = announcementQnAComplex.AnsweredTime
//                    });
//                }

//                var announcementsQnA = ServiceLocator.AnnouncementQnAService.GetAnnouncementQnAs(resAnnouncement.Id);
//                resAnnouncement.AnnouncementQnAs = announcementsQnA;

//                foreach (var announcementStandardDetails in sourceAnnouncement.AnnouncementStandards)
//                {
//                    ((DemoStandardService)ServiceLocator.StandardService).AddAnnouncementStandard(new AnnouncementStandardDetails
//                    {
//                        AnnouncementRef = resAnnouncement.Id,
//                        Standard = announcementStandardDetails.Standard,
//                        StandardRef = announcementStandardDetails.StandardRef
//                    });
//                }

//                resAnnouncement.AnnouncementStandards = ((DemoStandardService)ServiceLocator.StandardService).GetAnnouncementStandards(resAnnouncement.Id);
//                AnnouncementStorage.Update(resAnnouncement);
//            }
//        }

//        public void SubmitGroupsToAnnouncement(int announcementId, IList<int> groupsIds)
//        {
//            var ann = GetAnnouncementById(announcementId);
//            if (groupsIds != null)
//            {
//                var storage = new DemoAnnouncementRecipientStorage();
//                storage.DeleteByAnnouncementId(ann.Id);
//                storage.Add(groupsIds.Select(x => new AdminAnnouncementRecipient
//                    {
//                        AnnouncementRef = ann.Id,
//                        GroupRef = x
//                    }).ToList());
//            }
//        }

//        public DemoAnnouncementService GetTeacherAnnouncementService()
//        {
//            var teacherAnnouncementService = new DemoAnnouncementService(ServiceLocator, AnnouncementCompleteStorage,
//                AnnouncementStorage, AnnouncementRecipientStorage);
//            teacherAnnouncementService.SetAnnouncementProcessor(new TeacherAnnouncementProcessor(ServiceLocator));
//            return teacherAnnouncementService;
//        }

//        public void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
//        {
//            if (!Context.PersonId.HasValue)
//                throw new Exception("User local id doesn't have a valid value");
//            var annsResult = GetAnnouncements(new AnnouncementsQuery
//            {
//                ToDate = toDate,
//                PersonId = Context.PersonId,
//                Complete = !complete,
//                RoleId = Context.RoleId,
//            });
//            foreach (var ann in annsResult.Announcements)
//                SetComplete(ann.Id, complete);
//        }

//        public IEnumerable<Activity> GetActivitiesForClass(int classId)
//        {
//            return ActivityStorage.GetAll().Where(x => x.SectionId == classId);
//        }

//        public AnnouncementDetails CreateAdminAnnouncement(DateTime expiresDate)
//        {
//            throw new NotImplementedException();
//        }


//        public IList<AnnouncementComplex> GetAdminAnnouncements(bool? complete, IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? start, int? count, bool ownerOnly = false, int? studentId = null)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
