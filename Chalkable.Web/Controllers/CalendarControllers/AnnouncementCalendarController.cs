using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
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
             if (!SchoolLocator.Context.PersonId.HasValue)
                 throw new UnassignedUserException();
             DateTime start, end;
             MonthCalendar(ref date, out start, out end);
             var schoolYearId = GetCurrentSchoolYearId();
             int? studentId, teacherId;
             PrepareUsersIdsForCalendar(SchoolLocator, personId, out teacherId, out studentId);
             var isAdmin = BaseSecurity.IsDistrictAdmin(Context);
             var announcements = isAdmin
                    ? SchoolLocator.AnnouncementService.GetAdminAnnouncements(null, null, start, end, 0, int.MaxValue, true, studentId)
                    : SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, classId, true);
             if (personId.HasValue && !isAdmin)
             {
                 var classes = SchoolLocator.ClassService.GetClasses(schoolYearId, studentId, teacherId);
                 announcements = announcements.Where(a => classes.Any(c => c.Id == a.ClassRef)).ToList();
             }
             if (DemoUserService.IsDemoUser(Context))
             {
                 announcements = announcements.Where(x => x.State == AnnouncementState.Created).ToList();
             }
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, true, start, end);
             return Json(PrepareMonthCalendar(start, end, date.Value, (dateTime, isCurrentMonth) => 
                 AnnouncementMonthCalendarViewData.Create(dateTime, isCurrentMonth, announcements, days)));
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
         public ActionResult ListByDateRange(DateTime? startDate, DateTime? endDate, int? classId)
         {
             var query = new AnnouncementsQuery {FromDate = startDate, ToDate = endDate, ClassId = classId};
             var anns = SchoolLocator.AnnouncementService.GetAnnouncementsComplex(query);
             return Json(AnnouncementShortViewData.Create(anns));
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
         public ActionResult Week(DateTime? date, int? classId, int? personId)
         {

             var res = BuildDayAnnCalendar(SchoolLocator, date, classId, personId, GetCurrentSchoolYearId());
             return Json(res, 8);
         }

         [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Schedule })]
         public ActionResult Day(DateTime? date, int? personId)
         {
             var res = BuildDayAnnCalendar(SchoolLocator, date, null, personId, GetCurrentSchoolYearId());
             return Json(res, 8);
         }

         public static IList<AnnouncementCalendarWeekViewData> BuildDayAnnCalendar(IServiceLocatorSchool locator, DateTime? date,
             int? classId, int? personId, int schoolYearId)
         {

             DateTime start, end;
             WeekCalendar(ref date, out start, out end, locator.Context);
             int? teacherId, studentId;
             PrepareUsersIdsForCalendar(locator, personId, out teacherId, out studentId);
             var res = new List<AnnouncementCalendarWeekViewData>();
             var announcements = BaseSecurity.IsDistrictAdmin(locator.Context) 
                 ? locator.AnnouncementService.GetAdminAnnouncements(null, null, start, end, 0, int.MaxValue, true, studentId)
                 : locator.AnnouncementService.GetAnnouncements(start, end, false, classId, true);
             var schedule = locator.ClassPeriodService.GetSchedule(teacherId, studentId, classId, start, end);
             var classes = locator.ClassService.GetClasses(schoolYearId, studentId, teacherId);
             announcements = announcements.Where(a => a.IsAdminAnnouncement ||  classes.Any(c => c.Id == a.ClassRef)).ToList();
             for (var d = start; d <= end; d = d.AddDays(1))
             {
                 var currentDayAnns = announcements.Where(x => x.Expires.Date == d).ToList();
                 var daySchedule = schedule.Where(x => x.Day == d).ToList();
                 var annPeriods = AnnouncementPeriodViewData.Create(daySchedule, currentDayAnns, locator.Context.NowSchoolTime);
                 res.Add(AnnouncementCalendarWeekViewData.Create(d, annPeriods, currentDayAnns));
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