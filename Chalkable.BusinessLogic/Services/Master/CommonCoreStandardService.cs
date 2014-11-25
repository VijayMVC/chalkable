using System;
using System.Collections.Generic;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICommonCoreStandardService
    {
        IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId);
        IList<CC_StandardCategory> GetCCStandardCategories(Guid? paretnCategoryId, bool allCategories = true);
    }

    public class CommonCoreStandardService : MasterServiceBase, ICommonCoreStandardService
    {
        public CommonCoreStandardService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId)
        {
            var conds = new AndQueryCondition();
            if (standardCategoryId.HasValue)
                conds.Add(CommonCoreStandard.STANDARD_CATEGORY_REF_FIELD, standardCategoryId);
            return DoRead(u => new CommonCoreStandardDataAccess(u).GetAll(conds));
        }

        public IList<CC_StandardCategory> GetCCStandardCategories(Guid? paretnCategoryId, bool allCategories = true)
        {
            var conds = new AndQueryCondition();
            if (!allCategories || paretnCategoryId.HasValue)
                conds.Add(CC_StandardCategory.PARENT_CATEGORY_REF_FIELD, paretnCategoryId);
            return DoRead(u => new CC_StandardCategoryDataAccess(u).GetAll(conds));
        }
    }
}
