using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            using (var uow = Read())
            {
                var conds = new AndQueryCondition();
                if(standardCategoryId.HasValue)
                    conds.Add(CommonCoreStandard.STANDARD_CATEGORY_REF_FIELD, standardCategoryId);
                return new CommonCoreStandardDataAccess(uow).GetAll(conds);
            }
        }

        public IList<CC_StandardCategory> GetCCStandardCategories(Guid? paretnCategoryId, bool allCategories = true)
        {
            using (var uow = Read())
            {
                var conds = new AndQueryCondition();
                if (!allCategories || paretnCategoryId.HasValue)
                    conds.Add(CC_StandardCategory.PARENT_CATEGORY_REF_FIELD, paretnCategoryId);
                return new CC_StandardCategoryDataAccess(uow).GetAll();
            }
        }
    }
}
