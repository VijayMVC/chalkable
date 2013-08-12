using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IApplicationSchoolService
    {
        IList<Guid> GetAssignedUserIds(Guid appId, Guid? announcementAppId);
        AnnouncementApplication AddToAnnouncement(Guid announcementId, Guid applicationId);
        AnnouncementApplication GetAnnouncementApplication(Guid announcementAppId);
        void AttachAppToAnnouncement(Guid announcementAppId);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(Guid announcementId, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(Guid personId, bool onlyActive = false);
        Announcement RemoveFromAnnouncement(Guid announcementAppId);

    }

    public class ApplicationSchoolService : SchoolServiceBase, IApplicationSchoolService
    {
        public ApplicationSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<Guid> GetAssignedUserIds(Guid appId, Guid? announcementAppId)
        {
            var res = new List<Guid>();
            using (var uow = Read())
            {
                if (announcementAppId.HasValue)
                {
                    var anDa = new AnnouncementDataAccess(uow);
                    var da = new AnnouncementApplicationDataAccess(uow);
                    var announcementApplication = da.GetById(announcementAppId.Value);
                    var ann = anDa.GetById(announcementApplication.AnnouncementRef);
                    if (ann.MarkingPeriodClassRef.HasValue)
                    {
                        var mpclass = new MarkingPeriodClassDataAccess(uow).GetById(ann.MarkingPeriodClassRef.Value);
                        var csp = new ClassPersonDataAccess(uow).GetClassPersons(new ClassPersonQuery{ClassId = mpclass.ClassRef});
                        res.AddRange(csp.Select(x=>x.PersonRef));
                    }
                    res.Add(ann.PersonRef);
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

        public AnnouncementApplication AddToAnnouncement(Guid announcementId, Guid applicationId)
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
                new AnnouncementApplicationDataAccess(uow).Insert(aa);
                return aa;
            }
        }

        public AnnouncementApplication GetAnnouncementApplication(Guid announcementAppId)
        {
            using (var uow = Read())
            {
                return new AnnouncementApplicationDataAccess(uow).GetById(announcementAppId);
            }
        }

        public void AttachAppToAnnouncement(Guid announcementAppId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var aa = da.GetById(announcementAppId);
                var ann = new AnnouncementDataAccess(uow).GetAnnouncement(aa.AnnouncementRef, Context.Role.Id, Context.UserId);
                if (Context.UserId != ann.PersonRef)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Active = true;
                da.Update(aa);
                uow.Commit();
            }
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(Guid announcementId, bool onlyActive = false)
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

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(Guid personId, bool onlyActive = false)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                return da.GetAnnouncementApplicationsByPerson(personId, onlyActive);
            }
        }

        public Announcement RemoveFromAnnouncement(Guid announcementAppId)
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
                    if (Context.UserId != res.PersonRef)
                        throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                    return res;
                }
            }
            catch
            {
                throw new ChalkableException(String.Format(ChlkResources.ERR_CANT_DELETE_ANNOUNCEMENT_APPLICATION, announcementAppId));
            }
        }
    }
}