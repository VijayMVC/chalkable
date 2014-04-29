using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public interface IDemoAnnouncementStorage
    {
        AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int i);
        AnnouncementDetails GetDetails(int announcementId, int value, int id);
        Announcement GetById(int announcementId);
        void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state);
        void Update(Announcement ann);
        Announcement GetAnnouncement(int announcementId, int roleId, int value);
        Announcement GetLastDraft(int i);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId, int i);
        IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int i);
        bool Exists(string s);
        void ReorderAnnouncements(int id, int value, int personRef, int recipientId);
        bool CanAddStandard(int announcementId);
        bool IsEmpty();
        Dictionary<int, AnnouncementComplex> GetData();
        bool Exists(int id);
    }


    public abstract class DemoBaseAnnouncementStorage : BaseDemoStorage<int, AnnouncementComplex>, IDemoAnnouncementStorage
    {
        //todo pass announcement dictionary 
        protected DemoBaseAnnouncementStorage(DemoStorage storage):base(storage)
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
                index = oldData.Count;
            }
            storage.UpdateAnnouncementStorage(this);
        }


        public bool CanAddStandard(int announcementId)
        {
            var announcement = GetById(announcementId);

            var exists = false;

            if (announcement.ClassRef.HasValue)
            {
                var cls = Storage.ClassStorage.GetById(announcement.ClassRef.Value);
                
                exists = Storage.ClasStandardStorage.GetAll().Count(x => x.ClassRef == cls.Id || x.ClassRef == cls.CourseRef) > 0;
            }
            return exists;
        }

        public bool Exists(int id)
        {
            return data.Count(x => x.Value.SisActivityId == id) > 0;
        }

        public virtual AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            /*
        public int? RoleId { get; set; }
        public int? ClassId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? Now { get; set; }
        public bool StarredOnly { get; set; }
        public bool OwnedOnly { get; set; }
        public bool GradedOnly { get; set; }
        public bool AllSchoolItems { get; set; }

        public IList<int> GradeLevelIds { get; set; }
        public IList<int> SisActivitiesIds { get; set; } 

             */
            var announcements = data.Select(x => x.Value);


            if (query.Id.HasValue)
                announcements = announcements.Where(x => x.Id == query.Id);


            //person id is creator id
            //if (query.PersonId.HasValue)
                //announcements = announcements.Where(x => x.PersonRef == query.PersonId);


            if (query.Start > 0)
                announcements = announcements.Skip(query.Start);
            if (query.Count > 0)
                announcements = announcements.Take(query.Count);

            //
            //if (query.RoleId.HasValue)
            //    announcements = announcements.Where(x => x.R)
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
            var announcementReminders = Storage.AnnouncementReminderStorage.GetList(announcement.Id, announcement.PersonRef);


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
                Starred = announcement.Starred,
                Subject = announcement.Subject,
                StudentsCount = announcement.StudentsCount,
                WeightAddition = announcement.WeightAddition,
                WeightMultiplier = announcement.WeightMultiplier,
                QnACount = announcement.QnACount,
                OwnerAttachmentsCount = announcement.OwnerAttachmentsCount,
                PersonRef = announcement.PersonRef,
                PersonName = announcement.PersonName,
                SchoolRef = announcement.SchoolRef,
                ClassRef = announcement.ClassRef,
                Gender = announcement.Gender,
                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
                Created = announcement.Created,
                Expires = announcement.Expires,
                GradingStyle = announcement.GradingStyle,
                State = announcement.State,
                IsScored = announcement.IsScored,
                Avg = announcement.Avg,
                Title = announcement.Title,
                AnnouncementAttachments = announcementAttachments,
                AnnouncementReminders = announcementReminders,
                AnnouncementApplications = announcementApplications,
                AnnouncementQnAs = announcementsQnA,
                AnnouncementStandards = announcementStandards,
                Owner = Storage.PersonStorage.GetById(announcement.PersonRef),
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

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int userId)
        {

   

            var annId = GetNextFreeId();


            var person = Storage.PersonStorage.GetById(userId);
            var gradeLevelRef = classId.HasValue ? Storage.ClassStorage.GetById(classId.Value).GradeLevelRef : (int?)null;

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
                ClassAnnouncementTypeName = classAnnouncementTypeId.HasValue ? Storage.ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId.Value).Name : "",
                ChalkableAnnouncementType = classAnnouncementTypeId,
                PersonName = person.FullName,
                ClassName = classId.HasValue ? Storage.ClassStorage.GetById(classId.Value).Name : "",
                GradeLevelId = gradeLevelRef,
                Gender = person.Gender,
                IsScored = false,
                Id = annId,
                PersonRef = userId,
                ClassRef = classId,
                ClassAnnouncementTypeRef = classAnnouncementTypeId,
                Created = nowLocalDate,
                Expires = DateTime.MinValue,
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
            throw new NotImplementedException();
        }

        public void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state)
        {
            var announcementsToDelete = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = userId,
                Id = announcementId,
                ClassId = classId
                //announcement type,

            }).Announcements.Where(x => x.State == state).Select(x => x.Id).ToList();
            Delete(announcementsToDelete);
        }

        public void Update(Announcement ann)
        {
            if (data.ContainsKey(ann.Id))
            {
                data[ann.Id].PersonRef = ann.PersonRef;
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

        public Announcement GetLastDraft(int i)
        {
            throw new NotImplementedException();
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId, int i)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count)
        {

            // {Announcement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType}

            var announcements = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = personId,
                ClassId = classId,
                Count = count
            }).Announcements.ToList();
            
            return announcements.Select(x => x.Content).ToList();

        }

        public bool Exists(string s)
        {
            return data.Count(x => x.Value.Title == s) > 0;
        }

        public void ReorderAnnouncements(int schoolYearId, int classAnnouncementTypeId, int personRef, int recipientClassId)
        {


            /*
            var announcements = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = personRef,
                
            })*/
            /*
             * with AnnView as
               (
                select a.Id, Row_Number() over(order by a.Expires, a.[Created]) as [Order]  
                from Announcement a
                join Class c on c.Id = a.ClassRef
				where c.SchoolYearRef = @schoolYearId and a.ClassAnnouncementTypeRef = @classAnnType 
                      and a.PersonRef = @ownerId and a.ClassRef = @classId
               )
update Announcement
set [Order] = AnnView.[Order]
from AnnView 
where AnnView.Id = Announcement.Id
select  1
             */


        }
    }


    public class DemoAnnouncementForAdminStorage:DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
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

    public class DemoAnnouncementForStudentStorage : DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
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
                data.Where(x => x.Value.Id == announcementId && classRefs.Contains(x.Value.ClassRef.Value) || annRecipients.Contains(x.Value.Id))
                    .Select(x => x.Value)
                    .First();
        }
    }

    public class DemoAnnouncementForTeacherStorage : DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
    {
        public DemoAnnouncementForTeacherStorage(DemoStorage storage)
            : base(storage)
        {
        }



        /*public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {

            Select 
	                vwAnnouncement.*,
	                cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
	                ard.PersonRef as RecipientDataPersonId,
	                ard.Starred as Starred,
	                ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
	                --@starredCount as StarredCount,
	 	                @allCount as AllCount
                from 
	                vwAnnouncement	
	                left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			                on vwAnnouncement.Id = ard.AnnouncementRef
                where
	                (@id is not null  or [State] = 1) and
	                (@id is null or vwAnnouncement.Id = @id)
	                and (@classId is null or ClassRef = @classId)
	                and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PersonRef = @personId 
		                or (ClassAnnouncementTypeRef is null 
			                and exists(select AnnouncementRecipient.Id from AnnouncementRecipient
						                where AnnouncementRef = vwAnnouncement.Id  and (ToAll = 1 or PersonRef = @personId 
							                or (RoleRef = @roleId and (@roleId <> 2 or GradeLevelRef is null or GradeLevelRef in (select Id from @gradeLevelsT))))
						                )
			                )
		                )			
	                and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	                and (@staredOnly = 0 or Starred = 1)
	                and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	                and (@fromDate is null or Expires >= @fromDate)
	                and (@toDate is null or Expires <= @toDate)
	                and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	                and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
	
                order by Created desc				
                OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
                             *  var parameters = new Dictionary<string, object>
                {
                    {GRADED_ONLY_PARAM, query.GradedOnly},
                    {ALL_SCHOOL_ITEMS_PARAM, query.AllSchoolItems},
                    {"@sisActivitiesIds", query.SisActivitiesIds != null ? query.SisActivitiesIds.Select(x => x.ToString()).JoinString(",") : null}
                };
            return GetAnnouncementsComplex(GET_TEACHER_ANNOUNCEMENTS, parameters, query);
        }
             
            throw new System.NotImplementedException();
        }*/


        public override Announcement GetAnnouncement(int announcementId, int roleId, int userId)
        {
            var announcementRecipients =
                Storage.AnnouncementRecipientStorage.GetAll()
                    .Where(x => x.ToAll || x.PersonRef == userId || x.RoleRef == roleId).Select(x => x.AnnouncementRef);

            return
                data.Where(x => x.Value.Id == announcementId && x.Value.PersonRef == userId || announcementRecipients.Contains(x.Value.Id))
                    .Select(x => x.Value)
                    .First();
        }
    }
}
