using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationService
    {
        IList<AppPermissionType> GetPermisions(string applicationUrl);
        PaginatedList<Application> GetSysAdminApplications(Guid? developerId, ApplicationStateEnum? state, AppSortingMode? sorting, string filter, int start = 0, int count = int.MaxValue);
        PaginatedList<Application> GetDeveloperApplications(bool? live = false);

        PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords
            , AppFilterMode? filterMode = null, AppSortingMode? sortingMode = null, int start = 0, int count = int.MaxValue);

        IList<Application> GetApplicationsByIds(IList<Guid> ids);
        Application GetApplicationById(Guid id);
        Application GetApplicationByUrl(string url);
        ApplicationRating WriteReview(Guid applicationId, int rating, string review);
        bool ExistsReview(Guid applicationId);
        IList<ApplicationRating> GetRatings(Guid applicationId);
        bool CanGetSecretKey(IList<Application> applications);
        bool HasMyApps(Application application);

        Application GetPracticeGradesApplication();
        Application GetAssessmentApplication();

        IList<Application> GetSuggestedApplications(IList<Guid> abIds, IList<Guid> installedAppsIds, int start, int count);
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

        public PaginatedList<Application> GetSysAdminApplications(Guid? developerId, ApplicationStateEnum? state, AppSortingMode? sorting, string filter, int start = 0, int count = Int32.MaxValue)
        {
            var onlyWithLive = state == ApplicationStateEnum.Live;
            sorting = sorting ?? AppSortingMode.Newest;
            var query = new ApplicationQuery
            {
                UserId = Context.UserId,
                DeveloperId = developerId,
                State = onlyWithLive ? null : state,
                Filter = filter,
                OrderBy = MapSortingModeToColumnName(sorting.Value),
                OrderByDesc = sorting != AppSortingMode.Name
            };
            var apps = DoRead(uow=> new SysAdminApplicationDataAccess(uow).GetPaginatedApplications(query))
                        .Where(x=> x.State != ApplicationStateEnum.Live).ToList();
            if (onlyWithLive)
                apps = apps.Where(a => a.OriginalRef.HasValue).ToList();
            var appsLiveIds = apps.Where(x => x.OriginalRef.HasValue).Select(x => x.OriginalRef.Value).ToList();
            if (appsLiveIds.Count > 0)
            {
               var liveApps = GetApplicationsByIds(appsLiveIds);
               foreach (var app in apps)
               {
                   if(app.OriginalRef.HasValue)
                        app.LiveApplication = liveApps.First(x => x.Id == app.OriginalRef);
               }
            }
            return new PaginatedList<Application>(apps, start / count, count);
        }
        
        public PaginatedList<Application> GetDeveloperApplications(bool? live = false)
        {
            var apps = GetApplications(new ApplicationQuery()).ToList();
            if (live.HasValue)
                apps = live.Value ? apps.Where(x => x.IsLive).ToList() : apps.Where(x => !x.IsLive).ToList();
            return new PaginatedList<Application>(apps, 0, int.MaxValue);
        }

        private string MapSortingModeToColumnName(AppSortingMode sortingMode)
        {
            switch (sortingMode)
            {
                case AppSortingMode.Newest:
                    return Application.CREATE_DATE_TIME_FIELD;
                case AppSortingMode.Name:
                    return Application.NAME_FIELD;
                case AppSortingMode.HighestRated:
                    return Application.AVG_FIELD;
                case AppSortingMode.Updated:
                    return Application.UPDATE_DATE_TIME_FIELD;
                default: 
                    return Application.CREATE_DATE_TIME_FIELD;
            }
        }

        private PaginatedList<Application> GetApplications(ApplicationQuery query)
        {
            using (var uow = Read())
            {
                query.UserId = Context.UserId;
                if(!BaseSecurity.IsSysAdmin(Context))
                    query.DeveloperId = Context.DeveloperId;
                return CreateApplicationDataAccess(uow).GetPaginatedApplications(query);
            }
        }

        public Application GetApplicationById(Guid id)
        {
            if (id == Guid.Parse(PreferenceService.Get(Preference.PRACTICE_APPLICATION_ID).Value))
                return GetPracticeGradesApplication();
            if (id == Guid.Parse(PreferenceService.Get(Preference.ASSESSMENT_APLICATION_ID).Value))
                return GetAssessmentApplication();
            using (var uow = Read())
            {
                return CreateApplicationDataAccess(uow)
                    .GetApplication(new ApplicationQuery
                        {
                            Id = id,
                            UserId = Context.UserId,
                        });
            }
        }

        private ApplicationDataAccess CreateApplicationDataAccess(UnitOfWork uow)
        {
            if (BaseSecurity.IsSysAdmin(Context))
                return new SysAdminApplicationDataAccess(uow);
            if (Context.Role == CoreRoles.DEVELOPER_ROLE)
                return new DeveloperApplicationDataAccess(uow);
            if (Context.DeveloperId.HasValue)
                return new DemoPersonApplicationDataAccess(uow);
            return new PersonApplicationDataAccess(uow);
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


        public PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords
            , AppFilterMode? filterMode, AppSortingMode? sortingMode, int start = 0, int count = int.MaxValue)
        {
            var query = new ApplicationQuery
                {
                    CategoryIds = categoriesIds,
                    GradeLevels = gradeLevels,
                    Filter = filterWords,
                    Free = filterMode == AppFilterMode.Free,
                    OrderBy = MapSortingModeToColumnName(sortingMode ?? AppSortingMode.Newest),
                    Start = start,
                    Count = count,
                };
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
                return new ApplicationDataAccess(uow).GetByIds(ids);
            }
        }


        public IList<Application> GetSuggestedApplications(IList<Guid> abIds, IList<Guid> installedAppsIds, int start, int count)
        {
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow).GetSuggestedApplications(abIds, installedAppsIds, start, count);
            }
        }

        public Application GetPracticeGradesApplication()
        {
            var appId = Guid.Parse(PreferenceService.Get(Preference.PRACTICE_APPLICATION_ID).Value);
            return DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(appId));
        }


        public Application GetAssessmentApplication()
        {
            var appId = Guid.Parse(PreferenceService.Get(Preference.ASSESSMENT_APLICATION_ID).Value);
            return DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(appId));
        }
    }

    public  enum AppSortingMode
    {
        Popular = 1,
        Newest = 2,
        HighestRated = 3,
        Name = 4,
        Updated = 5,
    }

    public  enum AppFilterMode
    {
        All = 1,
        Paid = 2,
        Free = 3,
    }

}