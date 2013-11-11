using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AttendanceReasonDataAccess : DataAccessBase<AttendanceReason, int>
    {
        public AttendanceReasonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private DbQuery BuildGetReasonsDbQuery(QueryCondition attReasonConds, QueryCondition attLevelReasonConds = null)
        {
            var res = new DbQuery();
            var types = new List<Type> {typeof (AttendanceReason), typeof (AttendacneLevelReason)};
            res.Sql.AppendFormat(@"select {0} from [{1}] join [{3}] on [{3}].[{4}] = [{1}].[{2}]"
                                 , Orm.ComplexResultSetQuery(types), types[0].Name, AttendanceReason.ID_FIELD
                                 , types[1].Name, AttendacneLevelReason.ATTENDACNE_REASON_REF_FIELD);

            if(attReasonConds != null)
                attReasonConds.BuildSqlWhere(res, types[0].Name);
            if(attLevelReasonConds != null)
                attLevelReasonConds.BuildSqlWhere(res, types[1].Name, attReasonConds != null);
            return res;
        }

        public override IList<AttendanceReason> GetAll(QueryCondition conditions = null)
        {
            return ReadMany<AttendanceReason>(BuildGetReasonsDbQuery(conditions), true);
        }
    }

    public class AttendanceLevelReasonDataAccess : DataAccessBase<AttendacneLevelReason, int>
    {
        public AttendanceLevelReasonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
