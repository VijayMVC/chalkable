using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ABToCCMappingDataAccess : DataAccessBase<ABToCCMapping, Guid>
    {
        public ABToCCMappingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AbToCCMappingDetails> GetDetailsList()
        {
            var query = SelectDetailsQuery();
            return ReadMany<AbToCCMappingDetails>(query, true);
        }

        public IList<AbToCCMappingDetails> GetDetailsListByAdIds(IList<Guid> academicBenchamrIds)
        {
            if(academicBenchamrIds == null || academicBenchamrIds.Count == 0)
                return new List<AbToCCMappingDetails>();
            var query = SelectDetailsQuery();
            var idsStr = academicBenchamrIds.Select(x => $"'{x.ToString()}'").JoinString(",");
            query.Sql.AppendFormat(
                $" Where  [{nameof(ABToCCMapping)}].[{ABToCCMapping.ACADEMIC_BENCHMARK_ID_FIELD}] in ({idsStr})");
            return ReadMany<AbToCCMappingDetails>(query, true);
        } 

        private DbQuery SelectDetailsQuery()
        {
            var query = new DbQuery();
            var types = new List<Type> { typeof(ABToCCMapping), typeof(CommonCoreStandard) };
            query.Sql.AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), types[0].Name)
                 .Append(" ")
                 .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, types[1].Name, CommonCoreStandard.ID_FIELD,
                               types[0].Name, ABToCCMapping.CC_STANADARD_REF_FIELD);
            return query;
        }
    }
}
