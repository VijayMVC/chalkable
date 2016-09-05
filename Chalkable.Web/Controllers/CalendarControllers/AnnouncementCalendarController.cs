using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementCalendarController : CalendarController
    {
         [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult List(DateTime? date, int? classId, int? personId)
         {
             Trace.Assert(Context.PersonId.HasValue);
             DateTime start, end;
             MonthCalendar(ref date, out start, out end);
             var schoolYearId = GetCurrentSchoolYearId();
             int? studentId, teacherId;
             PrepareUsersIdsForCalendar(SchoolLocator, personId, out teacherId, out studentId);
             //var isAdmin = BaseSecurity.IsDistrictAdmin(Context);
             //var announcements = isAdmin
             //       ? SchoolLocator.AnnouncementService.GetAdminAnnouncements(null, null, start, end, 0, int.MaxValue, true, studentId)
             //       : SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, classId, true);
             var announcementList = SchoolLocator.AnnouncementFetchService.GetAnnouncementComplexList(start, end, false, classId, studentId, teacherId, false);
             if (personId.HasValue)
             {
                 var classes = SchoolLocator.ClassService.GetClasses(schoolYearId, studentId, teacherId);
                 announcementList.LessonPlans = announcementList.LessonPlans.Where(l => classes.Any(c => c.Id == l.ClassRef)).ToList();
                 announcementList.ClassAnnouncements = announcementList.ClassAnnouncements.Where(a => classes.Any(c => c.Id == a.ClassRef)).ToList();
                 announcementList.SupplementalAnnouncements = announcementList.SupplementalAnnouncements.Where(a => classes.Any(c => c.Id == a.ClassRef)).ToList();
             }
             if (DemoUserService.IsDemoUser(Context))
             {
                 //announcements = announcements.Where(x => x.State == AnnouncementState.Created).ToList();
             }
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, true, start, end);
             return Json(PrepareMonthCalendar(start, end, date.Value, (dateTime, isCurrentMonth) => 
                 AnnouncementMonthCalendarViewData.Create(dateTime, isCurrentMonth, announcementList, days, Context.Claims)));
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
         public ActionResult ListByDateRange(DateTime? startDate, DateTime? endDate, int? classId)
         {
             //todo : investigate where this method is used
             var annList = SchoolLocator.AnnouncementFetchService.GetAnnouncementComplexList(startDate, endDate, false, classId, filterByStartDate: false);
             var res = new List<AnnouncementViewData>();
             res.AddRange(annList.LessonPlans.Select(x => AnnouncementViewData.Create(x, Context.Claims)));
             res.AddRange(annList.ClassAnnouncements.Select(x => AnnouncementViewData.Create(x, Context.Claims)));
             res.AddRange(annList.AdminAnnouncements.Select(x => AnnouncementViewData.Create(x, Context.Claims)));
             return Json(res);
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
         public ActionResult ListClassAnnsByDateRange(DateTime? startDate, DateTime? endDate, int? classId)
         {
             var res = SchoolLocator.ClassAnnouncementService.GetClassAnnouncements(startDate, endDate, classId, null, null);
             return Json(ClassAnnouncementViewData.Create(res, Context.Claims));
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
         public ActionResult Week(DateTime? date, int? classId, int? personId)
         {

             var res = BuildDayAnnCalendar(SchoolLocator, date, classId, personId, GetCurrentSchoolYearId(), Context.Claims);
             return Json(res, 8);
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Schedule })]
         public ActionResult Day(DateTime? date, int? personId)
         {
             var res = BuildDayAnnCalendar(SchoolLocator, date, null, personId, GetCurrentSchoolYearId(), Context.Claims);
             return Json(res, 8);
         }

         public static IList<AnnouncementCalendarWeekViewData> BuildDayAnnCalendar(IServiceLocatorSchool locator, DateTime? date,
             int? classId, int? personId, int schoolYearId, IList<ClaimInfo> claims)
         {

             DateTime start, end;
             WeekCalendar(ref date, out start, out end, locator.Context);
             int? teacherId, studentId;
             PrepareUsersIdsForCalendar(locator, personId, out teacherId, out studentId);
             var res = new List<AnnouncementCalendarWeekViewData>();

             var announcementList = locator.AnnouncementFetchService.GetAnnouncementComplexList(start, end, true, classId, studentId, filterByStartDate: false);

             var schedule = locator.ClassPeriodService.GetSchedule(teacherId, studentId, classId, start, end);
             var classes = locator.ClassService.GetClasses(schoolYearId, studentId, teacherId);

            announcementList.LessonPlans = announcementList.LessonPlans.Where(l => classes.Any(c => c.Id == l.ClassRef)).ToList();
            announcementList.ClassAnnouncements = announcementList.ClassAnnouncements.Where(a => classes.Any(c => c.Id == a.ClassRef)).ToList();
            announcementList.SupplementalAnnouncements = announcementList.SupplementalAnnouncements.Where(a => classes.Any(c => c.Id == a.ClassRef)).ToList();
            

            for (var d = start; d <= end; d = d.AddDays(1))
             {
                 var classAnns = announcementList.ClassAnnouncements.Where(x => x.Expires.Date == d).ToList();
                 var lessonPlans = announcementList.LessonPlans.Where(x => x.StartDate <= d && x.EndDate >= d).ToList();
                 var adminAnns = announcementList.AdminAnnouncements.Where(x => x.Expires.Date == d).ToList();
                 var supplementalAnns = announcementList.SupplementalAnnouncements.Where(x => x.Expires.HasValue && x.Expires.Value.Date == d).ToList();
                 var daySchedule = schedule.Where(x => x.Day == d).ToList();
                 var annPeriods = AnnouncementPeriodViewData.Create(daySchedule, classAnns, lessonPlans, supplementalAnns, locator.Context.NowSchoolTime, claims);
                 res.Add(AnnouncementCalendarWeekViewData.Create(d, annPeriods, adminAnns));
             }
             return res;
         }


         private static void PrepareUsersIdsForCalendar(IServiceLocatorSchool locator, int? personId, out int? teacherId, out int? studentId)
         {
             if (!personId.HasValue)
             {
                 teacherId = locator.Context.Role == CoreRoles.TEACHER_ROLE || locator.Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE ? locator.Context.PersonId : null;
                 studentId = locator.Context.Role == CoreRoles.STUDENT_ROLE ? locator.Context.PersonId : null;
             }
             else
             {
                 var person = locator.PersonService.GetPersonDetails(personId.Value);
                 teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id || person.RoleRef == CoreRoles.DISTRICT_ADMIN_ROLE.Id ? person.Id : (int?)null;
                 studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : (int?)null;
             }
         }
        
    }
}