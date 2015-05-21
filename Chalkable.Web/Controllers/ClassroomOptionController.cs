using System.Web.Mvc;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ClassroomOptionController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher")]
        public ActionResult Update(ClassroomOptionViewData inputModel)
        {
            var classroomOption = new ClassroomOption
                {
                    Id = inputModel.ClassId,
                    AveragingMethodType = (AveragingMethodTypeEnum)inputModel.AveragingMethod,
                    DisplayAlphaGrade = inputModel.DisplayAlphaGrade,
                    DisplayStudentAverage = inputModel.DisplayStudentAverage,
                    DisplayTotalPoints = inputModel.DisplayTotalPoints,
                    IncludeWithdrawnStudents = inputModel.IncludeWithdrawnStudents,
                    StandardsCalculationMethod = inputModel.StandardsCalculationMethod,
                    StandardsCalculationRule = inputModel.StandardsCalculationRule,
                    StandardsCalculationWeightMaximumValues = inputModel.StandardsCalculationWeightMaximumValues,
                    StandardsGradingScaleRef = inputModel.StandardsGradingScaleId,
                    RoundDisplayedAverages = inputModel.RoundDisplayedAverages
                };
            SchoolLocator.ClassroomOptionService.SetUpClassroomOption(classroomOption);
            return Json(ClassroomOptionViewData.Create(classroomOption));
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher")]
        public ActionResult Get(int classId)
        {
            var classroomOption = SchoolLocator.ClassroomOptionService.GetClassOption(classId, true);
            return Json(ClassroomOptionViewData.Create(classroomOption));
        }
    }
}