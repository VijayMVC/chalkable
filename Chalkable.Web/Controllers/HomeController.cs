﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.SchoolsViewData;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class HomeController : ChalkableController
    {
        public ActionResult Index()
        {
            return View();
        }
      
        [AuthorizationFilter("SysAdmin")]
        public ActionResult SysAdmin()
        {
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            var sysUser = MasterLocator.UserService.GetById(Context.UserId);
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            PrepareJsonData(SysAdminViewData.Create(sysUser), ViewConstants.CURRENT_PERSON);
            return View();
        }

        [AuthorizationFilter("Developer")]
        public ActionResult Developer(Guid? currentApplicationId)
        {
            var developer = MasterLocator.DeveloperService.GetDeveloperById(MasterLocator.Context.UserId);
            PrepareJsonData(DeveloperViewData.Create(developer), ViewConstants.CURRENT_PERSON);
            var applications = MasterLocator.ApplicationService.GetApplications(0, int.MaxValue, false);
            if (applications.Count == 0)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = UrlsConstants.DEV_APP_INFO_URL;
            }
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString("yyyy/MM/dd");
            ViewData[ViewConstants.NEEDS_TOUR] = false;
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
            ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
            ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
            ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
            ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
            ViewData[ViewConstants.DEMO_PREFIX_KEY] = Context.UserId.ToString();

            if (Context.DistrictId.HasValue)
            {
                var district = DemoDistrictStorage.CreateDemoDistrict(Context.DistrictId.Value);
                var school = DemoMasterSchoolStorage.CreateMasterSchool(Context.DistrictId.Value);
                school.District = district;
                PrepareJsonData(ShortSchoolViewData.Create(school), ViewConstants.SCHOOL);
            }


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

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult Admin(bool? redirectToSetup)
        {
            PrepareAdminJsonData();
            return View();
        }

        [AuthorizationFilter("Student")]
        public ActionResult Student(bool? redirectToSetup)
        {
            PrepareStudentJsonData();
            return View();
        }

        private void PrepareCommonViewData(MarkingPeriod markingPeriod = null)
        {
            //TODO: render data for demo school 
            if (Context.DistrictId.HasValue && Context.SchoolId.HasValue)
            {
                var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
                var school = MasterLocator.SchoolService.GetById(Context.SchoolId.Value);
                school.District = district;
                PrepareJsonData(ShortSchoolViewData.Create(school), ViewConstants.SCHOOL);
                if (district.IsDemoDistrict)
                {
                    ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
                    ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
                    ViewData[ViewConstants.DEMO_PREFIX_KEY] = district.Id.ToString();
                    if (Context.DeveloperId != null)
                        ViewData[ViewConstants.IS_DEV] = true;

                }
                ViewData[ViewConstants.LAST_SYNC_DATE] = district.LastSync.HasValue 
                    ? district.LastSync.Value.ToString("yyyy/MM/dd h:MM:ss")
                    : "";

            }
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.CURR_SCHOOL_YEAR_ID] = GetCurrentSchoolYearId();
            ViewData[ViewConstants.VERSION] = CompilerHelper.Version;
            ViewData[ViewConstants.CROCODOC_API_URL] = PreferenceService.Get(Preference.CROCODOC_URL).Value;
            ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString("yyyy/MM/dd");
            PrepareJsonData(Context.Claims, ViewConstants.USER_CLAIMS);
            //PrepareJsonData(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);

            ViewData[ViewConstants.UNSHOWN_NOTIFICATIONS_COUNT] = SchoolLocator.NotificationService.GetUnshownNotifications().Count;
            if (markingPeriod != null && SchoolLocator.Context.SchoolId.HasValue)
            {
                PrepareJsonData(MarkingPeriodViewData.Create(markingPeriod), ViewConstants.MARKING_PERIOD);
                var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(markingPeriod.SchoolYearRef, Context.NowSchoolTime.Date);
                if (gradingPeriod != null)
                    PrepareJsonData(GradingPeriodViewData.Create(gradingPeriod), ViewConstants.GRADING_PERIOD);

                var nextmp = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
                if (nextmp != null)
                    PrepareJsonData(MarkingPeriodViewData.Create(nextmp), ViewConstants.NEXT_MARKING_PERIOD);
            }
            PrepareJsonData(AlphaGradeViewData.Create(SchoolLocator.AlphaGradeService.GetAlphaGrades()), ViewConstants.ALPHA_GRADES);
            PrepareJsonData(AlternateScoreViewData.Create(SchoolLocator.AlternateScoreService.GetAlternateScores()), ViewConstants.ALTERNATE_SCORES);
        }
        
        private void PrepareStudentJsonData()
        {
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.FullName;
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, null, Context.UserLocalId);
            
            //todo: move this logic to getClass stored procedure later
            var classPersons = SchoolLocator.ClassService.GetClassPersons(person.Id, true);
            classes = classes.Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            
            PrepareJsonData(ClassViewData.Create(classes), ViewConstants.CLASSES);
            PrepareCommonViewData(mp);
        }           

        private void PrepareAdminJsonData()
        {
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.ShortSalutationName;
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            var gradeLevels = SchoolLocator.GradeLevelService.GetGradeLevels();
            PrepareJsonData(GradeLevelViewData.Create(gradeLevels), ViewConstants.GRADE_LEVELS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
            PrepareCommonViewData(mp);
        }

        private void PrepareTeacherJsonData(MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.ShortSalutationName;
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);

            var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, null, Context.UserLocalId.Value);
            var now = SchoolLocator.Context.NowSchoolTime;
            //if (classes.Count > 0)
            //{
            //    MarkingPeriod currentMp = mp;
            //    if(mp.StartDate > now || mp.EndDate < now)
            //        currentMp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(now);
            //    if (currentMp != null)
            //    {
            //        var cp = SchoolLocator.ClassPeriodService.GetNearestClassPeriod(null, now);
            //        if (cp != null)
            //        {
            //            var minutes = (int) (now - now.Date).TotalMinutes;
            //            if (cp.Period.StartTime - minutes <= 5)
            //            {
            //                SchoolLocator.CalendarDateService.GetCalendarDateByDate(now);
            //                var attQuery = new ClassAttendanceQuery
            //                    {
            //                        MarkingPeriodId = currentMp.Id,
            //                        ClassId = cp.ClassRef,
            //                        FromTime = cp.Period.StartTime,
            //                        ToTime = cp.Period.EndTime,
            //                        FromDate = now.Date,
            //                        ToDate = now.Date
            //                    };
            //                var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(attQuery);
            //                //check is it tour now or demo school
            //                if (attendances.Any(x => x.Type == AttendanceTypeEnum.NotAssigned))
            //                {
            //                    ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.ATTENDANCE_CLASS_LIST_URL_FORMAT, cp.ClassRef);
            //                }
            //            }
            //        }
            //    }
            //} 
            var executionResult = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(executionResult, ViewConstants.CLASSES);
            PrepareClassesAdvancedData(classes, mp);
            PrepareCommonViewData(mp);
            PrepareJsonData(GradingCommentViewData.Create(SchoolLocator.GradingCommentService.GetGradingComments()), ViewConstants.GRADING_COMMMENTS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
        }

        
        private void PrepareClassesAdvancedData(IEnumerable<ClassDetails> classDetailses, MarkingPeriod mp)
        {
            var classesAdvancedData = new List<object>();
            classDetailses = classDetailses.Where(x => x.MarkingPeriodClasses.Any(y => y.MarkingPeriodRef == mp.Id)).ToList();
            var classesMaskDic = ClassController.BuildClassesUsageMask(SchoolLocator, mp.Id, SchoolLocator.Context.SchoolTimeZoneId);
            var allAlphaGrades = SchoolLocator.AlphaGradeService.GetAlphaGrades();
            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classDetailses.Select(x => x.Id).ToList());
            foreach (var classDetails in classDetailses)
            {
                int classId = classDetails.Id;
                var typesByClasses = classAnnouncementTypes.Where(x => x.ClassRef == classId).ToList();
                classesAdvancedData.Add(new
                {
                    ClassId = classId,
                    Mask = classesMaskDic.ContainsKey(classId) ? classesMaskDic[classId] : new List<int>(),
                    TypesByClass = ClassAnnouncementTypeViewData.Create(typesByClasses),
                    AlphaGrades = classDetails.GradingScaleRef.HasValue
                                        ? SchoolLocator.AlphaGradeService.GetAlphaGradesForClass(classDetails.Id)
                                        : allAlphaGrades,
                    AlphaGradesForStandards = SchoolLocator.AlphaGradeService.GetAlphaGradesForClassStandards(classId)
                });
            }
            PrepareJsonData(classesAdvancedData, ViewConstants.CLASSES_ADV_DATA);
        }
    }
}
