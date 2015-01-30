using System;
using System.Collections.Generic;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICommonCoreStandardService
    {
        IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId = null, Guid? parentStandardId = null);
        IList<CommonCoreStandardCategory> GetCCStandardCategories();
    }

    public class CommonCoreStandardService : MasterServiceBase, ICommonCoreStandardService
    {
        public CommonCoreStandardService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId, Guid? parentStandardId)
        {
            var conds = new AndQueryCondition();
            if (standardCategoryId.HasValue)
                conds.Add(CommonCoreStandard.STANDARD_CATEGORY_REF_FIELD, standardCategoryId);
            if(parentStandardId.HasValue)
                conds.Add(CommonCoreStandard.PARENT_STANDARD_REF_FIELD, parentStandardId);
            return DoRead(u => new CommonCoreStandardDataAccess(u).GetAll(conds));
        }

        public IList<CommonCoreStandardCategory> GetCCStandardCategories()
        {
            return DoRead(u => new CC_StandardCategoryDataAccess(u).GetAll());
        }
    }
}
