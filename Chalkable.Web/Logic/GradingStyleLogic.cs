using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models;

namespace Chalkable.Web.Logic
{
    public class GradingStyleLogic
    {

        public static GradingStyleViewData GetGradingStyleMapper(IServiceLocatorSchool locator)
        {
            var gradingStyleMapper = locator.GradingStyleService.GetMapper();
            var gradingAbcfList = gradingStyleMapper.GetValuesByStyle(GradingStyleEnum.Abcf);
            var gradingCompeleList = gradingStyleMapper.GetValuesByStyle(GradingStyleEnum.Complete);
            var gradingCheckList = gradingStyleMapper.GetValuesByStyle(GradingStyleEnum.Check);
            return GradingStyleViewData.Create(gradingAbcfList, gradingCompeleList, gradingCheckList);
        }
    }
}