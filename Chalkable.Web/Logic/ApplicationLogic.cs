using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Logic
{
    public class ApplicationLogic
    {

        public static IList<AnnouncementApplicationViewData> PrepareAnnouncementApplicationInfo(IServiceLocatorSchool schoolLocator, IServiceLocatorMaster masterLocator, int announcementId)
        {
            var applications = masterLocator.ApplicationService.GetApplications(0, int.MaxValue, null, false);
            var assessmentApp = masterLocator.ApplicationService.GetAssessmentApplication();
            if(assessmentApp != null && applications.All(x=>x.Id != assessmentApp.Id))
                applications.Add(assessmentApp);
            var annApps = schoolLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(announcementId, true);
            var installs = schoolLocator.AppMarketService.ListInstalledAppInstalls(schoolLocator.Context.PersonId ?? 0);
            return AnnouncementApplicationViewData.Create(annApps, applications, installs, schoolLocator.Context.PersonId);
        } 

        public static IList<InstalledForPersonsGroupViewData> PrepareInstalledForPersonGroupData(
            IServiceLocatorSchool schoolLocator, IServiceLocatorMaster maseterLocator, Application application)
        {
            IList<InstalledForPersonsGroupViewData> res = new List<InstalledForPersonsGroupViewData>();
            if (maseterLocator.Context.Role == CoreRoles.TEACHER_ROLE || maseterLocator.Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE)
            {
                if (!schoolLocator.Context.SchoolYearId.HasValue)
                    throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);

                var studentCountToAppInstall = schoolLocator.AppMarketService.GetStudentCountToAppInstallByClass(schoolLocator.Context.SchoolYearId.Value, application.Id);
                bool isPersonForInstall = studentCountToAppInstall.Sum(x => x.NotInstalledStudentCount) > 0;
                res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.All, null, "All", !isPersonForInstall));
                Trace.Assert(schoolLocator.Context.PersonId.HasValue);
                IList<ClassDetails> classes;
                if (maseterLocator.Context.Role == CoreRoles.TEACHER_ROLE)
                    classes = schoolLocator.ClassService.GetTeacherClasses(schoolLocator.Context.SchoolYearId.Value,
                        schoolLocator.Context.PersonId.Value);
                else
                    classes = schoolLocator.ClassService.GetAllSchoolsActiveClasses();
                
                foreach (var clazz in classes)
                {
                    var studentCountView = studentCountToAppInstall.FirstOrDefault(x => x.ClassId == clazz.Id);
                    var installed = studentCountView == null || studentCountView.NotInstalledStudentCount == 0;
                    res.Add(InstalledForPersonsGroupViewData
                        .Create(InstalledForPersonsGroupViewData.GroupTypeEnum.Class, clazz.Id.ToString(), clazz.Name, installed));
                    
                }
            }
            return res;
        }


        public static IList<ApplicationForAttachViewData> GetSuggestedAppsForAttach(IServiceLocatorMaster masterLocator, IServiceLocatorSchool schooLocator
            , int personId, int classId, IList<Guid> abIds, int markingPeriodId, int? start = null, int? count = null)
        {
            start = start ?? 0;
            var studentCountPerApp = schooLocator.AppMarketService.GetNotInstalledStudentCountPerApp(personId, classId, markingPeriodId);
            var installedAppsIds = studentCountPerApp.Select(x => x.Key).Distinct().ToList();
            var applications = masterLocator.ApplicationService.GetSuggestedApplications(abIds, installedAppsIds, 0, int.MaxValue);
            applications = applications.Where(a => a.CanAttach).ToList();
            if(count != null)
                applications = applications.Skip(start.Value).Take(count.Value).ToList();
                                        
            var classSize = schooLocator.ClassService.GetClassPersons(null, classId, true, markingPeriodId).Count;
            foreach (var application in applications)
            {
                if (!studentCountPerApp.ContainsKey(application.Id))
                    studentCountPerApp.Add(application.Id, classSize);
            }
            return ApplicationForAttachViewData.Create(applications, studentCountPerApp);
        }

    }
}