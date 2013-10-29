﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Logic
{
    public class ApplicationLogic
    {
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
                    var c = counts.FirstOrDefault(x => x.Type == PersonsFroAppInstallTypeEnum.Role && x.GroupId == groupId);
                    var installed = c == null || c.Count == 0;
                    res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.Role, groupId,
                        role.Name, installed));
                }
                foreach (var department in departments)
                {
                    var groupId = department.Id.ToString();
                    var c = counts.FirstOrDefault(x => x.Type == PersonsFroAppInstallTypeEnum.Department && x.GroupId == groupId);
                    var installed = c == null || c.Count == 0;
                    res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.Department, groupId,
                        department.Name, installed));
                }
                foreach (var gradeLevel in gradeLevels)
                {
                    var groupId = gradeLevel.Id.ToString();
                    var c = counts.FirstOrDefault(x => x.Type == PersonsFroAppInstallTypeEnum.GradeLevel && x.GroupId == groupId);
                    var installed = c == null || c.Count == 0;
                    res.Add(InstalledForPersonsGroupViewData.Create(InstalledForPersonsGroupViewData.GroupTypeEnum.GradeLevel, groupId,
                        gradeLevel.Name, installed));
                }
            }
            if (maseterLocator.Context.Role == CoreRoles.TEACHER_ROLE)
            {
                var schoolYear = schoolLocator.SchoolYearService.GetCurrentSchoolYear();
                var classes = schoolLocator.ClassService.GetClasses(schoolYear.Id, null, schoolLocator.Context.UserLocalId);
                var studentCountToAppInstall = schoolLocator.AppMarketService.GetStudentCountToAppInstallByClass(schoolYear.Id, application.Id);
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
    }
}