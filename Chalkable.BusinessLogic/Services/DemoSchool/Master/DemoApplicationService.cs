using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoApplicationService : DemoMasterServiceBase, IApplicationService
    {

        public DemoApplicationService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public PaginatedList<Application> GetApplicationsWithLive(Guid? developerId, ApplicationStateEnum? state, string filter, int start = 0,
                                                     int count = Int32.MaxValue)
        {
            throw new NotImplementedException();
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
                Ban = !withBanned && !BaseSecurity.IsSysAdminOrDeveloper(Context) ? false : (bool?)null
            };
            return GetApplications(query);
        }


        public IList<AppPermissionType> GetPermisions(string applicationUrl)
        {
            var app = GetApplicationByUrl(applicationUrl);
            return app.Permissions.Select(x => x.Permission).ToList();
        }

        public PaginatedList<Application> GetApplications(int start = 0, int count = int.MaxValue, bool? live = null, bool? canAttach = null)
        {
            var query = new ApplicationQuery
                {
                    Start = start,
                    Count = count,
                    Live = live,
                    CanAttach = canAttach
                };
            return GetApplications(query);
        }

        private PaginatedList<Application> GetApplications(ApplicationQuery query)
        {
            using (var uow = Read())
            {
                query.Role = Context.Role.Id;
                query.DeveloperId = Context.DeveloperId;
                return new ApplicationDataAccess(uow).GetPaginatedApplications(query);
            }
        }


        public Application GetApplicationById(Guid id)
        {
            if (id == GetMiniQuizAppicationId())
                return GetMiniQuizAppication();
            if (id == GetAssessmentId())
                return GetAssessmentApplication();
            
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow)
                    .GetApplication(new ApplicationQuery
                    {
                        Id = id,
                        Role = Context.Role.Id,
                        DeveloperId = Context.DeveloperId
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

        public IList<Application> GetSuggestedApplications(IList<Guid> abIds, int start, int count)
        {
            using (var uow = Read())
            {
                var apps =new  ApplicationDataAccess(uow).GetSuggestedApplications(abIds, start, count);
                apps = apps.Where(x => x.DeveloperRef == Context.DeveloperId.Value).ToList();
                return apps;
            }
        }
        public IList<ApplicationBanInfo> GetApplicationBanInfos(Guid districtId, Guid? schoolId, IList<Guid> applicationIds)
        {
            throw new NotImplementedException();
        }

        public void SubmitApplicationBan(Guid applicationId, IList<Guid> schoolIds)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationSchoolBan> GetApplicationSchoolBans(Guid districtId, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public string GetAccessToken(Guid applicationId, string session)
        {
            throw new NotImplementedException();
        }


        public IList<Application> GetApplicationsByIds(IList<Guid> ids)
        {
            using (var uow = Read())
            {
                return new ApplicationDataAccess(uow).GetByIds(ids);
            }
        }

        public Application GetMiniQuizAppication()
        {
            var id = GetMiniQuizAppicationId();
            if (!id.HasValue) return null;
            return DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(id.Value));
        }


        public Application GetAssessmentApplication()
        {
            var id = GetAssessmentId();
            if (!id.HasValue) return null;
            return DoRead(uow => new ApplicationDataAccess(uow).GetApplicationById(id.Value));
        }

        public Guid? GetMiniQuizAppicationId()
        {
            var key = ApplicationSecurity.HasAssessmentEnabled(Context)
                    ? Preference.ASSESSMENT_APLICATION_ID
                    : null;

            Guid res;
            return key != null ? (Guid.TryParse(PreferenceService.Get(key).Value, out res) ? res : (Guid?)null) : null;
        }
        public Guid? GetAssessmentId()
        {
            var id = PreferenceService.Get(Context.AssessmentEnabled || Context.SCEnabled ? Preference.ASSESSMENT_APLICATION_ID : null).Value;
            Guid res;
            return Guid.TryParse(id, out res) ? res : (Guid?)null;
        }
    }

}