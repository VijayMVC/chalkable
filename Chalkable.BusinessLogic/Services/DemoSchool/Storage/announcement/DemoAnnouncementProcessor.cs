using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.announcement
{
    public interface IAnnouncementProcessor
    {
        IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query);
        Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId);
    }

    abstract class BaseAnnouncementProcessor : IAnnouncementProcessor
    {
        protected DemoStorageLocator StorageLocator { get; set; }

        protected BaseAnnouncementProcessor(DemoStorageLocator storageLocator)
        {
            StorageLocator = storageLocator;
        }

        public abstract IEnumerable<AnnouncementComplex> GetAnnouncements(
            IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query);

        public abstract Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements,
            int announcementId, int roleId, int userId);
    }

    class TeacherAnnouncementProcessor : BaseAnnouncementProcessor
    {
        public TeacherAnnouncementProcessor(DemoStorageLocator storageLocator) : base(storageLocator)
        {
        }

        public override IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            return announcements.OrderByDescending(x => x.Id).ToList();
        }

        public override Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, 
            int announcementId, int roleId, int userId)
        {
            var announcementRecipients =
                StorageLocator.AnnouncementRecipientStorage.GetAll()
                    .Where(x => x.ToAll || x.PersonRef == userId || x.RoleRef == roleId).Select(x => x.AnnouncementRef);

            return
                announcements.Where(x => x.Id == announcementId && x.PrimaryTeacherRef == userId || announcementRecipients.Contains(x.Id))
                    .Select(x => x)
                    .First();
        }
    }

    class StudentAnnouncementProcessor : BaseAnnouncementProcessor
    {
        public StudentAnnouncementProcessor(DemoStorageLocator storageLocator) : base(storageLocator)
        {
        }

        public override IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            return announcements.Where(x => x.VisibleForStudent).OrderByDescending(x => x.Id).ToList();
        }

        public override Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements,
            int announcementId, int roleId, int userId)
        {
            var classRefs = StorageLocator.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                PersonId = userId
            }).Select(x => x.ClassRef).ToList();


            var gradeLevelRefs = StorageLocator.StudentSchoolYearStorage.GetAll(userId).Select(x => x.GradeLevelRef).ToList();

            var annRecipients =
                StorageLocator.AnnouncementRecipientStorage.GetAll()
                    .Where(
                        x =>
                            x.ToAll || x.PersonRef == userId || x.RoleRef == roleId ||
                            x.GradeLevelRef != null && gradeLevelRefs.Contains(x.GradeLevelRef.Value))
                    .Select(x => x.AnnouncementRef);

            return announcements.Where(x => x.Id == announcementId && classRefs.Contains(x.ClassRef) || annRecipients.Contains(x.Id))
                  .Select(x => x)
                  .First();
        }
    }




    class AdminAnnouncementProcessor : BaseAnnouncementProcessor
    {
        public override IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            throw new NotImplementedException();
        }

        public override Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, 
            int announcementId, int roleId, int userId)
        {
            throw new NotImplementedException();
        }

        public AdminAnnouncementProcessor(DemoStorageLocator storageLocator) : base(storageLocator)
        {
        }
    }


}
