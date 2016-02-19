using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IApplicationSchoolService
    {
        IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId);
        AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementTypeEnum type, Guid applicationId);
        AnnouncementApplication GetAnnouncementApplication(int announcementAppId);
        void AttachAppToAnnouncement(int announcementAppId, AnnouncementTypeEnum announcementType);
        void UpdateAnnouncementApplicationMeta(int announcementApplicationId, AnnouncementTypeEnum announcementType, string text, string imageUrl);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false);
        Announcement RemoveFromAnnouncement(int announcementAppId, AnnouncementTypeEnum type);
        IList<AnnouncementApplication> CopyAnnApplications(int toAnnouncementId, IList<AnnouncementApplication> annAppsForCopying);

        void BanUnBanApplication(Guid applicationId, bool ban);
        IList<ApplicationBanHistory> GetApplicationBanHistory(Guid applicationId);
    }

    public class ApplicationSchoolService : SchoolServiceBase, IApplicationSchoolService
    {
        public ApplicationSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var res = new List<int>();
            using (var uow = Read())
            {
                if (announcementAppId.HasValue)
                {
                    var anDa = new ClassAnnouncementForTeacherDataAccess(uow, Context.SchoolYearId.Value);
                    var da = new AnnouncementApplicationDataAccess(uow);
                    var announcementApplication = da.GetById(announcementAppId.Value);
                    var ann = anDa.GetById(announcementApplication.AnnouncementRef);

                    var csp = new ClassPersonDataAccess(uow)
                        .GetClassPersons(new ClassPersonQuery { ClassId = ann.ClassRef });
                    res.AddRange(csp.Select(x => x.PersonRef));

                    //TODO: think about this
                    var teacherId = new ClassDataAccess(uow).GetById(ann.ClassRef).PrimaryTeacherRef;
                    if (teacherId.HasValue) res.Add(teacherId.Value);   
                    
                }
                else
                {
                    var inst = new ApplicationInstallDataAccess(uow).GetAll(new AndQueryCondition
                        {
                            {ApplicationInstall.ACTIVE_FIELD, true}, 
                            {ApplicationInstall.APPLICATION_REF_FIELD, appId}
                        });
                    res.AddRange(inst.Select(x=>x.PersonRef));
                }
            }
            return res;
        }

        public AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementTypeEnum type, Guid applicationId)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);

            EnsureApplicationPermission(app.Id);

            if (!CanAddToAnnouncement(app.Id))
                throw new ChalkableSecurityException("Application is not installed yet");

            using (var uow = Update())
            {
                var ann = ServiceLocator.GetAnnouncementService(type).GetAnnouncementDetails(announcementId);
                if (!ApplicationSecurity.CanAddToAnnouncement(app, ann, Context))
                    throw new ChalkableSecurityException();
                var aa = new AnnouncementApplication
                    {
                        AnnouncementRef = announcementId,
                        ApplicationRef = applicationId,
                        Active = false,
                        Order = ServiceLocator.GetAnnouncementService(type).GetNewAnnouncementItemOrder(ann)
                    };
                var da = new AnnouncementApplicationDataAccess(uow);
                da.Insert(aa);
                uow.Commit();
                aa = da.GetAll(new AndQueryCondition
                    {
                        {nameof(AnnouncementApplication.AnnouncementRef), announcementId},
                        {nameof(AnnouncementApplication.ApplicationRef), applicationId},
                        {nameof(AnnouncementApplication.Active), false}
                    }).OrderByDescending(x=>x.Id).First();
                return aa;
            }
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            return DoRead(uow => new AnnouncementApplicationDataAccess(uow).GetById(announcementAppId));
        }

        public void AttachAppToAnnouncement(int announcementAppId, AnnouncementTypeEnum announcementType)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var aa = da.GetById(announcementAppId);
                if(!CanAddToAnnouncement(aa.ApplicationRef))
                    throw new ChalkableSecurityException("Application is not installed yet");

                var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
                if (!ann.IsOwner)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Active = true;
                da.Update(aa);
                uow.Commit();
            }
        }

        public void UpdateAnnouncementApplicationMeta(int announcementApplicationId, AnnouncementTypeEnum announcementType, string text, string imageUrl)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var aa = da.GetById(announcementApplicationId);
                if (!CanAddToAnnouncement(aa.ApplicationRef))
                    throw new ChalkableSecurityException("Application is not installed yet");
                var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
                if (!ann.IsOwner)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Text = text;
                aa.ImageUrl = imageUrl;
                da.Update(aa);
                uow.Commit();
            }
        }

        //TODO: maybe move this to ApplicationService and make it public 
        private void EnsureApplicationPermission(Guid appId)
        {
            if (appId == ServiceLocator.ServiceLocatorMaster.ApplicationService.GetAssessmentId())
            {
                if (!ApplicationSecurity.HasAssessmentEnabled(Context))
                    throw new ChalkableSecurityException("Current user has disabled assessment access");
            }
            else if(!ApplicationSecurity.HasStudyCenterAccess(Context))
                    throw new StudyCenterDisabledException();
        }

        private bool CanAddToAnnouncement(Guid appId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var assessmentId = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetAssessmentId();
            var appInstall = ServiceLocator.AppMarketService.GetInstallationForPerson(appId, Context.PersonId.Value);
            return assessmentId == appId || (appInstall != null && appInstall.Active);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            //TODO: thing about security
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var ps = new AndQueryCondition {{nameof(AnnouncementApplication.AnnouncementRef), announcementId}};
                if (onlyActive)
                    ps.Add(nameof(AnnouncementApplication.Active), true);
                return da.GetAll(ps);
            }
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false)
        {
            return DoRead(u => new AnnouncementApplicationDataAccess(u).GetAnnouncementApplicationsByPerson(personId, onlyActive));
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId, AnnouncementTypeEnum type)
        {
            try
            {
                using (var uow = Update())
                {
                    var da = new AnnouncementApplicationDataAccess(uow);
                    var aa = da.GetById(announcementAppId);
                    var res = ServiceLocator.GetAnnouncementService(type).GetAnnouncementById(aa.AnnouncementRef);
                    if (!res.IsOwner)
                        throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);

                    da.Delete(announcementAppId);
                    uow.Commit();
                    return res;
                }
            }
            catch
            {
                throw new ChalkableException(string.Format(ChlkResources.ERR_CANT_DELETE_ANNOUNCEMENT_APPLICATION, announcementAppId));
            }
        }

        public IList<AnnouncementApplication> CopyAnnApplications(int toAnnouncementId, IList<AnnouncementApplication> annAppsForCopying)
        {
            return DoRead(u => CopyAnnApplications(annAppsForCopying, new List<int> {toAnnouncementId}, u));
        }

        public void BanUnBanApplication(Guid applicationId, bool ban)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.DistrictId.HasValue);
            ServiceLocator.ServiceLocatorMaster.ApplicationService.SetApplicationDistrictOptions(applicationId, Context.DistrictId.Value, ban);
            DoUpdate(u=> new ApplicationBanHistoryDataAccess(u).Insert(new ApplicationBanHistory
            {
                ApplicationRef = applicationId,
                Banned = ban,
                Date = Context.NowSchoolTime,
                PersonRef = Context.PersonId.Value,
            }));
        }

        public static IList<AnnouncementApplication> CopyAnnApplications(IList<AnnouncementApplication> annAppsForCopying, IList<int> toAnnouncementIds, UnitOfWork unitOfWork)
        {
            var da = new AnnouncementApplicationDataAccess(unitOfWork);
            foreach (var toAnnouncementId in toAnnouncementIds)
            {
                var res = annAppsForCopying.Select(aa => new AnnouncementApplication
                {
                    ApplicationRef = aa.ApplicationRef,
                    Active = aa.Active,
                    Order = aa.Order,
                    AnnouncementRef = toAnnouncementId,
                }).ToList();
                da.Insert(res);   
            }
            return da.GetAnnouncementApplicationsbyAnnIds(toAnnouncementIds);
        }


        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false)
        {
            using (var uow = Read())
            {
                var res =  new AnnouncementApplicationDataAccess(uow).GetAnnouncementApplicationsbyAnnIds(announcementIds);
                if (onlyActive)
                    res = res.Where(x => x.Active).ToList();
                return res;
            }
        }

        public IList<ApplicationBanHistory> GetApplicationBanHistory(Guid applicationId)
        {
            return DoRead(u => new ApplicationBanHistoryDataAccess(u).GetApplicationBanHistory(applicationId));
        }
    }
}