using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;

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
    }


    public class DemoBaseAnnouncementStorage : BaseDemoStorage<int, AnnouncementComplex>, IDemoAnnouncementStorage
    {
        public DemoBaseAnnouncementStorage(DemoStorage storage):base(storage)
        {
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
            if (query.PersonId.HasValue)
                announcements = announcements.Where(x => x.PersonRef == query.PersonId);


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
                SourceCount = announcements.Count()
            };
        }


        private AnnouncementDetails ConvertToDetails(AnnouncementComplex announcement)
        {
            return new AnnouncementDetails
            {
                Id = announcement.Id,
                PersonRef = announcement.PersonRef,
                //PersonName = announcement.PersonName,
                SchoolRef = announcement.SchoolRef,
                ClassRef = announcement.ClassRef,
                //Gender = announcement.Gender,
                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
                Created = announcement.Created,
                Expires = announcement.Expires,
                GradingStyle = announcement.GradingStyle,
                State = announcement.State,
                StudentAnnouncements = new List<StudentAnnouncementDetails>(),
                AnnouncementApplications = new List<AnnouncementApplication>(),
                AnnouncementAttachments = new List<AnnouncementAttachment>(),
                AnnouncementReminders = new List<AnnouncementReminder>(),
                AnnouncementQnAs = new List<AnnouncementQnAComplex>(),
                AnnouncementStandards = new List<AnnouncementStandardDetails>(),
                
                Owner = Storage.PersonStorage.GetById(announcement.PersonRef)
                //ApplicationCount = announcement.ApplicationCount,
                //AttachmentsCount = announcement.AttachmentsCount,

            };
        }

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int userId)
        {

        
             /*   public string ClassAnnouncementTypeName { get; set; }
        public int? ChalkableAnnouncementType { get; set; }
        public string PersonName { get; set; }
        public string Gender { get; set; }

        public string ClassName { get; set; }
        public int? GradeLevelId { get; set; }
        
        
        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int? Avg { get; set; }
        public int ApplicationCount { get; set; }

        public bool IsOwner { get; set; }
        public int? RecipientDataPersonId { get; set; }
        public bool? Starred { get; set; }
             */

            var announcement = new AnnouncementComplex
            {
                Id = GetNextFreeId(),
                PersonRef = userId,
                ClassRef = classId,
                ClassAnnouncementTypeRef = classAnnouncementTypeId,
                Created = nowLocalDate,
                Expires = DateTime.MinValue,
                State = AnnouncementState.Draft,
                GradingStyle = GradingStyleEnum.Numeric100,
                SchoolRef = Storage.Context.SchoolLocalId.Value,
            };
            data[announcement.Id] = announcement;




            return ConvertToDetails(announcement);

            /*
             * public int? FinalGradeStatus { get; set; }
        
        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }
        public IList<AnnouncementApplication> AnnouncementApplications { get; set; }
        public IList<AnnouncementAttachment> AnnouncementAttachments { get; set; }
        public IList<AnnouncementReminder> AnnouncementReminders { get; set; }
        public IList<AnnouncementQnAComplex> AnnouncementQnAs { get; set; } 
        public Person Owner { get; set; }
        public IList<AnnouncementStandardDetails> AnnouncementStandards { get; set; } 
             */

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
            }

                /*  
    
        public int? SisActivityId { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool MayBeDropped { get; set; }
        [NotDbFieldAttr]
        public bool MayBeExempt { get; set; }
        [NotDbFieldAttr]
        public bool IsScored { get; set; }
                 */
                
        }

        public Announcement GetAnnouncement(int announcementId, int roleId, int userId)
        {
            //todo filter by role id
            return
                data.Where(x => x.Value.Id == announcementId && x.Value.PersonRef == userId)
                    .Select(x => x.Value)
                    .First();
        }

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

        public void ReorderAnnouncements(int schoolYearId, int ClassAnnouncementTypeId, int personRef, int recipientClassId)
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

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new System.NotImplementedException();
        }
    }

    public class DemoAnnouncementForStudentStorage : DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
    {
        public DemoAnnouncementForStudentStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new System.NotImplementedException();
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

            /*
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

    
    }
}
