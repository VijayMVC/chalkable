using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.SqlAzure.ImportExport;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public interface IDemoAnnouncementStorage
    {
        AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        AnnouncementDetails Create(int classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, DateTime? expiresDateTime = null);
        AnnouncementDetails GetDetails(int announcementId, int value, int id);
        Announcement GetById(int announcementId);
        void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state);
        void Update(Announcement ann);
        Announcement GetAnnouncement(int announcementId, int roleId, int value);
        Announcement GetLastDraft(int i);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId, int userId);
        IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int i);
        bool Exists(string s);
        void ReorderAnnouncements(int id, int value, int recipientId);
        bool CanAddStandard(int announcementId);
        bool IsEmpty();
        Dictionary<int, AnnouncementComplex> GetData();
        bool Exists(int id);
        void Setup();
    }


    public abstract class DemoBaseAnnouncementStorage : BaseDemoIntStorage<AnnouncementComplex>, IDemoAnnouncementStorage
    {
        protected DemoBaseAnnouncementStorage(DemoStorage storage):base(storage, x => x.Id)
        {
            if (storage.AnnouncementStorage != null && !storage.AnnouncementStorage.IsEmpty())
            {
                var oldData = storage.AnnouncementStorage.GetData();
                foreach (var item in oldData)
                {
                    if (!data.ContainsKey(item.Key)) 
                    {
                        data.Add(item.Key, item.Value);    
                    }
                    else
                    {
                        data[item.Key] = item.Value;
                    }
                }
                Index = oldData.Count;
            }
            storage.UpdateAnnouncementStorage(this);
        }


        public bool CanAddStandard(int announcementId)
        {
            var announcement = GetById(announcementId);
            var cls = Storage.ClassStorage.GetById(announcement.ClassRef);  
            return Storage.ClasStandardStorage.GetAll().Count(x => x.ClassRef == cls.Id || x.ClassRef == cls.CourseRef) > 0;
        }

        public bool Exists(int id)
        {
            return data.Count(x => x.Value.SisActivityId == id) > 0;
        }

        public virtual AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {

            var announcements = data.Select(x => x.Value);

            if (query.Id.HasValue)
                announcements = announcements.Where(x => x.Id == query.Id);

            if (query.ClassId.HasValue)
                announcements = announcements.Where(x => x.ClassRef == query.ClassId);

            if (query.MarkingPeriodId.HasValue)
            {
                var mp = Storage.MarkingPeriodStorage.GetById(query.MarkingPeriodId.Value);
                announcements = announcements.Where(x => x.Expires >= mp.StartDate && x.Expires <= mp.EndDate);
            }

            if (query.FromDate.HasValue)
                announcements = announcements.Where(x => x.Expires >= query.FromDate);

            if (query.ToDate.HasValue)
                announcements = announcements.Where(x => x.Expires <= query.ToDate);
            if (query.Complete.HasValue)
                announcements = announcements.Where(x => x.Complete == query.Complete);
            if (query.OwnedOnly)
                announcements = announcements.Where(x => x.PrimaryTeacherRef == query.PersonId);

            if (query.GradeLevelIds != null)
                announcements = announcements.Where(x => query.GradeLevelIds.Contains(x.Id));

            if (query.SisActivitiesIds != null)
                announcements = announcements.Where(x => query.SisActivitiesIds.Contains(x.Id));

            if (query.Start > 0)
                announcements = announcements.Skip(query.Start);
            if (query.Count > 0)
                announcements = announcements.Take(query.Count);

            return new AnnouncementQueryResult
            {
                Announcements = announcements.ToList(),
                Query = query,
                SourceCount = data.Count
            };
        }


        private AnnouncementDetails ConvertToDetails(AnnouncementComplex announcement)
        {
            var announcementAttachments = Storage.AnnouncementAttachmentStorage.GetAll(announcement.Id);
            
            var announcementApplications= new List<AnnouncementApplication>();
            var announcementsQnA = Storage.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
            {
                AnnouncementId = announcement.Id
            }).AnnouncementQnAs;
            var announcementStandards =
                Storage.AnnouncementStandardStorage.GetAll(announcement.Id).Select(x => new AnnouncementStandardDetails
                {
                    AnnouncementRef = x.AnnouncementRef,
                    StandardRef = x.StandardRef,
                    Standard = Storage.StandardStorage.GetById(x.StandardRef)
                }).ToList();

            return new AnnouncementDetails
            {
                Id = announcement.Id,
                SisActivityId = announcement.SisActivityId,
                Complete = announcement.Complete,
                Subject = announcement.Subject,
                StudentsCount = announcement.StudentsCount,
                WeightAddition = announcement.WeightAddition,
                WeightMultiplier = announcement.WeightMultiplier,
                QnACount = announcement.QnACount,
                OwnerAttachmentsCount = announcement.OwnerAttachmentsCount,
                PrimaryTeacherRef = announcement.PrimaryTeacherRef,
                IsOwner = announcement.IsOwner,
                PrimaryTeacherName = announcement.PrimaryTeacherName,
                SchoolRef = announcement.SchoolRef,
                ClassRef = announcement.ClassRef,
                PrimaryTeacherGender = announcement.PrimaryTeacherGender,
                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
                Created = announcement.Created,
                Expires = announcement.Expires,
                GradingStyle = announcement.GradingStyle,
                State = announcement.State,
                IsScored = announcement.IsScored,
                Avg = announcement.Avg,
                Title = announcement.Title,
                AnnouncementAttachments = announcementAttachments,
                AnnouncementApplications = announcementApplications,
                AnnouncementQnAs = announcementsQnA,
                AnnouncementStandards = announcementStandards,
                Owner = Storage.PersonStorage.GetById(announcement.PrimaryTeacherRef),
                ApplicationCount = announcement.ApplicationCount,
                AttachmentsCount = announcement.AttachmentsCount,
                StudentAnnouncements = Storage.StudentAnnouncementStorage.GetAll(announcement.Id),
                VisibleForStudent = announcement.VisibleForStudent,
                Order = announcement.Order,
                ClassAnnouncementTypeName = announcement.ClassAnnouncementTypeName,
                ChalkableAnnouncementType = announcement.ChalkableAnnouncementType,
                ClassName = announcement.ClassName,
                GradeLevelId = announcement.GradeLevelId
            };
        }

        public AnnouncementDetails Create(int classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, DateTime? expiresDateTime = null)
        {
            var annId = GetNextFreeId();
            var person = Storage.PersonStorage.GetById(userId);
            var gradeLevelRef = Storage.ClassStorage.GetById(classId).GradeLevelRef;

            //todo: create admin announcements if it's admin

            var persons = Storage.PersonStorage.GetPersons(new PersonQuery
            {
                ClassId = classId,
                RoleId = CoreRoles.STUDENT_ROLE.Id
            }).Persons.Select(x => x.Id).ToList();


            foreach (var personId in persons)
            {
                Storage.StudentAnnouncementStorage.Add(new StudentAnnouncement
                {
                    AnnouncementId = annId,
                    ActivityId = annId,
                    StudentId = personId
                });

                Storage.StiActivityScoreStorage.Add(new Score
                {
                    ActivityId = annId,
                    StudentId = personId
                });
            }

            var announcement = new AnnouncementComplex
            {
                ClassAnnouncementTypeName = Storage.ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId).Name,
                ChalkableAnnouncementType = classAnnouncementTypeId,
                PrimaryTeacherName = person.FullName,
                ClassName = Storage.ClassStorage.GetById(classId).Name,
                GradeLevelId = gradeLevelRef,
                PrimaryTeacherGender = person.Gender,
                IsScored = false,
                Id = annId,
                PrimaryTeacherRef = userId,
                IsOwner = Storage.Context.UserLocalId == userId,
                ClassRef = classId,
                ClassAnnouncementTypeRef = classAnnouncementTypeId,
                Created = nowLocalDate,
                Expires = expiresDateTime.HasValue ? expiresDateTime.Value : DateTime.MinValue,
                State = AnnouncementState.Draft,
                GradingStyle = GradingStyleEnum.Numeric100,
                SchoolRef = Storage.Context.SchoolLocalId.Value,
                QnACount = 0,
                StudentsCount = persons.Count,
                Order = 0,
                AttachmentsCount = 0,
                ApplicationCount = 0,
                OwnerAttachmentsCount = 0,
                StudentsCountWithAttachments = 0
            };
            
            data[announcement.Id] = announcement;
            return ConvertToDetails(announcement);
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

        public new Announcement GetById(int announcementId)
        {
            return data.ContainsKey(announcementId) ? data[announcementId] : null;
        }

        public override void Setup()
        {
            var ann1 = Create(0, DemoSchoolConstants.AlgebraClassId, DateTime.Today, 1195, DateTime.Now.AddDays(1));
            var ann2 = Create(1, DemoSchoolConstants.AlgebraClassId, DateTime.Today, 1195, DateTime.Now.AddDays(1));

            var ann3 = Create(0, DemoSchoolConstants.GeometryClassId, DateTime.Today, 1195, DateTime.Now.AddDays(1));
            var ann4 = Create(1, DemoSchoolConstants.GeometryClassId, DateTime.Today, 1195, DateTime.Now.AddDays(1));

            var announcements = new List<AnnouncementDetails>() {ann1, ann2, ann3, ann4};

            foreach (var announcementDetail in announcements)
            {
               
                var activity = new Activity();
                MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, announcementDetail);
                activity = Storage.StiActivityStorage.CreateActivity(announcementDetail.ClassRef, activity);
                if (Exists(activity.Id))
                    throw new ChalkableException("Announcement with such activityId already exists");
                data[announcementDetail.Id].SisActivityId = activity.Id;
            }

            
        }

        public void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state)
        {
            var announcementsToDelete = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = userId,
                Id = announcementId,
                ClassId = classId
            }).Announcements.Where(x => x.State == state);
            if (announcementType.HasValue)
                announcementsToDelete = announcementsToDelete.Where(x => x.ClassAnnouncementTypeRef == announcementType);
            Delete(announcementsToDelete.Select(x => x.Id).ToList());
        }

        public void Update(Announcement ann)
        {
            if (data.ContainsKey(ann.Id))
            {
                data[ann.Id].Content = ann.Content;
                data[ann.Id].Created = ann.Created;
                data[ann.Id].Expires = ann.Expires;
                data[ann.Id].ClassAnnouncementTypeRef = ann.ClassAnnouncementTypeRef;
                data[ann.Id].State = ann.State;
                data[ann.Id].GradingStyle = ann.GradingStyle;
                data[ann.Id].Subject = ann.Subject;
                data[ann.Id].ClassRef = ann.ClassRef;
                data[ann.Id].Order = ann.Order;
                data[ann.Id].Dropped = ann.Dropped;
                data[ann.Id].MayBeDropped = ann.MayBeDropped;
                data[ann.Id].VisibleForStudent = ann.VisibleForStudent;
                data[ann.Id].SchoolRef = ann.SchoolRef;
                data[ann.Id].Title = ann.Title;
                data[ann.Id].SisActivityId = ann.SisActivityId;
                data[ann.Id].MaxScore = ann.MaxScore;
                data[ann.Id].WeightAddition = ann.WeightAddition;
                data[ann.Id].WeightMultiplier = ann.WeightMultiplier;
                data[ann.Id].MayBeDropped = ann.MayBeDropped;
                data[ann.Id].MayBeExempt = ann.MayBeExempt;
                data[ann.Id].IsScored = ann.IsScored;
            }

        }

        public abstract Announcement GetAnnouncement(int announcementId, int roleId, int userId);

        public Announcement GetLastDraft(int userId)
        {
            return GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = userId,
            }).Announcements.Where(x => x.State == AnnouncementState.Draft).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId, int userId)
        {
            var annRecipients = Storage.AnnouncementRecipientStorage.GetList(announcementId);
            var result = new List<Person>();
            foreach (var announcementRecipient in annRecipients)
            {
                if (announcementRecipient.PersonRef.HasValue)
                    result.Add(Storage.PersonStorage.GetPerson(new PersonQuery
                    {
                        PersonId = announcementRecipient.PersonRef
                    }));
            }
            return result;
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count)
        {
            var announcements = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = personId,
                ClassId = classId,
            }).Announcements.ToList();
            
            return announcements.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType).Take(count).Select(x => x.Content).ToList();

        }

        public bool Exists(string s)
        {
            return data.Count(x => x.Value.Title == s) > 0;
        }

        public void ReorderAnnouncements(int schoolYearId, int classAnnouncementTypeId, int recipientClassId)
        {
        }
    }


    public class DemoAnnouncementForAdminStorage:DemoBaseAnnouncementStorage
    {
        public DemoAnnouncementForAdminStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new System.NotImplementedException();
        }

        public override Announcement GetAnnouncement(int announcementId, int roleId, int userId)
        {
            throw new NotImplementedException();
        }
    }

    public class DemoAnnouncementForStudentStorage : DemoBaseAnnouncementStorage
    {
        public DemoAnnouncementForStudentStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public override Announcement GetAnnouncement(int announcementId, int roleId, int userId)
        {

            var classRefs = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                PersonId = userId
            }).Select(x => x.ClassRef).ToList();


            var gradeLevelRefs = Storage.StudentSchoolYearStorage.GetAll(userId).Select(x => x.GradeLevelRef).ToList();

            var annRecipients =
                Storage.AnnouncementRecipientStorage.GetAll()
                    .Where(
                        x =>
                            x.ToAll || x.PersonRef == userId || x.RoleRef == roleId ||
                            x.GradeLevelRef != null && gradeLevelRefs.Contains(x.GradeLevelRef.Value))
                    .Select(x => x.AnnouncementRef);

            return
                data.Where(x => x.Value.Id == announcementId && classRefs.Contains(x.Value.ClassRef) || annRecipients.Contains(x.Value.Id))
                    .Select(x => x.Value)
                    .First();
        }
    }

    public class DemoAnnouncementForTeacherStorage : DemoBaseAnnouncementStorage
    {
        public DemoAnnouncementForTeacherStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {

            var result = base.GetAnnouncements(query);
            result.Announcements = result.Announcements.OrderByDescending(x => x.Id).ToList();
            return result;
            /*
	             
	                and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PersonRef = @personId 
		                or (ClassAnnouncementTypeRef is null 
			                and exists(select AnnouncementRecipient.Id from AnnouncementRecipient
						                where AnnouncementRef = vwAnnouncement.Id  and (ToAll = 1 or PersonRef = @personId 
							                or (RoleRef = @roleId and (@roleId <> 2 or GradeLevelRef is null or GradeLevelRef in (select Id from @gradeLevelsT))))
						                )
			                )
		                )			
        }
         */    
        }

        public override Announcement GetAnnouncement(int announcementId, int roleId, int userId)
        {
            var announcementRecipients =
                Storage.AnnouncementRecipientStorage.GetAll()
                    .Where(x => x.ToAll || x.PersonRef == userId || x.RoleRef == roleId).Select(x => x.AnnouncementRef);

            return
                data.Where(x => x.Value.Id == announcementId && x.Value.PrimaryTeacherRef == userId || announcementRecipients.Contains(x.Value.Id))
                    .Select(x => x.Value)
                    .First();
        }
    }
}
