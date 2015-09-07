using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Data.Master.DataAccess
{
    public class CommonCoreStandardDataAccess : DataAccessBase<CommonCoreStandard, string>
    {
        public CommonCoreStandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<CommonCoreStandard> GetByABIds(IList<Guid> academicBenchmarkIds)
        {
            if(academicBenchmarkIds == null || academicBenchmarkIds.Count == 0)
                return new List<CommonCoreStandard>();

            var dbQuery = BuildGetCommonCoreStandardQuery();
            var academicBenchmarkIdsStr = academicBenchmarkIds.Select(x => string.Format("'{0}'", x)).JoinString(",");
            dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in ({2})", typeof (ABToCCMapping).Name,
                                     ABToCCMapping.ACADEMIC_BENCHMARK_ID_FIELD, academicBenchmarkIdsStr);          
            
            return ReadMany<CommonCoreStandard>(dbQuery);
        }

        public IList<CommonCoreStandard> GetByIds(IList<Guid> ids)
        {
            var dbQuery = Orm.SimpleSelect<CommonCoreStandard>(null);
            var idsStr = ids.Select(id => string.Format("'{0}'", id)).JoinString(",");
            dbQuery.Sql.AppendFormat(" where [{0}].[{1}] in ({2}) ", typeof (CommonCoreStandard).Name,
                                     CommonCoreStandard.ID_FIELD, idsStr);
            return ReadMany<CommonCoreStandard>(dbQuery);
        } 

        public IList<CommonCoreStandard> GetByFilter(string filter)
        {
            if(string.IsNullOrEmpty(filter)) return new List<CommonCoreStandard>();
            var words = filter.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if(words.Length == 0) return new List<CommonCoreStandard>();

            var dbQuery = BuildGetCommonCoreStandardQuery();
            var stadnardTName = typeof (CommonCoreStandard).Name;
            dbQuery.Sql.Append(" and (");
            for (var i = 0; i < words.Length; i++)
            {
                if (i > 0) dbQuery.Sql.Append(" or ");
                var paramkey = string.Format("@word{0}", i + 1);
                dbQuery.Sql.AppendFormat(" [{0}].[{1}] like {2} ", stadnardTName, CommonCoreStandard.CODE_FIELD, paramkey);
                dbQuery.Parameters.Add(paramkey, string.Format("%{0}%", words[i]));
            }
            dbQuery.Sql.Append(")");
            return ReadMany<CommonCoreStandard>(dbQuery);
        }

        private DbQuery BuildGetCommonCoreStandardQuery()
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(
                "select [{0}].*, [{1}].[{4}] as [{5}]  from [{0}] join [{1}] on [{1}].[{3}] = [{0}].[{2}] where 1=1 "
                , typeof(CommonCoreStandard).Name, typeof(ABToCCMapping).Name, CommonCoreStandard.ID_FIELD, ABToCCMapping.CC_STANADARD_REF_FIELD
                                     , ABToCCMapping.ACADEMIC_BENCHMARK_ID_FIELD, CommonCoreStandard.ACADEMIC_BENCHMARK_ID_FIELD);

            return dbQuery;
        }
    }
}
