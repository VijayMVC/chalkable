using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Data.Master.DataAccess
{
    public class ABToCCMappingDataAccess : DataAccessBase<ABToCCMapping, Guid>
    {
        public ABToCCMappingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AbToCCMappingDetails> GetDetailsList()
        {
            var query = new DbQuery();
            var types = new List<Type> {typeof (ABToCCMapping), typeof (CommonCoreStandard)};
            query.Sql.AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), types[0].Name)
                 .Append(" ")
                 .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, types[1].Name, CommonCoreStandard.ID_FIELD,
                               types[0].Name, ABToCCMapping.CC_STANADARD_REF_FIELD);
            return ReadMany<AbToCCMappingDetails>(query, true);
        } 
    }
}
