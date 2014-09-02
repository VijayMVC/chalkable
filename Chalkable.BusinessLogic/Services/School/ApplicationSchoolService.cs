using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IApplicationSchoolService
    {
        IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId);
        AnnouncementApplication AddToAnnouncement(int announcementId, Guid applicationId);
        AnnouncementApplication GetAnnouncementApplication(int announcementAppId);
        void AttachAppToAnnouncement(int announcementAppId);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false);
        Announcement RemoveFromAnnouncement(int announcementAppId);

    }

    public class ApplicationSchoolService : SchoolServiceBase, IApplicationSchoolService
    {
        public ApplicationSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<int> GetAssignedUserIds(Guid appId, int? announcementAppId)
        {
            var res = new List<int>();
            using (var uow = Read())
            {
                if (announcementAppId.HasValue)
                {
                    var anDa = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId);
                    var da = new AnnouncementApplicationDataAccess(uow);
                    var announcementApplication = da.GetById(announcementAppId.Value);
                    var ann = anDa.GetById(announcementApplication.AnnouncementRef);
                        var csp = new ClassPersonDataAccess(uow, Context.SchoolLocalId)
                            .GetClassPersons(new ClassPersonQuery { ClassId = ann.ClassRef });
                        res.AddRange(csp.Select(x=>x.PersonRef));

                    //TODO: think about this
                    var teacherId = new ClassDataAccess(uow, Context.SchoolLocalId).GetById(ann.ClassRef).PrimaryTeacherRef; 
                    if(teacherId.HasValue) res.Add(teacherId.Value);
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

        public AnnouncementApplication AddToAnnouncement(int announcementId, Guid applicationId)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Update())
            {
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
            using (var uow = Read())
            {
                return new AnnouncementApplicationDataAccess(uow).GetById(announcementAppId);
            }
        }

        public void AttachAppToAnnouncement(int announcementAppId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var aa = da.GetById(announcementAppId);
                var ann = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId)
                    .GetAnnouncement(aa.AnnouncementRef, Context.Role.Id, Context.UserLocalId.Value);
                if (!ann.IsOwner)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Active = true;
                da.Update(aa);
                uow.Commit();
            }
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            //TODO: thing about security
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var ps = new AndQueryCondition();
                ps.Add(AnnouncementApplication.ANNOUNCEMENT_REF_FIELD, announcementId);
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

        public Announcement RemoveFromAnnouncement(int announcementAppId)
        {
            try
            {
                //TODO: thing about security
                using (var uow = Update())
                {
                    var da = new AnnouncementApplicationDataAccess(uow);
                    var aa = da.GetById(announcementAppId);

                    da.Delete(announcementAppId);
                    uow.Commit();
                    var res = ServiceLocator.AnnouncementService.GetAnnouncementById(aa.AnnouncementRef);
                    if (!res.IsOwner)
                        throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
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