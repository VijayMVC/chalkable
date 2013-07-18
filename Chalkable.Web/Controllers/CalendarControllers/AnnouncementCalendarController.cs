using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers
{
     [RequireHttps, TraceControllerFilter]
    public class AnnouncementCalendarController : CalendarController
    {
         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
         public ActionResult List(DateTime? date, Guid? classId, GuidList gradelevelids)
         {
             if (!SchoolLocator.Context.SchoolId.HasValue)
                 throw new UnassignedUserException();
             DateTime start, end;
             MonthCalendar(ref date, out start, out end);
             var isAdmin = BaseSecurity.IsAdminViewer(SchoolLocator.Context);
             var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, gradelevelids, !isAdmin ? classId : null);
             if (isAdmin)
                 announcements = announcements.Where(x => !x.ClassId.HasValue).ToList();
             var schoolYearId = GetCurrentSchoolYearId();
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, true, start, end);
             return Json(PrepareMonthCalendar(start, end, date.Value, (time, b) => AnnouncementMonthCalendarViewData.Create(time, b, announcements, days)));
         }
    }
}