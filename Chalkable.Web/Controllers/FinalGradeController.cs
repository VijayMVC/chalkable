using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.FinalGradesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FinalGradeController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Parent")]
        public ActionResult List(int status, int? start, int? count)
        {
            var finalGrades = SchoolLocator.FinalGradeService.GetPaginatedFinalGrades(
                (FinalGradeStatus) status, start ?? 0, count ?? DEFAULT_PAGE_SIZE);
            return Json(finalGrades.Transform(FinalGradeViewData.Create));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Get(Guid classId, Guid? markingPeriodId, bool needBuildItems)
        {
            return Json(FinalGradeViewData.Create(InternalGet(classId, ref markingPeriodId, needBuildItems)), 6);
        }
        [AuthorizationFilter("Teacher")]
        public ActionResult GetDetailed(Guid classId, Guid? markingPeriodId, bool needBuildItems)
        {
            var fg = InternalGet(classId, ref markingPeriodId, needBuildItems);
            var gradingStats = SchoolLocator.GradingStatisticService.GetStudentClassGradeStats(markingPeriodId.Value, classId, null);
            var gradingMapper = SchoolLocator.GradingStyleService.GetMapper();
            return Json(FinalGradeDetailsViewData.Create(fg, gradingStats, gradingMapper));
        }

        private FinalGradeDetails InternalGet(Guid classId, ref Guid? markingPeriodId, bool needBuildItems)
        {
            markingPeriodId = EnsureMarkingPeriodId(markingPeriodId);
            var mpClass = SchoolLocator.MarkingPeriodService.GetMarkingPeriodClass(classId, markingPeriodId.Value);
            return SchoolLocator.FinalGradeService.GetFinalGrade(mpClass.Id, needBuildItems);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Update(FinalGradeInputModel finalGradeInfo)
        {
            var byType = new List<FinalGradeAnnouncementType>();
            for (int i = 0; i < finalGradeInfo.FinalGradeAnnouncementType.Count; i++)
            {
                byType.Add(new FinalGradeAnnouncementType
                {
                    Id = finalGradeInfo.FinalGradeAnnouncementType[i].FinalGradeAnnouncementTypeId,
                    DropLowest = finalGradeInfo.FinalGradeAnnouncementType[i].DropLowest,
                    FinalGradeRef = finalGradeInfo.FinalGradeId,
                    GradingStyle = (GradingStyleEnum)finalGradeInfo.FinalGradeAnnouncementType[i].GradingStyle,
                    PercentValue = finalGradeInfo.FinalGradeAnnouncementType[i].PercentValue
                });
            }
            var finalGrade = SchoolLocator.FinalGradeService.Update(finalGradeInfo.FinalGradeId, finalGradeInfo.ParticipationPercent,
                  finalGradeInfo.Attendance, finalGradeInfo.DropLowestAttendance, finalGradeInfo.Discipline, finalGradeInfo.DropLowestDiscipline
                  , (GradingStyleEnum)finalGradeInfo.GradingStyle, byType);
            return Json(FinalGradeViewData.Create(finalGrade), 6);         
        }


        [AuthorizationFilter("Teacher")]
        public ActionResult Submit(Guid finalGradeId)
        {
            SchoolLocator.FinalGradeService.Submit(finalGradeId);
            var finalGrade = SchoolLocator.FinalGradeService.GetFinalGrade(finalGradeId);
            return Json(FinalGradeViewData.Create(finalGrade));
        }

        [AuthorizationFilter("AdminGrade")]
        public ActionResult ApproveReject(Guid finalGradeId, bool isApprove)
        {
            var res = SchoolLocator.FinalGradeService.ApproveReject(finalGradeId, isApprove);
            return Json(res);
        }
    }
}