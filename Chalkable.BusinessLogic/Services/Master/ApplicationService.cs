using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
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
        PaginatedList<Application> GetApplicationsWithLive(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0, int count = int.MaxValue);
        PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, AppFilterMode? filterMode
            , AppSortingMode? sortingMode, int start = 0, int count = int.MaxValue);

        IList<Application> GetApplicationsByIds(IList<Guid> ids);
        Application GetApplicationById(Guid id);
        Application GetApplicationByUrl(string url);
        ApplicationRating WriteReview(Guid applicationId, int rating, string review);
        bool ReviewExists(Guid applicationId);
        IList<ApplicationRating> GetRatings(Guid applicationId);
        bool CanGetSecretKey(IList<Application> applications);
        bool HasMyApps(Application application);
        bool HasExternalAttachMode(Application application);
        Application GetMiniQuizAppication();
        Application GetAssessmentApplication();
        Guid? GetMiniQuizAppicationId();
        Guid? GetAssessmentId();
        IList<Application> GetSuggestedApplications(IList<Guid> abIds, IList<Guid> installedAppsIds, int start, int count);
        void SetApplicationDistrictOptions(Guid applicationId, Guid districtId, bool ban);
    }


    public class ApplicationService : MasterServiceBase, IApplicationService
    {
        public ApplicationService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }
        
        public IList<AppPermissionType> GetPermisions(string applicationUrl)
        {
            var app = DoRead(u => new ApplicationDataAccess(u).GetLiveApplicationByUrl(applicationUrl));
            return app.Permissions.Select(x => x.Permission).ToList();
        }

        public PaginatedList<Application> GetApplicationsWithLive(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0, int count = Int32.MaxValue)
        {
            var onlyWithLive = state == ApplicationStateEnum.Live;
            var query = new ApplicationQuery
            {
                DeveloperId = developerId,
                State = onlyWithLive ? null : state,
                Filter = filter
            };
            var apps = GetApplications(query).Where(x=> x.State != ApplicationStateEnum.Live).ToList();
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
                if (!BaseSecurity.IsSysAdmin(Context))
                {
                    query.DistrictId = Context.DistrictId;
                    query.DeveloperId = Context.DeveloperId;
                    if (!BaseSecurity.IsDistrictAdmin(Context))
                        query.Ban = false;
                }
                return new ApplicationDataAccess(uow).GetPaginatedApplications(query);
            }
        }
 
        
        public Application GetApplicationById(Guid id)
        {
            if (id == InternalGetAssessmentId())
                return GetAssessmentApplication();

            if (id == GetMiniQuizAppicationId())
                return GetMiniQuizAppication();

            var q = new ApplicationQuery
            {
                Id = id,
                UserId = Context.UserId,
                Role = Context.Role.Id,
                OnlyForInstall = false
            };
            if (!BaseSecurity.IsSysAdmin(Context))
                q.DistrictId = Context.DistrictId;

            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow)
                    .GetApplication(q);
            }
        }

        //TODO: security
        public Application GetApplicationByUrl(string url)
        {
            return DoRead(u => new ApplicationDataAccess(u).GetLiveApplicationByUrl(url));
        }

        public ApplicationRating WriteReview(Guid applicationId, int rating, string review)
        {
            using (var uow = Update())
            {
                var da = new ApplicationRatingDataAccess(uow);
                var res = da.GetAll(new AndQueryCondition
                    {
                        {nameof(ApplicationRating.ApplicationRef), applicationId},
                        {nameof(ApplicationRating.UserRef), Context.UserId}
                    }).FirstOrDefault();
                Action<ApplicationRating> modifyAction = da.Update;
                if (res == null)
                {
                    res = new ApplicationRating
                        {
                            Id = Guid.NewGuid(),
                            ApplicationRef = applicationId,
                            UserRef = Context.UserId 
                        };
                    modifyAction = da.Insert;
                }
                res.Rating = rating;
                res.Review = review;
                modifyAction(res);
                uow.Commit();
                return res;
            }
        }

        public IList<ApplicationRating> GetRatings(Guid applicationId)
        {
            var query = new AndQueryCondition {{nameof(ApplicationRating.ApplicationRef), applicationId}};
            return DoRead(u => new ApplicationRatingDataAccess(u).GetAll(query));
        }

        //TODO: can we apply thin on service leyer get methods?
        public bool CanGetSecretKey(IList<Application> applications)
        {
            return applications.All(x => x.DeveloperRef == Context.UserId);
        }

        public bool HasMyApps(Application application)
        {
            if (BaseSecurity.IsDistrictAdmin(ServiceLocator.Context))
                return application.HasAdminMyApps;
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                return application.HasTeacherMyApps;
            return Context.Role.Id == CoreRoles.STUDENT_ROLE.Id && application.HasStudentMyApps;
        }

        public bool HasExternalAttachMode(Application application)
        {
            return BaseSecurity.IsDistrictAdmin(ServiceLocator.Context) && application.HasAdminExternalAttach
                   || Context.Role == CoreRoles.TEACHER_ROLE && application.HasTeacherExternalAttach
                   || Context.Role == CoreRoles.STUDENT_ROLE && application.HasStudentExternalAttach;
        }


        public PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, AppFilterMode? filterMode, AppSortingMode? sortingMode, int start = 0, int count = int.MaxValue)
        {
            var query = new ApplicationQuery
                {
                    CategoryIds = categoriesIds,
                    GradeLevels = gradeLevels,
                    Filter = filterWords,
                    Start = start,
                    Count = count,
                    Live = true
                };
            filterMode = filterMode ?? AppFilterMode.All;
            sortingMode = sortingMode ?? AppSortingMode.Newest;
            if (filterMode != AppFilterMode.All)
                query.Free = filterMode == AppFilterMode.Free;
            switch (sortingMode)
            {
                case AppSortingMode.Newest:
                    query.OrderBy = nameof(Application.CreateDateTime);
                    break;
                case AppSortingMode.HighestRated:
                    query.OrderBy = Application.AVG_FIELD;
                    break;
            }
            return GetApplications(query);
        }


        public bool ReviewExists(Guid applicationId)
        {
            return DoRead(u => new ApplicationRatingDataAccess(u).Exists(applicationId, Context.UserId));
        }
        public IList<Application> GetApplicationsByIds(IList<Guid> ids)
        {
            return DoRead(u => new ApplicationDataAccess(u).GetByIds(ids));
        }
        public IList<Application> GetSuggestedApplications(IList<Guid> abIds, IList<Guid> installedAppsIds, int start, int count)
        {
            return DoRead(u => new ApplicationDataAccess(u).GetSuggestedApplications(abIds, installedAppsIds, start, count));
        }
        public void SetApplicationDistrictOptions(Guid applicationId, Guid districtId, bool ban)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(uow =>
            {
                new ApplicationDataAccess(uow).SetDistrictOption(applicationId, districtId, ban);
            });
        }
        public Application GetMiniQuizAppication()
        {
            var id = GetMiniQuizAppicationId();
            return !id.HasValue ? null : DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(id.Value));
        }
        public Application GetAssessmentApplication()
        {
            var id = GetAssessmentId();
            return !id.HasValue ? null : DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(id.Value));
        }

        public Guid? GetMiniQuizAppicationId()
        {
            var id = PreferenceService.Get(Context.AssessmentEnabled ? Preference.NEW_ASSESSMENT_APLICATION_ID : Preference.PRACTICE_APPLICATION_ID).Value;
            Guid res;
            return Guid.TryParse(id, out res) ? res : (Guid?)null;
        }
        public Guid? GetAssessmentId()
        {
            return InternalGetAssessmentId();
        }

        private Guid? InternalGetAssessmentId()
        {
            var id = PreferenceService.Get(Context.AssessmentEnabled ? Preference.NEW_ASSESSMENT_APLICATION_ID : Preference.ASSESSMENT_APLICATION_ID).Value;
            Guid res;
            return Guid.TryParse(id, out res) ? res : (Guid?)null;
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