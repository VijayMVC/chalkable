using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationService
    {
        IList<AppPermissionType> GetPermisions(string applicationUrl);
        PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null, bool onlyForInstall = true);
        PaginatedList<Application> GetApplications(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0, int count = int.MaxValue);
        
        PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, AppFilterMode? filterMode
            , AppSortingMode? sortingMode, int start = 0, int count = int.MaxValue);

        IList<Application> GetApplicationsByIds(IList<Guid> ids);
        Application GetApplicationById(Guid id);
        Application GetApplicationByUrl(string url);
        ApplicationRating WriteReview(Guid applicationId, int rating, string review);
        bool ExistsReview(Guid applicationId);
        IList<ApplicationRating> GetRatings(Guid applicationId);
        bool CanGetSecretKey(IList<Application> applications);
        bool HasMyApps(Application application);

        IList<Application> GetSuggestedApplications(List<string> standardsCodes, List<Guid> installedAppsIds, int start, int count);
    }


    public class ApplicationService : MasterServiceBase, IApplicationService
    {
        public ApplicationService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }
        
        public IList<AppPermissionType> GetPermisions(string applicationUrl)
        {
            using (var uow = Read())
            {
                var app = new ApplicationDataAccess(uow).GetApplicationByUrl(applicationUrl);
                return app.Permissions.Select(x => x.Permission).ToList();
            }
        }

        public PaginatedList<Application> GetApplications(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0, int count = int.MaxValue)
        {
            var query = new ApplicationQuery
            {
                Start = start,
                Count = count,
                DeveloperId = developerId,
                State = state,
                Filter = filter
            };
            return GetApplications(query);
        }

        public PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null, bool onlyForInstall = true)
        {
            var query = new ApplicationQuery
                {
                    Start = start, 
                    Count = count, 
                    Live = live, 
                    OnlyForInstall = onlyForInstall,
                };
            return GetApplications(query);
        }

        private PaginatedList<Application> GetApplications(ApplicationQuery query)
        {
            using (var uow = Read())
            {
                query.UserId = Context.UserId;
                query.Role = Context.Role.Id;
                if(!BaseSecurity.IsSysAdmin(Context))
                    query.DeveloperId = Context.DeveloperId;
                return new ApplicationDataAccess(uow).GetPaginatedApplications(query);
            }
        }
 
        
        public Application GetApplicationById(Guid id)
        {
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow)
                    .GetApplication(new ApplicationQuery
                        {
                            Id = id,
                            UserId = Context.UserId,
                            Role = Context.Role.Id,
                            OnlyForInstall = false
                        });
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

        public ApplicationRating WriteReview(Guid applicationId, int rating, string review)
        {
            using (var uow = Update())
            {
                var da = new ApplicationRatingDataAccess(uow);
                if(da.Exists(applicationId, Context.UserId))
                    throw new ChalkableException("User can send only one review per application");
                var res = new ApplicationRating
                    {
                        Id = Guid.NewGuid(),
                        ApplicationRef = applicationId,
                        Rating = rating,
                        Review = review,
                        UserRef = Context.UserId
                    };
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }

        public IList<ApplicationRating> GetRatings(Guid applicationId)
        {
            using (var uow = Read())
            {
                var da = new ApplicationRatingDataAccess(uow);
                return da.GetAll(new AndQueryCondition {{ApplicationRating.APPLICATION_REF_FIELD, applicationId}});
            }
        }

        //TODO: can we apply thin on service leyer get methods?
        public bool CanGetSecretKey(IList<Application> applications)
        {
            return applications.All(x => x.DeveloperRef == Context.UserId);
        }

        public bool HasMyApps(Application application)
        {
            if (BaseSecurity.IsAdminViewer(ServiceLocator.Context))
                return application.HasAdminMyApps;
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                return application.HasTeacherMyApps;
            return Context.Role.Id == CoreRoles.STUDENT_ROLE.Id && application.HasStudentMyApps;
        }


        public PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, AppFilterMode? filterMode, AppSortingMode? sortingMode, int start = 0, int count = int.MaxValue)
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
            using (var uow = Read())
            {
                return new ApplicationRatingDataAccess(uow).Exists(applicationId, Context.UserId);
            }
        }


        public IList<Application> GetApplicationsByIds(IList<Guid> ids)
        {
            using (var uow = Read())
            {
                var res = new ApplicationDataAccess(uow).GetByIds(ids);
                return res.Where(x=>x.State == ApplicationStateEnum.Live).ToList();
            }
        }


        public IList<Application> GetSuggestedApplications(List<string> standardsCodes, List<Guid> installedAppsIds, int start, int count)
        {
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow).GetSuggestedApplications(standardsCodes, installedAppsIds, start, count);
            }
        }
    }

    public  enum AppSortingMode
    {
        Popular = 1,
        Newest = 2,
        HighestRated = 3
    }

    public  enum AppFilterMode
    {
        All = 1,
        Paid = 2,
        Free = 3,
    }

}