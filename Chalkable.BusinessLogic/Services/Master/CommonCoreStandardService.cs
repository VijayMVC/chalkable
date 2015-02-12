using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICommonCoreStandardService
    {
        void AddStandards(IList<CommonCoreStandard> commonCoreStandards);
        void AddStandardsCategories(IList<CommonCoreStandardCategory> standardCategories);
        void AddABToCCMapping(IList<ABToCCMapping> abtoCcMappings);

        IList<ABToCCMapping> GetABToCCMappings(Guid? academicBenchmarkId, Guid? ccStandardId); 
        IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId = null, Guid? parentStandardId = null, bool allStandards = true);
        IList<CommonCoreStandardCategory> GetCCStandardCategories();
        CommonCoreStandard GetStandardByABId(Guid academicBenchmarkId);
        IList<CommonCoreStandard> GetStandardsByABIds(IList<Guid> academicBenchmarkIds);
        IList<CommonCoreStandard> GetStandards(string filter);
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


        public IList<CommonCoreStandard> GetStandards(string filter)
        {
            if(string.IsNullOrEmpty(filter)) return new List<CommonCoreStandard>();

            return DoRead(uow => new CommonCoreStandardDataAccess(uow).GetByFilter(filter));
        }

        public void AddStandards(IList<CommonCoreStandard> commonCoreStandards)
        {
            //if (!BaseSecurity.IsSysAdmin(Context))
            //    throw new ChalkableSecurityException();
            DoUpdate(uow=> new CommonCoreStandardDataAccess(uow).Insert(commonCoreStandards));
        }

        public void AddStandardsCategories(IList<CommonCoreStandardCategory> standardCategories)
        {
            //if (!BaseSecurity.IsSysAdmin(Context))
            //    throw new ChalkableSecurityException();

            DoUpdate(uow => new CC_StandardCategoryDataAccess(uow).Insert(standardCategories));
        }

        public void AddABToCCMapping(IList<ABToCCMapping> abtoCcMappings)
        {
            //if (!BaseSecurity.IsSysAdmin(Context))
            //    throw new ChalkableSecurityException();
            DoUpdate(uow => new ABToCCMappingDataAccess(uow).Insert(abtoCcMappings));
        }


        public IList<ABToCCMapping> GetABToCCMappings(Guid? academicBenchmarkId, Guid? ccStandardId)
        {
            var conds = new AndQueryCondition();
            if(academicBenchmarkId.HasValue)
                conds.Add(ABToCCMapping.ACADEMIC_BENCHMARK_ID_FIELD, academicBenchmarkId);
            if(ccStandardId.HasValue)
                conds.Add(ABToCCMapping.CC_STANADARD_REF_FIELD, ccStandardId);
            return DoRead(uow => new ABToCCMappingDataAccess(uow).GetAll(conds));
        }
    }
}
