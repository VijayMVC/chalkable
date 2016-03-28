using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public interface IAnnouncementProcessor
    {
        IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query);
        Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId);
    }

    abstract class BaseAnnouncementProcessor : IAnnouncementProcessor
    {
        protected IServiceLocatorSchool ServiceLocator { get; set; }

        protected BaseAnnouncementProcessor(IServiceLocatorSchool serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        public abstract IEnumerable<AnnouncementComplex> GetAnnouncements(
            IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query);

        public abstract Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements,
            int announcementId, int roleId, int userId);
    }

    class TeacherAnnouncementProcessor : BaseAnnouncementProcessor
    {
        public TeacherAnnouncementProcessor(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public override IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            return announcements.OrderByDescending(x => x.Id).ToList();
        }

        public override Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, 
            int announcementId, int roleId, int userId)
        {
            //TODO it later
            throw new NotImplementedException();
            //var announcementRecipients =
            //    ((DemoAnnouncementService) ServiceLocator.AnnouncementService).GetAnnouncementRecipients(
            //        announcementId, roleId, userId, true).Select(x => x.AnnouncementRef);
            //return
            //    announcements.Where(x => x.Id == announcementId && x.PrimaryTeacherRef == userId || announcementRecipients.Contains(x.Id))
            //        .Select(x => x)
            //        .First();
        }
    }

    class StudentAnnouncementProcessor : BaseAnnouncementProcessor
    {
        public StudentAnnouncementProcessor(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public override IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            throw new NotImplementedException();
            //return announcements.Where(x => x.VisibleForStudent).OrderByDescending(x => x.Id).ToList();
        }

        public override Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements,
            int announcementId, int roleId, int userId)
        {
            //TODO : it later
            throw new NotImplementedException();
            //var classRefs = ServiceLocator.ClassService.GetClassPersons(userId, null).Select(x => x.ClassRef).ToList();
            //var gradeLevelRefs = ((DemoSchoolYearService)ServiceLocator.SchoolYearService)
            //    .GetStudentAssignments(userId).Select(x => x.GradeLevelRef).ToList();

            //var annRecipients =
            //    ((DemoAnnouncementService) ServiceLocator.AnnouncementService).GetAnnouncementRecipients(
            //        announcementId, roleId, userId, true)
            //       // .Where(x => x.GradeLevelRef != null && gradeLevelRefs.Contains(x.GradeLevelRef.Value))
            //        .Select(x => x.AnnouncementRef);

            //return announcements.Where(x => x.Id == announcementId && classRefs.Contains(x.ClassRef.Value) || annRecipients.Contains(x.Id))
            //      .Select(x => x)
            //      .First();
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

        public AdminAnnouncementProcessor(IServiceLocatorSchool locator) : base(locator)
        {
        }
    }


}
