﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoApplicationSchoolService : DemoSchoolServiceBase, IApplicationSchoolService
    {
        public DemoApplicationSchoolService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId)
        {
            var res = new List<int>();
            if (announcementAppId.HasValue)
            {

                var anDa = new DemoAnnouncementForTeacherStorage(Storage);
                var announcementApplication = Storage.AnnouncementApplicationStorage.GetById(announcementAppId.Value);
                var ann = anDa.GetById(announcementApplication.AnnouncementRef);
                if (ann.ClassRef.HasValue)
                {

                    var csp = Storage.ClassPersonStorage
                        .GetClassPersons(new ClassPersonQuery { ClassId = ann.ClassRef });
                    res.AddRange(csp.Select(x => x.PersonRef));
                }
                res.Add(ann.PersonRef);
            }
            else
            {

                var inst = Storage.ApplicationInstallStorage.GetAll(new AndQueryCondition
                        {
                            {ApplicationInstall.ACTIVE_FIELD, true}, 
                            {ApplicationInstall.APPLICATION_REF_FIELD, appId}
                        });
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
            Storage.AnnouncementApplicationStorage.Add(aa);
            aa = Storage.AnnouncementApplicationStorage.GetAll(announcementId, applicationId, false).OrderByDescending(x => x.Id).First();
            return aa;
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            return Storage.AnnouncementApplicationStorage.GetById(announcementAppId);
        }

        public void AttachAppToAnnouncement(int announcementAppId)
        {
            var aa = Storage.AnnouncementApplicationStorage.GetById(announcementAppId);
            var ann = new DemoAnnouncementForTeacherStorage(Storage)
                .GetAnnouncement(aa.AnnouncementRef, Context.Role.Id, Context.UserLocalId.Value);
            if (Context.UserLocalId != ann.PersonRef)
                throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
            aa.Active = true;
            Storage.AnnouncementApplicationStorage.Update(aa);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            return Storage.AnnouncementApplicationStorage.GetAll(announcementId, onlyActive);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false)
        {
            return Storage.AnnouncementApplicationStorage.GetAnnouncementApplicationsByPerson(personId, onlyActive);
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId)
        {
            try
            {
                var aa = Storage.AnnouncementApplicationStorage.GetById(announcementAppId);
                Storage.AnnouncementApplicationStorage.Delete(announcementAppId);
                var res = ServiceLocator.AnnouncementService.GetAnnouncementById(aa.AnnouncementRef);
                if (Context.UserLocalId != res.PersonRef)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                return res;
            }
            catch
            {
                throw new ChalkableException(String.Format(ChlkResources.ERR_CANT_DELETE_ANNOUNCEMENT_APPLICATION, announcementAppId));
            }
        }
    }
}