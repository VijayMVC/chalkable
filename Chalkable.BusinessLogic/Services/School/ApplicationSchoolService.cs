using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
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
        AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementType type, Guid applicationId);
        AnnouncementApplication GetAnnouncementApplication(int announcementAppId);
        void AttachAppToAnnouncement(int announcementAppId, AnnouncementType type);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false);
        Announcement RemoveFromAnnouncement(int announcementAppId, AnnouncementType type);

    }

    public class ApplicationSchoolService : SchoolServiceBase, IApplicationSchoolService
    {
        public ApplicationSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            var res = new List<int>();
            using (var uow = Read())
            {
                if (announcementAppId.HasValue)
                {
                    var anDa = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId.Value);
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

        public AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementType type, Guid applicationId)
        {
            if (!Context.SCEnabled)
                throw new StudyCenterDisabledException();
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            if(!CanAddToAnnouncement(app.Id))
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
                        {AnnouncementApplication.ANNOUNCEMENT_REF_FIELD, announcementId},
                        {AnnouncementApplication.APPLICATION_REF_FIELD, applicationId},
                        {AnnouncementApplication.ACTIVE_FIELD, false}
                    }).OrderByDescending(x=>x.Id).First();
                return aa;
            }
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            return DoRead(uow => new AnnouncementApplicationDataAccess(uow).GetById(announcementAppId));
        }

        public void AttachAppToAnnouncement(int announcementAppId, AnnouncementType announcementType)
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

        private bool CanAddToAnnouncement(Guid appId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var assessmentId = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetAssessmentId();
            var appInstall = ServiceLocator.AppMarketService.GetInstallationForPerson(appId, Context.PersonId.Value);
            return assessmentId == appId  || (appInstall != null && appInstall.Active);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            //TODO: thing about security
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var ps = new AndQueryCondition {{AnnouncementApplication.ANNOUNCEMENT_REF_FIELD, announcementId}};
                if (onlyActive)
                    ps.Add(AnnouncementApplication.ACTIVE_FIELD, true);
                return da.GetAll(ps);
            }
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                return da.GetAnnouncementApplicationsByPerson(personId, onlyActive);
            }
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId, AnnouncementType type)
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
                throw new ChalkableException(String.Format(ChlkResources.ERR_CANT_DELETE_ANNOUNCEMENT_APPLICATION, announcementAppId));
            }
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
    }
}