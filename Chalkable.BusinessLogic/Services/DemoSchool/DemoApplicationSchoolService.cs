﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementApplicationStorage : BaseDemoIntStorage<AnnouncementApplication>
    {
        public DemoAnnouncementApplicationStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<AnnouncementApplication> GetAll(int announcementId, Guid applicationId, bool active)
        {
            return
                data.Where(
                    x =>
                        x.Value.AnnouncementRef == announcementId && x.Value.ApplicationRef == applicationId &&
                        x.Value.Active == active).Select(x => x.Value).ToList();
        }

        public IList<AnnouncementApplication> GetAll(int announcementId, bool onlyActive)
        {
            var aa = data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value);
            if (onlyActive)
                aa = aa.Where(x => x.Active);
            return aa.ToList();
        }

        public void DeleteByAnnouncementId(int announcementId)
        {
            var announcementAttachments = GetAll(announcementId, false);
            Delete(announcementAttachments);
        }

        
    }

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
            throw new NotImplementedException();
            //TODO: impl this later
            //var res = new List<int>();
            //if (announcementAppId.HasValue)
            //{

            //    var anDa = ((DemoAnnouncementService)ServiceLocator.AnnouncementService).GetTeacherAnnouncementService();
            //    var announcementApplication = GetAnnouncementApplication(announcementAppId.Value);
            //    var ann = anDa.GetAnnouncementById(announcementApplication.AnnouncementRef);
            //    var csp = ServiceLocator.ClassService.GetClassPersons(ann.ClassRef.Value, null);
            //    res.AddRange(csp.Select(x => x.PersonRef));
            //    var c = ServiceLocator.ClassService.GetById(ann.ClassRef.Value);
            //    if (Context.PersonId != c.PrimaryTeacherRef)
            //        if (c.PrimaryTeacherRef.HasValue) res.Add(c.PrimaryTeacherRef.Value);
            //}
            //else
            //{
            //    var inst = ((DemoAppMarketService)ServiceLocator.AppMarketService).GetAppInstalls(appId, true);
            //    res.AddRange(inst.Select(x => x.PersonRef));
            //}
            //return res;
        }

        public AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementTypeEnum announcementType, Guid applicationId)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            if (!ApplicationSecurity.CanAddToAnnouncement(app, ann, Context))
                throw new ChalkableSecurityException();
            var aa = new AnnouncementApplication
            {
                AnnouncementRef = announcementId,
                ApplicationRef = applicationId,
                Active = false,
                Order = ServiceLocator.GetAnnouncementService(announcementType).GetNewAnnouncementItemOrder(ann)
            };
            AnnouncementApplicationStorage.Add(aa);
            aa = AnnouncementApplicationStorage.GetAll(announcementId, applicationId, false).OrderByDescending(x => x.Id).First();
            return aa;
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            return AnnouncementApplicationStorage.GetById(announcementAppId);
        }

        public void AttachAppToAnnouncement(int announcementAppId, AnnouncementTypeEnum announcementType)
        {
            var aa = GetAnnouncementApplication(announcementAppId);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
            if (ann.IsOwner)
                throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
            aa.Active = true;
            AnnouncementApplicationStorage.Update(aa);
        }

        public void UpdateAnnouncementApplicationMeta(int announcementApplicationId, AnnouncementTypeEnum announcementType, string text, string imageUrl)
        {
            var aa = GetAnnouncementApplication(announcementApplicationId);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
            if (ann.IsOwner)
                throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
            aa.Text = text;
            aa.ImageUrl = imageUrl;
            AnnouncementApplicationStorage.Update(aa);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false)
        {
            var res = AnnouncementApplicationStorage.GetAll()
                       .Where(x => announcementIds.Contains(x.AnnouncementRef));
            if (onlyActive)
                res = res.Where(x => x.Active);
            return res.ToList();
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            return GetAnnouncementApplicationsByAnnIds(new List<int> { announcementId }, onlyActive);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false)
        {
            //implement this later
            throw new NotImplementedException();
            //var announcementApplications = AnnouncementApplicationStorage.GetData().Select(x => x.Value);

            //announcementApplications = announcementApplications.Where(x =>
            //{
            //    var announcement = ServiceLocator.AnnouncementService.GetAnnouncementById(x.AnnouncementRef);
            //    return ((DemoAppMarketService)ServiceLocator.AppMarketService).AppInstallExists(x.ApplicationRef, personId)
            //           && announcement.PrimaryTeacherRef == personId
            //           || ((DemoClassService)ServiceLocator.ClassService).ClassPersonExists(announcement.ClassRef.Value, personId);
            //});
            //return announcementApplications.ToList();
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId, AnnouncementTypeEnum announcementType)
        {
            try
            {
                var aa = AnnouncementApplicationStorage.GetById(announcementAppId);
                AnnouncementApplicationStorage.Delete(announcementAppId);
                var res = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
                if (res.IsOwner)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                return res;
            }
            catch (Exception)
            {
                throw new ChalkableException(String.Format(ChlkResources.ERR_CANT_DELETE_ANNOUNCEMENT_APPLICATION, announcementAppId));
            }
        }

        public IList<AnnouncementApplication> CopyAnnApplications(int toAnnouncementId, IList<AnnouncementApplication> annAppsForCopying)
        {
            throw new NotImplementedException();
        }

        public void BanUnBanApplication(Guid applicationId, bool ban)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationBanHistory> GetApplicationBanHistory(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public void DeleteAnnouncementApplications(int announcementId, bool onlyActive = false)
        {
            var announcementApps = AnnouncementApplicationStorage.GetAll(announcementId, onlyActive);
            AnnouncementApplicationStorage.Delete(announcementApps);
        }

        public IList<AnnouncementApplicationRecipient> GetAnnouncementApplicationRecipients(int? studentId, Guid appId)
        {
            throw new NotImplementedException();
        }
    }
}