using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationService
    {
        IList<AppPermissionType> GetPermisions(Guid applicationId);
        IList<AppPermissionType> GetPermisions(string applicationUrl);
        IList<Application> GetApplications();
        IList<Application> GetNewestApplications();
        IList<Application> GetHigestRatedApplications();
        IList<Application> GetPopularApplications();
        IList<Application> GetFreeApplications();
        IList<Application> GetApplicationsByCategory(Guid categoryId);
        Application GetApplicationById(Guid id, bool includeInstallPermission = true);
        Application GetApplicationByUrl(string url);
        ApplicationRating WriteReveiw(Guid applicationId, int rating, string review);
        IList<ApplicationRating> GetRatings(Guid applicationId);
        bool CanGetSecretKey(IList<Application> applications);
        bool HasMyApps(Application application);
    }


    public class ApplicationService : MasterServiceBase, IApplicationService
    {
        public ApplicationService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<AppPermissionType> GetPermisions(Guid applicationId)
        {
            using (var uow = Read())
            {
                var app = new ApplicationDataAccess(uow).GetApplicationById(applicationId);
                return app.Permissions.Select(x => x.Permission).ToList();
            }
        }

        public IList<AppPermissionType> GetPermisions(string applicationUrl)
        {
            using (var uow = Read())
            {
                var app = new ApplicationDataAccess(uow).GetApplicationByUrl(applicationUrl);
                return app.Permissions.Select(x => x.Permission).ToList();
            }
        }

        public IList<Application> GetApplications()
        {
            using (var uow = Read())
            {
                var apps = new ApplicationDataAccess(uow).GetApplications(Context.UserId, Context.Role.Id);
                return apps;
            }
        }

        public IList<Application> GetNewestApplications()
        {
            using (var uow = Read())
            {
                var apps = new ApplicationDataAccess(uow).GetApplications(Context.UserId, Context.Role.Id, Application.CREATE_DATE_TIME_FIELD);
                return apps;
            }
        }

        public IList<Application> GetHigestRatedApplications()
        {
            using (var uow = Read())
            {
                var apps = new ApplicationDataAccess(uow).GetApplications(Context.UserId, Context.Role.Id, Application.AVG_FIELD);
                return apps;
            }
        }

        public IList<Application> GetPopularApplications()
        {
            //TODO: need to count installs on master
            throw new NotImplementedException();
        }

        public IList<Application> GetFreeApplications()
        {
            var res = GetApplications();
            return res.Where(x => x.Price == 0 && (!x.PricePerClass.HasValue || x.PricePerClass == 0)
                && (!x.PricePerSchool.HasValue || x.PricePerSchool == 0)).ToList();
        }

        public IList<Application> GetApplicationsByCategory(Guid categoryId)
        {
            using (var uow = Read())
            {
                var apps = new ApplicationDataAccess(uow).GetApplications(Context.UserId, Context.Role.Id, Application.NAME_FIELD, false, categoryId);
                return apps;
            }
        }
        
        public Application GetApplicationById(Guid id, bool includeInstallPermission = true)
        {
            using (var uow = Read())
            {
                var apps = new ApplicationDataAccess(uow).GetApplications(Context.UserId, Context.Role.Id, Application.CREATE_DATE_TIME_FIELD, includeInstallPermission);
                return apps.FirstOrDefault(x=>x.Id == id);
            }
        }

        //TODO: security
        public Application GetApplicationByUrl(string url)
        {
            using (var uow = Read())
            {
                var app = new ApplicationDataAccess(uow).GetApplicationByUrl(url);
                return app;
            }
        }

        public ApplicationRating WriteReveiw(Guid applicationId, int rating, string review)
        {
            using (var uow = Update())
            {
                var res = new ApplicationRating
                    {
                        Id = Guid.NewGuid(),
                        ApplicationRef = applicationId,
                        Rating = rating,
                        Review = review,
                        UserRef = Context.UserId
                    };
                new ApplicationRatingDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }

        public IList<ApplicationRating> GetRatings(Guid applicationId)
        {
            using (var uow = Read())
            {
                var da = new ApplicationRatingDataAccess(uow);
                return da.GetAll(new Dictionary<string, object> {{ApplicationRating.APPLICATION_REF_FIELD, applicationId}});
            }
        }

        //TODO: do we ned this?
        public bool CanGetSecretKey(IList<Application> applications)
        {
            throw new System.NotImplementedException();
        }

        public bool HasMyApps(Application application)
        {
            if (BaseSecurity.IsAdminViewer(ServiceLocator.Context))
                return application.HasAdminMyApps;
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                return application.HasTeacherMyApps;
            return Context.Role.Id == CoreRoles.STUDENT_ROLE.Id && application.HasStudentMyApps;
        }
    }
}