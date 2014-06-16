﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
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
         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult List(DateTime? date, int? classId, IntList gradeLevelIds, int? schoolPersonId)
         {
             if (!SchoolLocator.Context.SchoolId.HasValue)
                 throw new UnassignedUserException();
             DateTime start, end;
             MonthCalendar(ref date, out start, out end);
             var isAdmin = BaseSecurity.IsAdminViewer(SchoolLocator.Context);
             var schoolYearId = GetCurrentSchoolYearId();
             var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, gradeLevelIds, !isAdmin ? classId : null);
             if (schoolPersonId.HasValue)
             {
                 var classes = SchoolLocator.ClassService.GetClasses(schoolYearId, null, schoolPersonId);
                 announcements = announcements.Where(x => classes.Any(y => y.Id == x.ClassRef)).ToList();
             }
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, true, start, end);
             return Json(PrepareMonthCalendar(start, end, date.Value, (dateTime, isCurrentMonth) => 
                 AnnouncementMonthCalendarViewData.Create(dateTime, isCurrentMonth, announcements, days)));
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
         public ActionResult ListForWeek(DateTime? date)
         {
             DateTime start, end;
             var dates = GetWeekDates(ref date, out start, out end);
             var anns = SchoolLocator.AnnouncementService.GetAnnouncements(start, end);
             var listDayAnnouncements = AnnouncementByDateViewData.Create(dates, anns);
             return Json(listDayAnnouncements);
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
         public ActionResult ListByDateRange(DateTime? startDate, DateTime? endDate, int? classId)
         {
             var query = new AnnouncementsQuery {FromDate = startDate, ToDate = endDate, ClassId = classId};
             var anns = SchoolLocator.AnnouncementService.GetAnnouncementsComplex(query);
             return Json(AnnouncementShortViewData.Create(anns));
         }
        

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_ADMIN_DAY, true, CallType.Get, new[] { AppPermissionType.Schedule })]
         public ActionResult AdminDay(DateTime? day, IntList gradeLevelIds)
         {
             var currentDay = (day ?? SchoolLocator.Context.NowSchoolTime).Date;
             var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDay);
             if (mp != null)
             {
                 var classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(currentDay, null, null, null, null);
                 var periods = SchoolLocator.PeriodService.GetPeriods(mp.SchoolYearRef);
                 var gradeLevels = SchoolLocator.GradeLevelService.GetGradeLevels();
                 if (gradeLevelIds != null && gradeLevelIds.Count > 0)
                     gradeLevels = gradeLevels.Where(x => gradeLevelIds.Contains(x.Id)).ToList();
                 var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, mp.Id, null);
                 var rooms = SchoolLocator.RoomService.GetRooms();
                 return Json(AdminDayCalendarViewData.Create(periods, gradeLevels, classPeriods, classes, rooms, currentDay), 8);
             }
             throw new NoMarkingPeriodException();
         }

         //TODO: rewrite this method 
         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_WEEK, true, CallType.Get, new[] { AppPermissionType.Announcement })]
         public ActionResult Week(DateTime? date, int? classId, IntList gradeLevelIds, int? schoolPersonId)
         {
             DateTime start, end;
             int? teacherId, studentId;
             WeekCalendar(ref date, out start, out end);
             PrepareUsersIdsForCalendar(SchoolLocator, schoolPersonId, out teacherId, out studentId);
             var anns = SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, gradeLevelIds, classId);
             var rooms = SchoolLocator.RoomService.GetRooms();
             IList<ClassPeriod> classPeriods = new List<ClassPeriod>();
             var res = new List<AnnouncementCalendarWeekViewData>();
             var schoolYearId = GetCurrentSchoolYearId();
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, false, start, end);
             var periods = SchoolLocator.PeriodService.GetPeriods(schoolYearId);
             if (!BaseSecurity.IsAdminViewer(SchoolLocator.Context))
                 classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(schoolYearId, null, null, null, null, null, studentId, teacherId);

             IList<AnnouncementPeriodViewData> annPeriods = null;
             foreach (var d in days)
             {
                 var announcements = anns.Where(x => x.Expires.Date == d.Day.Date).ToList();
                 if (d.DayTypeRef.HasValue)
                 {
                     var cPeriods = classPeriods.Where(x => x.DayTypeRef == d.DayTypeRef).ToList();
                     annPeriods = AnnouncementPeriodViewData.Create(periods, cPeriods, d, announcements, rooms);    
                 }
                 else
                     annPeriods = AnnouncementPeriodViewData.Create(periods, new List<ClassPeriod>(), d, announcements, rooms);
                 
                 var ann = announcements.Where(x => !x.GradableType).ToList();
                 res.Add(AnnouncementCalendarWeekViewData.Create(d.Day, annPeriods, ann));
             }
             return Json(res, 6);
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_DAY, true, CallType.Get, new[] { AppPermissionType.Schedule })]
         public ActionResult Day(DateTime? date, int? schoolPersonId)
         {
             var res = BuildDayAnnCalendar(SchoolLocator, date, null, schoolPersonId, GetCurrentSchoolYearId());
             return Json(res, 8);
         }

         public static IList<AnnouncementDayCalendarViewData> BuildDayAnnCalendar(IServiceLocatorSchool locator, DateTime? date,
             int? classId, int? personId, int schoolYearId)
         {

             DateTime start, end;
             WeekCalendar(ref date, out start, out end, locator.Context);
             var res = new List<AnnouncementDayCalendarViewData>();
             int? teacherId, studentId;
             PrepareUsersIdsForCalendar(locator, personId, out teacherId, out studentId);

             IList<ClassDetails> classes = new List<ClassDetails>();
             if (classId.HasValue)
                 classes.Add(locator.ClassService.GetClassDetailsById(classId.Value));
             else
                 classes = locator.ClassService.GetClasses(null, null, teacherId ?? studentId);

             var rooms = locator.RoomService.GetRooms();
             IList<AnnouncementComplex> announcements = locator.AnnouncementService.GetAnnouncements(start, end, false, null, classId);
             IList<ClassPeriod> classPeriods = new List<ClassPeriod>();
             IList<Period> periods = new List<Period>();
             classPeriods = locator.ClassPeriodService.GetClassPeriods(schoolYearId, null, classId, null, null, null, studentId, teacherId);
             periods = locator.PeriodService.GetPeriods(schoolYearId);
                    
             var days = locator.CalendarDateService.GetLastDays(schoolYearId, false, start, end);
             while (start <= end)
             {
                 var d = days.FirstOrDefault(x => x.Day.Date == start.Date);
                 if (d != null && d.DayTypeRef.HasValue)
                 {
                     var currentDayAnns = announcements.Where(x => x.Expires.Date == d.Day).ToList();
                     var currentDayClassPeriods = classPeriods.Where(x => periods.Any(y => y.Id == x.PeriodRef) && x.DayTypeRef == d.DayTypeRef).ToList();
                     res.Add(AnnouncementDayCalendarViewData.Create(periods, d.Day, currentDayClassPeriods, classes, currentDayAnns, rooms));
                 }
                 else res.Add(AnnouncementDayCalendarViewData.Create(null, start.Date, null, null, null, null));
                 start = start.AddDays(1);
             }
             return res;
         }


         private static void PrepareUsersIdsForCalendar(IServiceLocatorSchool locator, int? personId, out int? teacherId, out int? studentId)
         {
             if (!personId.HasValue)
             {
                 teacherId = locator.Context.Role == CoreRoles.TEACHER_ROLE ? locator.Context.UserLocalId : null;
                 studentId = locator.Context.Role == CoreRoles.STUDENT_ROLE ? locator.Context.UserLocalId : null;
             }
             else
             {
                 var person = locator.PersonService.GetPerson(personId.Value);
                 teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : (int?)null;
                 studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : (int?)null;
             }
         }
        
    }
}