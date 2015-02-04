using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICommonCoreStandardService
    {
        IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId = null, Guid? parentStandardId = null, bool allStandards = true);
        IList<CommonCoreStandardCategory> GetCCStandardCategories();
        CommonCoreStandard GetStandardByABId(Guid academicBenchmarkId);
        IList<CommonCoreStandard> GetStandardsByABIds(IList<Guid> academicBenchmarkIds);
    }

    public class CommonCoreStandardService : MasterServiceBase, ICommonCoreStandardService
    {
        public CommonCoreStandardService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId, Guid? parentStandardId, bool allStandards = true)
        {
            var conds = new AndQueryCondition();
            if (standardCategoryId.HasValue)
                conds.Add(CommonCoreStandard.STANDARD_CATEGORY_REF_FIELD, standardCategoryId);
            if(!allStandards || parentStandardId.HasValue)
                conds.Add(CommonCoreStandard.PARENT_STANDARD_REF_FIELD, parentStandardId);
            return DoRead(u => new CommonCoreStandardDataAccess(u).GetAll(conds));
        }

        public IList<CommonCoreStandardCategory> GetCCStandardCategories()
        {
            return DoRead(u => new CC_StandardCategoryDataAccess(u).GetAll());
        }


        public CommonCoreStandard GetStandardByABId(Guid academicBenchmarkId)
        {
            var abIds = new List<Guid> {academicBenchmarkId};
            return GetStandardsByABIds(abIds).FirstOrDefault();
        }

        public IList<CommonCoreStandard> GetStandardsByABIds(IList<Guid> academicBenchmarkIds)
        {
            if(academicBenchmarkIds.Count == 0) return new List<CommonCoreStandard>();
            return DoRead(uow => new CommonCoreStandardDataAccess(uow).GetByABIds(academicBenchmarkIds));
        }
    }
}
