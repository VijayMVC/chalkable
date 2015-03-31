using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoApplicationSchoolService : DemoSchoolServiceBase, IApplicationSchoolService
    {
        private DemoAnnouncementApplicationStorage AnnouncementApplicationStorage { get; set; }
        public DemoApplicationSchoolService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            AnnouncementApplicationStorage = new DemoAnnouncementApplicationStorage();
        }

        public IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId)
        {
            var res = new List<int>();
            if (announcementAppId.HasValue)
            {

                var anDa = StorageLocator.AnnouncementStorage.GetTeacherStorage();
                var announcementApplication = GetAnnouncementApplication(announcementAppId.Value);
                var ann = anDa.GetById(announcementApplication.AnnouncementRef);
                var csp = StorageLocator.ClassPersonStorage.GetClassPersons(ann.ClassRef);
                res.AddRange(csp.Select(x => x.PersonRef));
                var c = StorageLocator.ClassStorage.GetById(ann.ClassRef);
                if (Context.PersonId != c.PrimaryTeacherRef)
                    if (c.PrimaryTeacherRef.HasValue) res.Add(c.PrimaryTeacherRef.Value);
            }
            else
            {
                var inst = StorageLocator.ApplicationInstallStorage.GetAll(appId, true);
                res.AddRange(inst.Select(x => x.PersonRef));
            }
            return res;
        }

        public AnnouncementApplication AddToAnnouncement(int announcementId, Guid applicationId)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (!ApplicationSecurity.CanAddToAnnouncement(app, ann, Context))
                throw new ChalkableSecurityException();
            var aa = new AnnouncementApplication
            {
                AnnouncementRef = announcementId,
                ApplicationRef = applicationId,
                Active = false,
                Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
            };
            StorageLocator.AnnouncementApplicationStorage.Add(aa);
            aa = StorageLocator.AnnouncementApplicationStorage.GetAll(announcementId, applicationId, false).OrderByDescending(x => x.Id).First();
            return aa;
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            return AnnouncementApplicationStorage.GetAnnouncementApplication(announcementAppId);
        }

        public void AttachAppToAnnouncement(int announcementAppId)
        {
            AnnouncementApplicationStorage.AttachAppToAnnouncement(announcementAppId);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            return AnnouncementApplicationStorage.GetAnnouncementApplicationsByAnnId(announcementId, onlyActive);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false)
        {
            return AnnouncementApplicationStorage.GetAnnouncementApplicationsByPerson(personId, onlyActive);
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId)
        {
            return AnnouncementApplicationStorage.RemoveFromAnnouncement(announcementAppId);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false)
        {
            return AnnouncementApplicationStorage.GetAnnouncementApplicationsByAnnIds(announcementIds,
                onlyActive);
            
        }
    }
}