using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
   
    public class DemoApplicationService : DemoMasterServiceBase, IApplicationService
    {
        public DemoApplicationService(IServiceLocatorMaster serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<AppPermissionType> GetPermisions(Guid applicationId)
        {
            var app = Storage.ApplicationStorage.GetById(applicationId);
            return app.Permissions.Select(x => x.Permission).ToList();
        }

        public IList<AppPermissionType> GetPermisions(string applicationUrl)
        {
            var app = Storage.ApplicationStorage.GetApplicationByUrl(applicationUrl);
            return app.Permissions.Select(x => x.Permission).ToList();
        }

        public PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null)
        {
            var query = new ApplicationQuery {Start = start, Count = count, Live = live};
            return GetApplications(query);
        }

        private PaginatedList<Application> GetApplications(ApplicationQuery query)
        {
            return Storage.ApplicationStorage.GetPaginatedApplications(query);
        }
 
        
        public Application GetApplicationById(Guid id)
        {
            return Storage.ApplicationStorage.GetApplication(new ApplicationQuery
            {
                Id = id,
                UserId = Context.UserId,
                Role = Context.Role.Id
            });
        }

        //TODO: security
        public Application GetApplicationByUrl(string url)
        {
            return Storage.ApplicationStorage.GetApplicationByUrl(url);
        }

        public ApplicationRating WriteReview(Guid applicationId, int rating, string review)
        {
            if (Storage.ApplicationRatingStorage.Exists(applicationId, Context.UserId))
                throw new ChalkableException("User can send only one review per application");

            var appRating = new ApplicationRating
            {
                Id = Guid.NewGuid(),
                ApplicationRef = applicationId,
                Rating = rating,
                Review = review,
                UserRef = Context.UserId
            };
            Storage.ApplicationRatingStorage.Add(appRating);
            return appRating;
        }

        public IList<ApplicationRating> GetRatings(Guid applicationId)
        {
            return Storage.ApplicationRatingStorage.GetAll(applicationId);
        }

        //TODO: do we need this?
        public bool CanGetSecretKey(IList<Application> applications)
        {
            if (Context.Role.Id == CoreRoles.STUDENT_ROLE.Id)//WHY???
                return true;
            if (Context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id)
            {
                var developer = ServiceLocator.DeveloperService.GetDeveloperById(Context.UserId);
                return applications.All(x => x.DeveloperRef == developer.Id);
            }
            return false;
        }

        public bool HasMyApps(Application application)
        {
            if (BaseSecurity.IsAdminViewer(ServiceLocator.Context))
                return application.HasAdminMyApps;
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                return application.HasTeacherMyApps;
            return Context.Role.Id == CoreRoles.STUDENT_ROLE.Id && application.HasStudentMyApps;
        }


        public PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, Services.Master.AppFilterMode? filterMode, Services.Master.AppSortingMode? sortingMode, int start = 0, int count = int.MaxValue)
        {
            var query = new ApplicationQuery
            {
                CategoryIds = categoriesIds,
                GradeLevels = gradeLevels,
                Filter = filterWords,
                Start = start,
                Count = count
            };
            filterMode = filterMode ?? AppFilterMode.All;
            sortingMode = sortingMode ?? AppSortingMode.Newest;
            if (filterMode != AppFilterMode.All)
                query.Free = filterMode == AppFilterMode.Free;
            switch (sortingMode)
            {
                case AppSortingMode.Newest:
                    query.OrderBy = Application.CREATE_DATE_TIME_FIELD;
                    break;
                case AppSortingMode.HighestRated:
                    query.OrderBy = Application.AVG_FIELD;
                    break;
            }
            return GetApplications(query);
        }


        public bool ExistsReview(Guid applicationId)
        {
            return Storage.ApplicationRatingStorage.Exists(applicationId, Context.UserId);
        }
    }

}