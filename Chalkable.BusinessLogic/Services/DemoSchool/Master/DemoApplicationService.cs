using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoApplicationRatingStorage : BaseDemoGuidStorage<ApplicationRating>
    {
        public DemoApplicationRatingStorage()
            : base(x => x.Id)
        {
        }

        public bool Exists(Guid applicationId, Guid userId)
        {
            return data.Count(x => x.Value.ApplicationRef == applicationId && x.Value.UserRef == userId) == 1;
        }

        public IList<ApplicationRating> GetAll(Guid applicationId)
        {
            return data.Where(x => x.Value.ApplicationRef == applicationId).Select(x => x.Value).ToList();
        }
    }

    public class DemoApplicationService : DemoMasterServiceBase, IApplicationService
    {
        private DemoApplicationRatingStorage ApplicationRatingStorage { get; set; }
        public DemoApplicationService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            ApplicationRatingStorage = new DemoApplicationRatingStorage();
        }

        public ApplicationRating WriteReview(Guid applicationId, int rating, string review)
        {
            if (ReviewExists(applicationId))
                throw new ChalkableException("User can send only one review per application");

            var user = DemoUserService.CreateDemoUser(Context.DistrictId.Value, Context.UserId, Context.Login);
            user.FullName = ""; //ServiceLocator.Personsc.GetPerson(Context.PersonId.Value).FullName();
            ServiceLocator.UserService.Add(new List<User>{user});

            var appRating = new ApplicationRating
            {
                Id = Guid.NewGuid(),
                ApplicationRef = applicationId,
                Rating = rating,
                Review = review,
                UserRef = Context.UserId,
                User = user
            };
            ApplicationRatingStorage.Add(appRating);
            return appRating;
        }

        public IList<ApplicationRating> GetRatings(Guid applicationId)
        {
            return ApplicationRatingStorage.GetAll(applicationId);
        }

        public PaginatedList<Application> GetApplications(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0, int count = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<Application> GetApplicationsWithLive(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0,
                                                     int count = Int32.MaxValue)
        {
            throw new NotImplementedException();
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


        public bool ReviewExists(Guid applicationId)
        {
            return ApplicationRatingStorage.Exists(applicationId, Context.UserId);
        }

        public IList<AppPermissionType> GetPermisions(string applicationUrl)
        {
            var app = GetApplicationByUrl(applicationUrl);
            return app.Permissions.Select(x => x.Permission).ToList();
        }

        public PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null, bool onlyForInstall = true)
        {
            var query = new ApplicationQuery
                {
                    Start = start,
                    Count = count,
                    Live = live,
                    OnlyForInstall = onlyForInstall
                };
            return GetApplications(query);
        }

        private PaginatedList<Application> GetApplications(ApplicationQuery query)
        {
            using (var uow = Read())
            {
                query.UserId = Context.UserId;
                query.Role = Context.Role.Id;
                query.DeveloperId = Context.DeveloperId;
                return new ApplicationDataAccess(uow).GetPaginatedApplications(query);
            }
        }


        public Application GetApplicationById(Guid id)
        {
            if (id == GetPracticeGradeId())
                return GetPracticeGradesApplication();
            if (id == GetAssessmentId())
                return GetAssessmentApplication();
            
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow)
                    .GetApplication(new ApplicationQuery
                    {
                        Id = id,
                        UserId = Context.UserId,
                        Role = Context.Role.Id,
                        DeveloperId = Context.DeveloperId,
                        OnlyForInstall = false
                    });
            }
        }

        //TODO: security
        public Application GetApplicationByUrl(string url)
        {
            using (var uow = Read())
            {
                var da = new ApplicationDataAccess(uow);
                return url == Settings.ApiExplorerClientId 
                    ? da.GetLiveApplicationByUrl(url) 
                    : da.GetDraftApplicationByUrl(url);
            }
        }

        //TODO: do we ned this?
        public bool CanGetSecretKey(IList<Application> applications)
        {
            if (Context.Role.Id == CoreRoles.STUDENT_ROLE.Id)//WHY???
                return true;
            if (Context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id)
            {
                var developer = ServiceLocator.DeveloperService.GetById(Context.UserId);
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

        public IList<Application> GetSuggestedApplications(IList<Guid> abIds, IList<Guid> installedAppsIds, int start, int count)
        {
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow).GetSuggestedApplications(abIds, installedAppsIds, start, count);
            }
        }


        public IList<Application> GetApplicationsByIds(IList<Guid> ids)
        {
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow).GetByIds(ids);
            }
        }

        public Application GetPracticeGradesApplication()
        {
            var id = GetPracticeGradeId();
            if (!id.HasValue) return null;
            return DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(id.Value));
        }


        public Application GetAssessmentApplication()
        {
            var id = GetAssessmentId();
            if (!id.HasValue) return null;
            return DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(id.Value));
        }

        public Guid? GetPracticeGradeId()
        {
            var id = PreferenceService.Get(Preference.PRACTICE_APPLICATION_ID).Value;
            Guid res;
            return Guid.TryParse(id, out res) ? res : (Guid?)null;
        }
        public Guid? GetAssessmentId()
        {
            var id = PreferenceService.Get(Preference.ASSESSMENT_APLICATION_ID).Value;
            Guid res;
            return Guid.TryParse(id, out res) ? res : (Guid?)null;
        }
    }

}