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
        void AddStandardsCategories(IList<CommonCoreStandardCategory> standardCategories);
        IList<CommonCoreStandardCategory> GetCCStandardCategories();

        void AddStandards(IList<CommonCoreStandard> commonCoreStandards);
        IList<CommonCoreStandard> GetStandards(Guid? standardCategoryId = null, Guid? parentStandardId = null, bool allStandards = true);
        IList<CommonCoreStandard> GetStandards(string filter);
        IList<CommonCoreStandard> GetStandardsByIds(IList<Guid> ids); 

        void AddABToCCMapping(IList<ABToCCMapping> abtoCcMappings);
        IList<ABToCCMapping> GetABToCCMappings(Guid? academicBenchmarkId, Guid? ccStandardId);
        string GetStandardCodeByABId(Guid academicBenchmarkIds);
        IDictionary<Guid, CommonCoreStandard> GetAbToCCMapper();
        void BuildAbToCCMapper();
    }

    public class CommonCoreStandardService : MasterServiceBase, ICommonCoreStandardService
    {
        public CommonCoreStandardService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }


        private static IDictionary<Guid, CommonCoreStandard> abToccMapper;
        public void BuildAbToCCMapper()
        {
            var abToccMappingDetailsList = DoRead(u => new ABToCCMappingDataAccess(u).GetDetailsList());
            abToccMapper = abToccMappingDetailsList.ToDictionary(x => x.AcademicBenchmarkId, 
                x =>
                {
                    x.Standard.AcademicBenchmarkId = x.AcademicBenchmarkId;
                    return x.Standard;
                });           
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

        public IList<CommonCoreStandard> GetStandards(string filter)
        {
            if(string.IsNullOrEmpty(filter)) return new List<CommonCoreStandard>();

            return DoRead(uow => new CommonCoreStandardDataAccess(uow).GetByFilter(filter));
        }

        public IList<CommonCoreStandard> GetStandardsByIds(IList<Guid> ids)
        {
            if (ids.Count == 0) return new List<CommonCoreStandard>();
            return DoRead(u => new CommonCoreStandardDataAccess(u).GetByIds(ids));
        }

        public void AddStandards(IList<CommonCoreStandard> commonCoreStandards)
        {
            DoUpdate(uow=> new CommonCoreStandardDataAccess(uow).Insert(commonCoreStandards));
        }

        public void AddStandardsCategories(IList<CommonCoreStandardCategory> standardCategories)
        {
            DoUpdate(uow => new CC_StandardCategoryDataAccess(uow).Insert(standardCategories));
        }

        public void AddABToCCMapping(IList<ABToCCMapping> abtoCcMappings)
        {
            DoUpdate(uow => new ABToCCMappingDataAccess(uow).Insert(abtoCcMappings));
            BuildAbToCCMapper();
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

        public string GetStandardCodeByABId(Guid academicBenchmarkIds)
        {
            var  mapper = GetAbToCCMapper();
            return !mapper.ContainsKey(academicBenchmarkIds) ? null : mapper[academicBenchmarkIds].Code;
        }

        public IDictionary<Guid, CommonCoreStandard> GetAbToCCMapper()
        {
            if(abToccMapper == null)
                BuildAbToCCMapper();
            return abToccMapper;
        }
    }
}
