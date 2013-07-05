using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class GradingStyleController:ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_GRADING_STYLE_LIST, true, CallType.Get, new[] { AppPermissionType.Grade })]
        public ActionResult List()
        {
            var gradingStyleMapper = SchoolLocator.GradingStyleService.GetMapper();

            var gradingAbcfList = gradingStyleMapper.GetValuesByStyle(GradingStyleEnum.Abcf);
            var gradingCompeleList = gradingStyleMapper.GetValuesByStyle(GradingStyleEnum.Complete);
            var gradingCheckList = gradingStyleMapper.GetValuesByStyle(GradingStyleEnum.Check);
            
            return Json(GradingStyleVeiwData.Create(gradingAbcfList, gradingCompeleList, gradingCheckList));
        }

        [AuthorizationFilter("AdminGrade")]
        public ActionResult Update(IntList gradingAbcf, IntList gradingCompele, IntList gradingCheck)
        {
            IDictionary<GradingStyleEnum, List<int>> gradingStyleDictionary = new Dictionary<GradingStyleEnum, List<int>>();
            gradingStyleDictionary.Add(GradingStyleEnum.Abcf, gradingAbcf);
            gradingStyleDictionary.Add(GradingStyleEnum.Complete, gradingCompele);
            gradingStyleDictionary.Add(GradingStyleEnum.Check, gradingCheck);
            
             foreach (var gradingStyleEnum in gradingStyleDictionary.Keys)
             {
                 if (!GradingStyleValueValidation(gradingStyleDictionary[gradingStyleEnum]))
                     return Json(false);
             } 

            var gradingStyleMapper = GradingStyleMapper.Create(gradingStyleDictionary);
            SchoolLocator.GradingStyleService.SetMapper(gradingStyleMapper);
            var mapper = SchoolLocator.GradingStyleService.GetMapper();
            return Json(GradingStyleVeiwData.Create(mapper.GetValuesByStyle(GradingStyleEnum.Abcf), 
                                  mapper.GetValuesByStyle(GradingStyleEnum.Complete), mapper.GetValuesByStyle(GradingStyleEnum.Check)));
        }

        private bool GradingStyleValueValidation(List<int> gradingValues)
        {
            int lastCorrectGrade = -2;
            for (int i = 0; i < gradingValues.Count; i++)
            {
                if (gradingValues[i] >= 0)
                {
                    if (lastCorrectGrade != -2 && gradingValues[i] < lastCorrectGrade
                        || lastCorrectGrade == -2 && gradingValues[i] == 100)
                        lastCorrectGrade = gradingValues[i];
                    else return false;
                }
            }
            return lastCorrectGrade != -2;
        }
    }
}