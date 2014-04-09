using System;
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
            throw new NotImplementedException();
            var res = new List<int>();
            using (var uow = Read())
            {
                if (announcementAppId.HasValue)
                {
                    var anDa = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId);
                    var da = new AnnouncementApplicationDataAccess(uow);
                    var announcementApplication = da.GetById(announcementAppId.Value);
                    var ann = anDa.GetById(announcementApplication.AnnouncementRef);
                    if (ann.ClassRef.HasValue)
                    {
                        var csp = new ClassPersonDataAccess(uow, Context.SchoolLocalId)
                            .GetClassPersons(new ClassPersonQuery { ClassId = ann.ClassRef });
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

        public AnnouncementApplication AddToAnnouncement(int announcementId, Guid applicationId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
            using (var uow = Read())
            {
                return new AnnouncementApplicationDataAccess(uow).GetById(announcementAppId);
            }
        }

        public void AttachAppToAnnouncement(int announcementAppId)
        {
            throw new NotImplementedException();
            using (var uow = Update())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var aa = da.GetById(announcementAppId);
                var ann = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId)
                    .GetAnnouncement(aa.AnnouncementRef, Context.Role.Id, Context.UserLocalId.Value);
                if (Context.UserLocalId != ann.PersonRef)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Active = true;
                da.Update(aa);
                uow.Commit();
            }
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            return new List<AnnouncementApplication>();
            /*throw new NotImplementedException();
            
            //TODO: thing about security
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var ps = new AndQueryCondition();
                ps.Add(AnnouncementApplication.ANNOUNCEMENT_REF_FIELD, announcementId);
                if (onlyActive)
                    ps.Add(AnnouncementApplication.ACTIVE_FIELD, true);
                return da.GetAll(ps);
            }*/
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false)
        {
            throw new NotImplementedException();
            using (var uow = Read())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                return da.GetAnnouncementApplicationsByPerson(personId, onlyActive);
            }
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId)
        {
            throw new NotImplementedException();
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
                    if (Context.UserLocalId != res.PersonRef)
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