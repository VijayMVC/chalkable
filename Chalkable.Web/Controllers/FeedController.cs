using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
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
        public ActionResult List(int? start, int? count, bool? starredOnly, Guid? classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();

            var list = SchoolLocator.AnnouncementService.GetAnnouncements(starredOnly ?? false, start ?? 0, count ?? 10, 
                classId, null, BaseSecurity.IsAdminViewer(SchoolLocator.Context));
            return Json(list.Transform(x => AnnouncementViewData.Create(x)));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult Admin(GuidList gradeLevelIds)
        {
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            
            var nowDate = Context.NowSchoolTime.Date;
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(nowDate);
            if (mp == null)
            {
                if (SchoolLocator.MarkingPeriodService.GetMarkingPeriods(GetCurrentSchoolYearId()).FirstOrDefault() == null)
                    throw new NoMarkingPeriodException();
                return Json(AdminFeedViewData.Create(null, null, nowDate));
            }

            var departments = MasterLocator.ChalkableDepartmentService.GetChalkableDepartments();
            var departmentGradeAvgsForAllGl = SchoolLocator.GradingStatisticService.GetDepartmentGradeAvgPerMp(mp.Id, null);
            IList<DepartmentGradeAvg> departmentGradeAvgsByGl = new List<DepartmentGradeAvg>();
            if (gradeLevelIds != null && gradeLevelIds.Count > 0)
            {
                departmentGradeAvgsByGl = SchoolLocator.GradingStatisticService.GetDepartmentGradeAvgPerMp(mp.Id, gradeLevelIds);
            }
            var departmentsGradingStats = DepartmentGradingStatViewData.Create(departments, departmentGradeAvgsByGl, departmentGradeAvgsForAllGl);
            var stAbsentForDay = SchoolLocator.AttendanceService.GetStudentCountAbsentFromDay(mp.StartDate, nowDate, gradeLevelIds);
            
            return Json(AdminFeedViewData.Create(departmentsGradingStats, stAbsentForDay, nowDate));
        }
    }
}