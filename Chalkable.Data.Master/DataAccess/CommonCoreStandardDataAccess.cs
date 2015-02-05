using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class CommonCoreStandardDataAccess : DataAccessBase<CommonCoreStandard, string>
    {
        public CommonCoreStandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<CommonCoreStandard> GetByABIds(IList<Guid> academicBenchmarkIds)
        {
            var dbQuery = new DbQuery();
            if(academicBenchmarkIds == null || academicBenchmarkIds.Count == 0)
                return new List<CommonCoreStandard>();
            var academicBenchmarkIdsStr = academicBenchmarkIds.Select(x => string.Format("'{0}'", x)).JoinString(",");
            dbQuery.Sql.AppendFormat(@"select distinct [{0}].*, [{1}].[{4}] as [{6}]  from [{0}] join [{1}] on [{1}].[{3}] = [{0}].[{2}] 
                                       where [{1}].[{4}] in ({5})"
                                     , typeof (CommonCoreStandard).Name, typeof (ABToCCMapping).Name,
                                     CommonCoreStandard.ID_FIELD, ABToCCMapping.CC_STANADARD_REF_FIELD
                                     , ABToCCMapping.ACADEMIC_BENCHMARK_ID_FIELD, academicBenchmarkIdsStr
                                     ,CommonCoreStandard.ACADEMIC_BENCHMARK_ID_FIELD);

            return ReadMany<CommonCoreStandard>(dbQuery);
        } 
    }
}
