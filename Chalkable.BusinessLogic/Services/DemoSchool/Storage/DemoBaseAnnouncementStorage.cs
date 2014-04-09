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
        void Delete(int? announcementId, object o, object o1, object o2, object o3);
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
        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new NotImplementedException();
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


            var annDetails = new AnnouncementDetails
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

            return annDetails;

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

        public AnnouncementDetails GetDetails(int announcementId, int value, int id)
        {
            throw new NotImplementedException();
        }

        public new Announcement GetById(int announcementId)
        {
            return data.ContainsKey(announcementId) ? data[announcementId] : null;
        }

        public void Delete(int? announcementId, object o, object o1, object o2, object o3)
        {
            throw new NotImplementedException();
        }

        public void Update(Announcement ann)
        {
            throw new NotImplementedException();
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

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int i)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string s)
        {
            throw new NotImplementedException();
        }

        public void ReorderAnnouncements(int id, int value, int personRef, int recipientId)
        {
            throw new NotImplementedException();
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

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new System.NotImplementedException();
        }

    
    }
}
