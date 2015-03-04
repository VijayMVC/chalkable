﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
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
            var applications = masterLocator.ApplicationService.GetApplications(null, null, null);
            var assessmentApp = masterLocator.ApplicationService.GetAssessmentApplication();
            if(applications.All(x=>x.Id != assessmentApp.Id))
                applications.Add(assessmentApp);
            var annApps = schoolLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(announcementId, true);
            var installs = schoolLocator.AppMarketService.ListInstalledAppInstalls(schoolLocator.Context.PersonId ?? 0);
            return AnnouncementApplicationViewData.Create(annApps, applications, installs, schoolLocator.Context.PersonId);
        } 

        public static IList<InstalledForPersonsGroupViewData> PrepareInstalledForPersonGroupData(
            IServiceLocatorSchool schoolLocator, IServiceLocatorMaster maseterLocator, Application application)
        {
            IList<InstalledForPersonsGroupViewData> res = new List<InstalledForPersonsGroupViewData>();
            bool isPersonForInstall = schoolLocator.AppMarketService.IsPersonForInstall(application.Id);
            
            res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.All, null, "All", !isPersonForInstall));
            if (BaseSecurity.IsAdminViewer(maseterLocator.Context))
            {
                var roles = new List<CoreRole>
                    {
                        CoreRoles.ADMIN_GRADE_ROLE,
                        CoreRoles.ADMIN_EDIT_ROLE,
                        CoreRoles.ADMIN_VIEW_ROLE,
                        CoreRoles.TEACHER_ROLE,
                        CoreRoles.STUDENT_ROLE
                    };
                var departments = maseterLocator.ChalkableDepartmentService.GetChalkableDepartments();
                var depIds = departments.Select(x => x.Id).ToList();
                var gradeLevels = schoolLocator.GradeLevelService.GetGradeLevels();
                var glIds = gradeLevels.Select(x => x.Id).ToList();

                var counts = schoolLocator.AppMarketService.GetPersonsForApplicationInstallCount(application.Id, null, roles.Select(x => x.Id).ToList(), null, depIds, glIds);
                foreach (var role in roles)
                {
                    var groupId = role.Id.ToString();
                    var c = counts.FirstOrDefault(x => x.Type == PersonsForAppInstallTypeEnum.Role && x.GroupId == groupId);
                    var installed = c == null || c.Count == 0;
                    res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.Role, groupId,
                        role.Name, installed));
                }
                foreach (var department in departments)
                {
                    var groupId = department.Id.ToString();
                    var c = counts.FirstOrDefault(x => x.Type == PersonsForAppInstallTypeEnum.Department && x.GroupId == groupId);
                    var installed = c == null || c.Count == 0;
                    res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.Department, groupId,
                        department.Name, installed));
                }
                foreach (var gradeLevel in gradeLevels)
                {
                    var groupId = gradeLevel.Id.ToString();
                    var c = counts.FirstOrDefault(x => x.Type == PersonsForAppInstallTypeEnum.GradeLevel && x.GroupId == groupId);
                    var installed = c == null || c.Count == 0;
                    res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.GradeLevel, groupId,
                        gradeLevel.Name, installed));
                }
            }
            if (maseterLocator.Context.Role == CoreRoles.TEACHER_ROLE)
            {
                if (!schoolLocator.Context.SchoolYearId.HasValue)
                    throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
                Trace.Assert(schoolLocator.Context.PersonId.HasValue);
                var classes = schoolLocator.ClassService.GetTeacherClasses(schoolLocator.Context.SchoolYearId.Value, schoolLocator.Context.PersonId.Value);
                var studentCountToAppInstall = schoolLocator.AppMarketService.GetStudentCountToAppInstallByClass(schoolLocator.Context.SchoolYearId.Value, application.Id);
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
            count = count ?? 3;
            var studentCountPerApp = schooLocator.AppMarketService.GetNotInstalledStudentCountPerApp(personId, classId, markingPeriodId);
            var installedAppsIds = studentCountPerApp.Select(x => x.Key).Distinct().ToList();
            var applications = masterLocator.ApplicationService.GetSuggestedApplications(abIds, installedAppsIds, start.Value, count.Value);
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