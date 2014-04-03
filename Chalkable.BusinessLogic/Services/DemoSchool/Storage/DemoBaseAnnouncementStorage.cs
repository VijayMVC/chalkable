using System;
using System.Collections.Generic;
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
    }


    public class DemoBaseAnnouncementStorage:IDemoAnnouncementStorage
    {
        public DemoBaseAnnouncementStorage(DemoStorage storage)
        {
        }

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new NotImplementedException();
        }

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int i)
        {
            throw new NotImplementedException();
        }

        public AnnouncementDetails GetDetails(int announcementId, int value, int id)
        {
            throw new NotImplementedException();
        }

        public Announcement GetById(int announcementId)
        {
            throw new NotImplementedException();
        }

        public void Delete(int? announcementId, object o, object o1, object o2, object o3)
        {
            throw new NotImplementedException();
        }

        public void Update(Announcement ann)
        {
            throw new NotImplementedException();
        }

        public Announcement GetAnnouncement(int announcementId, int roleId, int value)
        {
            throw new NotImplementedException();
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

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int i)
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

        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int i)
        {
            throw new NotImplementedException();
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

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime nowLocalDate, int i)
        {
            throw new NotImplementedException();
        }
    }
}
