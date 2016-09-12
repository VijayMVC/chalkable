using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IApplicationSchoolService
    {
        AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementTypeEnum type, Guid applicationId);
        AnnouncementApplication GetAnnouncementApplication(int announcementAppId);
        void AttachAppToAnnouncement(int announcementAppId, AnnouncementTypeEnum announcementType);
        void UpdateAnnouncementApplicationMeta(int announcementApplicationId, AnnouncementTypeEnum announcementType, string text, string imageUrl, string description);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false);
        IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive = false);
        Announcement RemoveFromAnnouncement(int announcementAppId, AnnouncementTypeEnum type);
        IList<AnnouncementApplication> CopyAnnApplications(int toAnnouncementId, IList<AnnouncementApplication> annAppsForCopying);
        IList<AnnouncementApplicationRecipient> GetAnnouncementApplicationRecipients(int? studentId, Guid appId);
        void UpdateStudentAnnouncementApplicationMeta(int announcementApplicationId, int studentId, string text);
        IList<StudentAnnouncementApplicationMeta> GetStudentAnnouncementApplicationMetaByAnnouncementId(int announcementId);
    }

    public class ApplicationSchoolService : SchoolServiceBase, IApplicationSchoolService
    {
        public ApplicationSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public AnnouncementApplication AddToAnnouncement(int announcementId, AnnouncementTypeEnum type, Guid applicationId)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);

            EnsureApplicationPermission(app.Id);

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

                var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
                if (!ann.IsOwner && Context.Role != CoreRoles.DISTRICT_ADMIN_ROLE)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Active = true;
                da.Update(aa);
                uow.Commit();
            }
        }

        public void UpdateAnnouncementApplicationMeta(int announcementApplicationId, AnnouncementTypeEnum announcementType, string text, string imageUrl, string description)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = new AnnouncementApplicationDataAccess(uow);
                var aa = da.GetById(announcementApplicationId);
                var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(aa.AnnouncementRef);
                if (!ann.IsOwner && Context.Role != CoreRoles.DISTRICT_ADMIN_ROLE)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                aa.Text = text;

                if (string.IsNullOrWhiteSpace(imageUrl) || !Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    imageUrl = null;

                aa.ImageUrl = imageUrl;   
                aa.Description = description;
                da.Update(aa);
                uow.Commit();
            }
        }

        //TODO: maybe move this to ApplicationService and make it public 
        private void EnsureApplicationPermission(Guid appId)
        {
            if (appId == ServiceLocator.ServiceLocatorMaster.ApplicationService.GetAssessmentId())
            {
                if (!ApplicationSecurity.HasAssessmentEnabled(Context) && !ApplicationSecurity.HasStudyCenterAccess(Context))
                    throw new ChalkableSecurityException("Current user has disabled assessment access");
            }
            else if(!ApplicationSecurity.HasStudyCenterAccess(Context))
                    throw new StudyCenterDisabledException();
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
                    if (!res.IsOwner && Context.Role != CoreRoles.DISTRICT_ADMIN_ROLE)
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

        public IList<AnnouncementApplicationRecipient> GetAnnouncementApplicationRecipients(int? studentId, Guid appId)
        {
            if (Context.Role == CoreRoles.STUDENT_ROLE)
            {
                if (studentId != null)
                {
                    if (Context.PersonId != studentId)
                        return new List<AnnouncementApplicationRecipient>();
                }
                else
                    studentId = Context.PersonId;
            }
            var teacherId = Context.Role == CoreRoles.TEACHER_ROLE ? Context.PersonId : null;
            var adminId = Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE ? Context.PersonId : null;
            var schoolYear = Context.SchoolYearId.Value;
            return DoRead(u => new AnnouncementApplicationDataAccess(u).GetAnnouncementApplicationRecipients(studentId, teacherId, adminId, appId, schoolYear));
        }

        public void UpdateStudentAnnouncementApplicationMeta(int announcementApplicationId, int studentId, string text)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = new StudentAnnouncementApplicationMetaDataAccess(uow);
                var stAnnAppMeta = new StudentAnnouncementApplicationMeta
                {
                    AnnouncementApplicationRef = announcementApplicationId,
                    StudentRef = studentId,
                    Text = text ?? ""
                };
                if (da.Exists(announcementApplicationId, studentId))
                    da.Update(stAnnAppMeta);
                else
                    da.Insert(stAnnAppMeta);
                uow.Commit();
            }
        }

        public IList<StudentAnnouncementApplicationMeta> GetStudentAnnouncementApplicationMetaByAnnouncementId(int announcementId)
        {
            return DoRead(uow => new StudentAnnouncementApplicationMetaDataAccess(uow).GetStudentAnnouncementApplicationMetaByAnnouncementId(announcementId));
        }
    }
}