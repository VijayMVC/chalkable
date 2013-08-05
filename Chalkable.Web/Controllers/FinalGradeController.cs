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
            markingPeriodId = EnsureMarkingPeriodId(markingPeriodId);
            var mpClass = SchoolLocator.MarkingPeriodService.GetMarkingPeriodClass(classId, markingPeriodId.Value);
            var finalGrade = SchoolLocator.FinalGradeService.GetFinalGrade(mpClass.Id, needBuildItems);
            //TODO: get announcement if needs  
            return Json(FinalGradeViewData.Create(finalGrade), 6);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Update(FinalGrade finalGradeInfo, GuidList finalGradeAnnouncementTypeIds, IntList perscents, IntList dropLowest, IntList gradingStyleBytype)
        {
            if (finalGradeAnnouncementTypeIds.Count != perscents.Count 
                || finalGradeAnnouncementTypeIds.Count != dropLowest.Count
                || finalGradeAnnouncementTypeIds.Count != gradingStyleBytype.Count)
                throw new ChalkableException(ChlkResources.ERR_FINALGRADE_LISTS_NOT_THE_SAME_SIZE);
            var byType = new List<FinalGradeAnnouncementType>();
           
            for (int i = 0; i < finalGradeAnnouncementTypeIds.Count; i++)
            {
                byType.Add(new FinalGradeAnnouncementType
                {
                    Id = finalGradeAnnouncementTypeIds[i],
                    DropLowest = dropLowest[i] == 1,
                    FinalGradeRef = finalGradeInfo.Id,
                    GradingStyle = (GradingStyleEnum)gradingStyleBytype[i],
                    PercentValue = perscents[i]
                });
            }
            var finalGrade = SchoolLocator.FinalGradeService.Update(finalGradeInfo.Id, finalGradeInfo.ParticipationPercent,
                  finalGradeInfo.Attendance, finalGradeInfo.DropLowestAttendance, finalGradeInfo.Discipline, finalGradeInfo.DropLowestDiscipline
                  , finalGradeInfo.GradingStyle, byType);
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