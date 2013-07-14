using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IApplicationSchoolService
    {
        IList<int> GetAssignedUserIds(int appId, int? announcementAppId);
        AnnouncementApplication AddToAnnouncement(int announcementId, int applicationId);
        AnnouncementApplication GetAnnouncementApplication(int announcementAppId);
        void AttachAppToAnnouncement(int announcementAppId);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int schoolPersonId, bool onlyActive = false);
        Announcement RemoveFromAnnouncement(int announcementAppId);

    }

    public class ApplicationSchoolService : SchoolServiceBase, IApplicationSchoolService
    {
        public ApplicationSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<int> GetAssignedUserIds(int appId, int? announcementAppId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementApplication AddToAnnouncement(int announcementId, int applicationId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            throw new System.NotImplementedException();
        }

        public void AttachAppToAnnouncement(int announcementAppId)
        {
            throw new System.NotImplementedException();
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            throw new System.NotImplementedException();
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int schoolPersonId, bool onlyActive = false)
        {
            throw new System.NotImplementedException();
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId)
        {
            throw new System.NotImplementedException();
        }
    }
}