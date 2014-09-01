using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FeedController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_FEED_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult List(int? start, int? count, bool? complete, int? classId)
        {
            //start = start ?? 0;
            //count = count ?? 10;
            //var list = SchoolLocator.AnnouncementService.GetAnnouncements(complete, start.Value, count.Value, 
            //    classId, null, BaseSecurity.IsAdminViewer(SchoolLocator.Context));

            //var annsIdsWithApp = list.Where(x => x.ApplicationCount == 1).Select(x=>x.Id).ToList();
            //var annApps = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp);
            //var apps = MasterLocator.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            return Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, BaseSecurity.IsAdminViewer(SchoolLocator.Context)));
            //Json(list.Transform(x => AnnouncementViewData.Create(x)));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult Admin(IntList gradeLevelIds)
        {
            return FakeJson("~/fakeData/adminFeed.json");
            throw new NotImplementedException();
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            
            //var nowDate = Context.NowSchoolTime.Date;
            //var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(nowDate);
            //if (mp == null)
            //{
            //    if (SchoolLocator.MarkingPeriodService.GetMarkingPeriods(GetCurrentSchoolYearId()).FirstOrDefault() == null)
            //        throw new NoMarkingPeriodException();
            //    return Json(AdminFeedViewData.Create(null, null, nowDate, null, null, null));
            //}
            //var departments = MasterLocator.ChalkableDepartmentService.GetChalkableDepartments();
            //var departmentGradeAvgsForAllGl = SchoolLocator.GradingStatisticService.GetDepartmentGradeAvgPerMp(mp.Id, null);
            //IList<DepartmentGradeAvg> departmentGradeAvgsByGl = new List<DepartmentGradeAvg>();
            //if (gradeLevelIds != null && gradeLevelIds.Count > 0)
            //{
            //    departmentGradeAvgsByGl = SchoolLocator.GradingStatisticService.GetDepartmentGradeAvgPerMp(mp.Id, gradeLevelIds);
            //}
            //var departmentsGradingStats = DepartmentGradingStatViewData.Create(departments, departmentGradeAvgsByGl, departmentGradeAvgsForAllGl);
            //var d = SchoolLocator.CalendarDateService.GetCalendarDateByDate(nowDate);
            //IList<StudentCountAbsentFromPeriod> studentCountAbsentFromPeriods = new List<StudentCountAbsentFromPeriod>();
            //if (d != null)
            //{
            //    var lastPeriod = SchoolLocator.PeriodService.GetPeriods(mp.SchoolYearRef).OrderByDescending(x => x.StartTime).FirstOrDefault();
            //    if (lastPeriod != null)
            //    {
            //        studentCountAbsentFromPeriods = SchoolLocator.AttendanceService.GetStudentCountAbsentFromPeriod(nowDate, nowDate,
            //            gradeLevelIds, 0, lastPeriod.Order);
            //    }
            //}
            //var attendances =  SchoolLocator.AttendanceService.GetClassAttendanceDetails(new ClassAttendanceQuery
            //                    {
            //                        FromDate = nowDate, 
            //                        ToDate = nowDate,
            //                        MarkingPeriodId = mp.Id,
            //                        Type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused
            //                    }, gradeLevelIds);
            //var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(new ClassDisciplineQuery
            //                    {
            //                        FromDate = nowDate,
            //                        ToDate = nowDate,
            //                        MarkingPeriodId = mp.Id,
            //                    }, gradeLevelIds);
            //var stAbsentForDay = SchoolLocator.AttendanceService.GetStudentCountAbsentFromDay(mp.StartDate, nowDate, gradeLevelIds);
            //return Json(AdminFeedViewData.Create(departmentsGradingStats, stAbsentForDay, nowDate, studentCountAbsentFromPeriods, disciplines, attendances));
        }


        public static IList<AnnouncementViewData> GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, int? start, int? count
            , bool? complete, int? classId, bool ownerOnly =false, bool? graded = null)
        {
            start = start ?? 0;
            count = count ?? 10;
            var list = schoolL.AnnouncementService.GetAnnouncements(complete, start.Value, count.Value, classId, null, ownerOnly, graded);
            var annsIdsWithApp = list.Where(x => x.ApplicationCount == 1).Select(x => x.Id).ToList();
            var annApps = schoolL.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp, true);
            var apps = schoolL.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            annApps = annApps.Where(x => apps.Any(a => a.Id == x.ApplicationRef)).ToList();
            return AnnouncementViewData.Create(list, annApps, apps);
        }
    }
}