﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.SchoolsViewData;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    public class HomeController : ChalkableController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOn(string userName, string password, bool remember)
        {
            var context = LogOn(remember, us => us.Login(userName, password));
            if(context != null)
                return Json(new { Success = true, data = new {Role = context.Role.LoweredName} }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = false, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            var userName = ControllerContext.HttpContext.User.Identity.Name;
            ChalkableAuthentication.SignOut();
            return Json(new { success = true, data = new {success = true} }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Confirm(string key)
        {
            return Confirm(key, AfterConfirmAction);
        }
        private ActionResult AfterConfirmAction(UserContext context)
        {
            //TODO: create default Announcement for teacher
            SchoolLocator.PersonService.ActivatePerson(context.UserId);
            //TODO: mix panel 
            if (context.Role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin());
            if (context.Role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher(true));
            return Redirect<HomeController>(c => c.Index());
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult SysAdmin()
        {
            var sysUser = MasterLocator.UserService.GetById(Context.UserId);
            PrepareJsonData(SysAdminViewData.Create(sysUser), ViewConstants.CURRENT_PERSON);
            return View();
        }


        [AuthorizationFilter("Developer")]
        public ActionResult Developer(Guid? currentApplicationId)
        {
            var prefDemoSchool = MasterLocator.SchoolService.GetById(Context.SchoolId.Value).DemoPrefix;
            var developer = MasterLocator.DeveloperService.GetDeveloperById(MasterLocator.Context.UserId);
            ViewData[ViewConstants.IS_DEV] = true;
            PrepareJsonData(DeveloperViewData.Create(developer), ViewConstants.CURRENT_PERSON);
            var applications = MasterLocator.ApplicationService.GetApplications(0, int.MaxValue, false);
            if (applications.Count == 0)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = UrlsConstants.DEV_APP_INFO_URL;
            }
            ViewData[ViewConstants.NEEDS_TOUR] = false;
            PrepareCommonViewData(prefDemoSchool);
            PrepareJsonData(BaseApplicationViewData.Create(applications), ViewConstants.APPLICATIONS);
            if (applications.Count > 0)
            {
                var app = currentApplicationId.HasValue ? applications.First(x => x.Id == currentApplicationId) : applications.Last();
                app = MasterLocator.ApplicationService.GetApplicationById(app.Id);
                var res = ApplicationController.PrepareAppInfo(MasterLocator, app, true, true);
                PrepareJsonData(res, ViewConstants.DEFAULT_APPLICATION, 6);
            }
            //TODO: mix panel
            return View();
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Teacher(bool? redirectToSetup)
        {
            if (redirectToSetup.HasValue && redirectToSetup.Value)
                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.SETUP_URL_FORMAT, Context.UserId);

            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            PrepareTeacherJsonData(mp, false);
            return View();
        }

        private void PrepareCommonViewData(string prefixDemoSchool = null, MarkingPeriod markingPeriod = null)
        {
            //TODO: render data for demo school 
            if (Context.SchoolId.HasValue)
            {
                var school = MasterLocator.SchoolService.GetById(Context.SchoolId.Value);
                PrepareJsonData(ShortSchoolViewData.Create(school), ViewConstants.SCHOOL);
                if (!string.IsNullOrEmpty(school.DemoPrefix) && !string.IsNullOrEmpty(prefixDemoSchool))
                {
                    ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
                    ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
                }
            }
            ViewData[ViewConstants.NEEDS_TOUR] = false; //TODO : tour 
            ViewData[ViewConstants.CURR_SCHOOL_YEAR_ID] = GetCurrentSchoolYearId();
            ViewData[ViewConstants.VERSION] = CompilerHelper.Version;
            ViewData[ViewConstants.CROCODOC_API_URL] = PreferenceService.Get(Preference.CROCODOC_URL).Value;

            ViewData[ViewConstants.UNSHOWN_NOTIFICATIONS_COUNT] = SchoolLocator.NotificationService.GetUnshownNotifications().Count;
            if (markingPeriod != null && SchoolLocator.Context.SchoolId.HasValue)
            {
                PrepareJsonData(MarkingPeriodViewData.Create(markingPeriod), ViewConstants.MARKING_PERIOD);
                var nextmp = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
                if (nextmp != null)
                    PrepareJsonData(MarkingPeriodViewData.Create(nextmp), ViewConstants.NEXT_MARKING_PERIOD);
            }
        }
        
        private void PrepareTeacherJsonData(MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserId);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.ShortSalutationName;
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);

            var finalizedClasses = SchoolLocator.FinalGradeService.GetFinalizedClasses(mp.Id);
            PrepareJsonData(finalizedClasses.Select(x => x.Id), ViewConstants.FINALIZED_CLASSES_IDS);

            var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, null, SchoolLocator.Context.UserId);
            var now = SchoolLocator.Context.NowSchoolTime;
            if (classes.Count > 0)
            {
                MarkingPeriod currentMp = mp;
                if(mp.StartDate > now || mp.EndDate < now)
                    currentMp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(now);
                if (currentMp != null)
                {
                    var cp = SchoolLocator.ClassPeriodService.GetNearestClassPeriod(null, now);
                    if (cp != null)
                    {
                        var minutes = (int) (now - now.Date).TotalMinutes;
                        if (cp.Period.StartTime - minutes <= 5)
                        {
                            SchoolLocator.CalendarDateService.GetCalendarDateByDate(now);
                            var attQuery = new ClassAttendanceQuery
                                {
                                    MarkingPeriodId = currentMp.Id,
                                    ClassId = cp.ClassRef,
                                    FromTime = cp.Period.StartTime,
                                    ToTime = cp.Period.EndTime,
                                    FromDate = now.Date,
                                    ToDate = now.Date
                                };
                            var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(attQuery);
                            //check is it tour now or demo school
                            if (attendances.Any(x => x.Type == AttendanceTypeEnum.NotAssigned))
                            {
                                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.ATTENDANCE_CLASS_LIST_URL_FORMAT, cp.ClassRef);
                            }
                        }
                    }
                }
            } 
            var executionResult = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(executionResult, ViewConstants.CLASSES);
            PrepareClassesAdvancedData(classes, mp, getAllAnnouncementTypes);
            PrepareCommonViewData(null, mp);
            PrepareJsonData(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
        }

        private void PrepareClassesAdvancedData(IEnumerable<ClassDetails> classDetailses, MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            var classesAdvancedData = new List<object>();
            classDetailses = classDetailses.Where(x => x.MarkingPeriodClasses.Any(y => y.MarkingPeriodRef == mp.Id));
            var classesMaskDic = ClassController.BuildClassesUsageMask(SchoolLocator, mp.Id, SchoolLocator.Context.SchoolTimeZoneId);
            var baseAnnTypes = SchoolLocator.AnnouncementTypeService.GetAnnouncementTypes(null);
            foreach (var classDetails in classDetailses)
            {
                Guid classId = classDetails.Id;
                var typesByClasses = getAllAnnouncementTypes ? baseAnnTypes
                                    : AnnouncementTypeController.GetTypesByClass(SchoolLocator, mp.Id, classId);
                classesAdvancedData.Add(new
                {
                    ClassId = classId,
                    Mask = classesMaskDic.ContainsKey(classId) ? classesMaskDic[classId] : new List<int>(),
                    TypesByClass = AnnouncementTypeViewData.Create(typesByClasses)
                });
            }
            PrepareJsonData(classesAdvancedData, ViewConstants.CLASSES_ADV_DATA);
        }

        //TODO: test only. don't forget to remove :)
        public ActionResult Create(string userName, string password)
        {
            ServiceLocatorFactory.CreateMasterSysAdmin().UserService.CreateSysAdmin(userName, password);
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }        
    }
}
