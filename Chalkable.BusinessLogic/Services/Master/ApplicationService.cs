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
        PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null, bool? canAttach = null);
        PaginatedList<Application> GetApplicationsWithLive(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0, int count = int.MaxValue);
        PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, int start = 0, int count = int.MaxValue, bool? myApps = null, bool withBanned = false);
        IList<Application> GetApplicationsByIds(IList<Guid> ids);
        Application GetApplicationById(Guid id);
        Application GetApplicationByUrl(string url);
        bool CanGetSecretKey(IList<Application> applications);
        bool HasMyApps(Application application);
        bool HasExternalAttachMode(Application application);
        Application GetMiniQuizAppication();
        Application GetAssessmentApplication();
        Guid? GetMiniQuizAppicationId();
        Guid? GetAssessmentId();
        IList<Application> GetSuggestedApplications(IList<Guid> abIds, int start, int count);
        IList<ApplicationBanInfo> GetApplicationBanInfos(Guid districtId, Guid? schoolId, IList<Guid> applicationIds);
        void SubmitApplicationBan(Guid applicationId, IList<Guid> schoolIds);
        IList<ApplicationSchoolBan> GetApplicationSchoolBans(Guid districtId, Guid applicationId);
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
                Filter = filter,
                Ban = !BaseSecurity.IsSysAdminOrDeveloper(Context) ? false : (bool?)null
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

        public PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null, bool? canAttach = null)
        {
            var query = new ApplicationQuery
                {
                    Start = start, 
                    Count = count, 
                    Live = live, 
                    CanAttach = canAttach,
                    Ban = !BaseSecurity.IsSysAdminOrDeveloper(Context) ? false : (bool?)null
                };
            return GetApplications(query);
        }

        private PaginatedList<Application> GetApplications(ApplicationQuery query)
        {
            using (var uow = Read())
            {
                query.Role = Context.Role.Id;
                if (!BaseSecurity.IsSysAdmin(Context))
                {
                    query.SchoolId = Context.SchoolId;
                    query.DeveloperId = Context.DeveloperId;
                    if(!ApplicationSecurity.HasAccessToBannedApps(Context))
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
                Role = Context.Role.Id
            };
            if (!BaseSecurity.IsSysAdmin(Context))
            {
                q.SchoolId = Context.SchoolId;
            }

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


        public PaginatedList<Application> GetApplications(IList<Guid> categoriesIds, IList<int> gradeLevels, string filterWords, int start = 0, int count = int.MaxValue, bool? myApps = null, bool withBanned = false)
        {
            var query = new ApplicationQuery
                {
                    CategoryIds = categoriesIds,
                    GradeLevels = gradeLevels,
                    Filter = filterWords,
                    Start = start,
                    Count = count,
                    Live = true,
                    Ban = !withBanned && !BaseSecurity.IsSysAdminOrDeveloper(Context) ? false : (bool?)null
                };
             return GetApplications(query);
        }


        public IList<Application> GetApplicationsByIds(IList<Guid> ids)
        {
            return DoRead(u => new ApplicationDataAccess(u).GetByIds(ids));
        }
        public IList<Application> GetSuggestedApplications(IList<Guid> abIds, int start, int count)
        {
            return DoRead(u => new ApplicationDataAccess(u).GetSuggestedApplications(abIds, start, count));
        }

        public IList<ApplicationBanInfo> GetApplicationBanInfos(Guid districtId, Guid? schoolId, IList<Guid> applicationIds)
        {
            return DoRead(u => new ApplicationSchoolOptionDataAccess(u).GetApplicationBanInfos(districtId, schoolId, applicationIds));
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
            var key = Preference.ASSESSMENT_APLICATION_ID;

            Guid res;
            return Guid.TryParse(PreferenceService.Get(key).Value, out res) ? res : (Guid?)null;
        }
        public Guid? GetAssessmentId()
        {
            return InternalGetAssessmentId();
        }

        private Guid? InternalGetAssessmentId()
        {
            var key = Preference.ASSESSMENT_APLICATION_ID;

            Guid res;
            return Guid.TryParse(PreferenceService.Get(key).Value, out res) ? res : (Guid?)null;
        }

        public void SubmitApplicationBan(Guid applicationId, IList<Guid> schoolIds)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new ApplicationSchoolOptionDataAccess(u).BanSchoolsByIds(applicationId, schoolIds));
        }

        public IList<ApplicationSchoolBan> GetApplicationSchoolBans(Guid districtId, Guid applicationId)
        {
            return DoRead(u => new ApplicationSchoolOptionDataAccess(u).GetApplicationSchoolBans(districtId, applicationId));
        }
    }

    public  enum AppSortingMode
    {
        Popular = 1,
        Newest = 2,
        HighestRated = 3
    }
}